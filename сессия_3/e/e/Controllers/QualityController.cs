using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using e.Models;

namespace e.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QualityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QualityController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/quality/batches/pending - партии, ожидающие контроля
        [HttpGet("batches/pending")]
        public async Task<IActionResult> GetPendingBatches()
        {
            var batches = await _context.Batches
                .Include(b => b.Product)
                .Where(b => b.Status == "completed" && b.LabStatus == "pending")
                .Select(b => new
                {
                    b.Id,
                    b.BatchNumber,
                    b.ProductId,
                    ProductName = b.Product != null ? b.Product.Name : null,
                    b.StartTime,
                    b.EndTime,
                    b.ActualQuantityKg,
                    b.LabStatus
                })
                .ToListAsync();

            return Ok(batches);
        }

        // GET: api/quality/batches/{id}/controls - результаты контроля партии
        [HttpGet("batches/{id}/controls")]
        public async Task<IActionResult> GetBatchControls(int id)
        {
            var controls = await _context.QualityControls
                .Where(q => q.BatchId == id)
                .OrderByDescending(q => q.AnalysisDate)
                .ToListAsync();

            return Ok(controls);
        }

        // POST: api/quality/control - создать контроль качества
        [HttpPost("control")]
        public async Task<IActionResult> CreateQualityControl([FromBody] CreateQualityControlRequest request)
        {
            var batch = await _context.Batches.FindAsync(request.BatchId);
            if (batch == null)
                return BadRequest(new { message = "Партия не найдена" });

            // Обновляем статус лабораторного контроля
            batch.LabStatus = "in_progress";
            await _context.SaveChangesAsync();

            foreach (var param in request.Parameters)
            {
                var qualityControl = new QualityControl
                {
                    BatchId = request.BatchId,
                    SampleType = request.SampleType,
                    ParameterName = param.ParameterName,
                    MeasuredValue = param.MeasuredValue,
                    Unit = param.Unit,
                    StandardValue = GetStandardValue(param.ParameterName),
                    Result = CheckResult(param.MeasuredValue, GetStandardValue(param.ParameterName)),
                    Decision = "pending",
                    AnalystId = GetCurrentUserId(),
                    AnalysisDate = DateTime.UtcNow,
                    Comment = param.Comment
                };

                _context.QualityControls.Add(qualityControl);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Результаты контроля сохранены" });
        }

        // POST: api/quality/approve - утвердить/заблокировать партию
        [HttpPost("approve")]
        public async Task<IActionResult> ApproveBatch([FromBody] ApproveQualityRequest request)
        {
            var batch = await _context.Batches.FindAsync(request.BatchId);
            if (batch == null)
                return BadRequest(new { message = "Партия не найдена" });

            var controls = await _context.QualityControls
                .Where(q => q.BatchId == request.BatchId && q.Decision == "pending")
                .ToListAsync();

            // Проверяем наличие не пройденных тестов
            var hasFailed = controls.Any(c => c.Result == "fail");

            if (request.Decision == "approve" && hasFailed)
                return BadRequest(new { message = "Невозможно одобрить партию с не пройденными тестами" });

            foreach (var control in controls)
            {
                control.Decision = request.Decision == "approve" ? "approved" : "blocked";
                control.Comment = request.Comment;
            }

            batch.LabStatus = request.Decision == "approve" ? "approved" : "blocked";
            if (request.Decision == "block")
                batch.Status = "blocked";

            batch.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Создаем уведомление
            var notification = new Notification
            {
                Title = $"Контроль качества партии {batch.BatchNumber}",
                Message = request.Decision == "approve" ? "Партия одобрена" : "Партия заблокирована",
                Type = request.Decision == "approve" ? "success" : "error",
                RelatedEntityId = batch.Id,
                RelatedEntityType = "batch",
                CreatedAt = DateTime.UtcNow
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Партия {(request.Decision == "approve" ? "одобрена" : "заблокирована")}" });
        }

        private string GetStandardValue(string parameterName)
        {
            return parameterName switch
            {
                "concentration" => "≥97%",
                "ph" => "6.5-7.0",
                "moisture" => "≤2.5%",
                _ => "По ТУ"
            };
        }

        private string CheckResult(decimal measuredValue, string standardValue)
        {
            if (standardValue.Contains("≥"))
            {
                var minValue = decimal.Parse(standardValue.Replace("≥", "").Replace("%", ""));
                return measuredValue >= minValue ? "pass" : "fail";
            }
            if (standardValue.Contains("≤"))
            {
                var maxValue = decimal.Parse(standardValue.Replace("≤", "").Replace("%", ""));
                return measuredValue <= maxValue ? "pass" : "fail";
            }
            if (standardValue.Contains("-"))
            {
                var parts = standardValue.Split('-');
                var minValue = decimal.Parse(parts[0]);
                var maxValue = decimal.Parse(parts[1]);
                return measuredValue >= minValue && measuredValue <= maxValue ? "pass" : "fail";
            }
            return "pass";
        }

        private int GetCurrentUserId()
        {
            // Временная заглушка - вернуть 1
            return 1;
        }
    }

    public class CreateQualityControlRequest
    {
        public int BatchId { get; set; }
        public string SampleType { get; set; } = string.Empty;
        public List<QualityParameterDto> Parameters { get; set; } = new();
    }

    public class QualityParameterDto
    {
        public string ParameterName { get; set; } = string.Empty;
        public decimal MeasuredValue { get; set; }
        public string? Unit { get; set; }
        public string? Comment { get; set; }
    }

    public class ApproveQualityRequest
    {
        public int BatchId { get; set; }
        public string Decision { get; set; } = string.Empty; // approve, block
        public string? Comment { get; set; }
    }
}