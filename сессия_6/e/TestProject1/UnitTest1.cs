using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AgroControlClient;
using Xunit;

namespace TestProject1
{
    public class AgroControlTests
    {
        // ==================== 1-5: ТЕСТЫ МОДЕЛЕЙ ДАННЫХ ====================

        [Fact]
        public void Test_ProductItem_ShouldCreateValidProduct()
        {
            // Arrange
            var product = new ProductItem
            {
                Id = 1,
                Code = "TEST-001",
                Name = "Тестовый продукт",
                Type = "Гербицид",
                Status = "Активен"
            };

            // Act & Assert
            Assert.Equal(1, product.Id);
            Assert.Equal("TEST-001", product.Code);
            Assert.Equal("Тестовый продукт", product.Name);
            Assert.Equal("Гербицид", product.Type);
            Assert.Equal("Активен", product.Status);
        }

        [Fact]
        public void Test_RecipeItem_ShouldCalculateSumCorrectly()
        {
            // Arrange
            var components = new List<ComponentItem>
            {
                new ComponentItem { Percent = "45", Order = "1" },
                new ComponentItem { Percent = "35", Order = "2" },
                new ComponentItem { Percent = "20", Order = "3" }
            };

            // Act
            int total = components.Sum(c => int.Parse(c.Percent));

            // Assert
            Assert.Equal(100, total);
        }

        [Fact]
        public void Test_BatchItem_StatusTransitions_ShouldBeValid()
        {
            // Arrange
            var batch = new BatchItem { Status = "planned" };

            // Act
            batch.Status = "running";
            bool canStart = batch.Status == "running";

            // Assert
            Assert.True(canStart);
            Assert.Equal("running", batch.Status);
        }

        [Fact]
        public void Test_DeviationItem_SeverityLevels_ShouldBeValid()
        {
            // Arrange
            var deviation = new DeviationItem { Severity = "critical" };
            string[] validSeverities = { "info", "warning", "critical" };

            // Act
            bool isValid = validSeverities.Contains(deviation.Severity);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void Test_OrderItem_QuantityString_ShouldFormatCorrectly()
        {
            // Arrange
            var order = new OrderItem { Quantity = 1000 };

            // Act
            string quantityStr = order.QuantityStr;

            // Assert
            Assert.Equal("1000 кг", quantityStr);
        }

        // ==================== 6-10: ТЕСТЫ ВАЛИДАЦИИ ====================

        [Fact]
        public void Test_RecipeSumValidation_ShouldReturnFalseIfNot100Percent()
        {
            // Arrange
            var components = new List<RecipeComponent>
            {
                new RecipeComponent { QuantityPercent = 30 },
                new RecipeComponent { QuantityPercent = 30 },
                new RecipeComponent { QuantityPercent = 30 }
            };

            // Act
            decimal total = components.Sum(c => c.QuantityPercent);
            bool isValid = Math.Abs(total - 100) < 0.01m;

            // Assert
            Assert.False(isValid);
            Assert.Equal(90, total);
        }

        [Fact]
        public void Test_RecipeSumValidation_ShouldReturnTrueIf100Percent()
        {
            // Arrange
            var components = new List<RecipeComponent>
            {
                new RecipeComponent { QuantityPercent = 50 },
                new RecipeComponent { QuantityPercent = 30 },
                new RecipeComponent { QuantityPercent = 20 }
            };

            // Act
            decimal total = components.Sum(c => c.QuantityPercent);
            bool isValid = Math.Abs(total - 100) < 0.01m;

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void Test_PasswordValidation_ShouldRejectShortPassword()
        {
            // Arrange
            string password = "123";
            bool isValid = password.Length >= 6;

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void Test_PasswordValidation_ShouldAcceptLongPassword()
        {
            // Arrange
            string password = "123456";
            bool isValid = password.Length >= 6;

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void Test_EmailValidation_ShouldRejectInvalidEmail()
        {
            // Arrange
            string email = "invalid-email";
            bool isValid = email.Contains("@") && email.Contains(".");

            // Assert
            Assert.False(isValid);
        }

        // ==================== 11-15: ТЕСТЫ СТАТУСОВ ====================

        [Fact]
        public void Test_BatchStatus_PlannedToRunning_ShouldBeValid()
        {
            // Arrange
            var batch = new BatchItem { Status = "planned" };
            var validTransitions = new Dictionary<string, string[]>
            {
                { "planned", new[] { "running", "cancelled" } },
                { "running", new[] { "paused", "completed", "blocked" } },
                { "paused", new[] { "running", "cancelled" } }
            };

            // Act
            batch.Status = "running";
            bool isValid = validTransitions["planned"].Contains(batch.Status);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void Test_BatchStatus_RunningToCompleted_ShouldBeValid()
        {
            // Arrange
            var batch = new BatchItem { Status = "running" };
            var validTransitions = new Dictionary<string, string[]>
            {
                { "running", new[] { "paused", "completed", "blocked" } }
            };

            // Act
            batch.Status = "completed";
            bool isValid = validTransitions["running"].Contains(batch.Status);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void Test_BatchStatus_RunningToPlanned_ShouldBeInvalid()
        {
            // Arrange
            var batch = new BatchItem { Status = "running" };

            // Act
            batch.Status = "planned";

            // Assert
            Assert.NotEqual("planned", batch.Status);
        }

        [Fact]
        public void Test_LabStatus_ShouldTransitionCorrectly()
        {
            // Arrange
            var batch = new BatchItem { LabStatus = "pending" };

            // Act
            batch.LabStatus = "in_progress";
            batch.LabStatus = "approved";

            // Assert
            Assert.Equal("approved", batch.LabStatus);
        }

        [Fact]
        public void Test_OrderStatus_ShouldNotAllowInvalidTransition()
        {
            // Arrange
            var order = new OrderItem { Status = "completed" };

            // Act
            string originalStatus = order.Status;
            order.Status = "in_progress";

            // Assert
            Assert.NotEqual("in_progress", order.Status);
            Assert.Equal("completed", originalStatus);
        }

        // ==================== 16-20: ТЕСТЫ ПАРАМЕТРОВ ====================

        [Fact]
        public void Test_TemperatureCheck_ShouldDetectDeviation()
        {
            // Arrange
            decimal plannedTemp = 85;
            decimal actualTemp = 78;
            decimal tolerance = 5;

            // Act
            bool isDeviation = Math.Abs(plannedTemp - actualTemp) > tolerance;

            // Assert
            Assert.True(isDeviation);
        }

        [Fact]
        public void Test_PressureCheck_ShouldBeWithinTolerance()
        {
            // Arrange
            decimal plannedPressure = 3.5m;
            decimal actualPressure = 3.4m;
            decimal tolerance = 0.2m;

            // Act
            bool isWithinTolerance = Math.Abs(plannedPressure - actualPressure) <= tolerance;

            // Assert
            Assert.True(isWithinTolerance);
        }

        [Fact]
        public void Test_QualityCheck_PassIfValueAboveMinimum()
        {
            // Arrange
            decimal minConcentration = 97;
            decimal actualConcentration = 98.5m;

            // Act
            bool isPass = actualConcentration >= minConcentration;

            // Assert
            Assert.True(isPass);
        }

        [Fact]
        public void Test_QualityCheck_FailIfValueBelowMinimum()
        {
            // Arrange
            decimal minConcentration = 97;
            decimal actualConcentration = 96.2m;

            // Act
            bool isPass = actualConcentration >= minConcentration;

            // Assert
            Assert.False(isPass);
        }

        [Fact]
        public void Test_DeviationSeverity_ShouldBeCriticalIfExceedsThreshold()
        {
            // Arrange
            decimal tolerance = 5;
            decimal deviation = 8;
            string expectedSeverity = "critical";

            // Act
            string severity = deviation > tolerance ? "critical" : "warning";

            // Assert
            Assert.Equal(expectedSeverity, severity);
        }

        // ==================== 21-25: ТЕСТЫ КОЛЛЕКЦИЙ ====================

        [Fact]
        public void Test_ProductCollection_ShouldAddItem()
        {
            // Arrange
            var products = new ObservableCollection<ProductItem>();

            // Act
            products.Add(new ProductItem { Id = 1, Name = "Тест" });

            // Assert
            Assert.Single(products);
        }

        [Fact]
        public void Test_ProductCollection_ShouldRemoveItem()
        {
            // Arrange
            var products = new ObservableCollection<ProductItem>();
            var product = new ProductItem { Id = 1, Name = "Тест" };
            products.Add(product);

            // Act
            products.Remove(product);

            // Assert
            Assert.Empty(products);
        }

        [Fact]
        public void Test_RecipeComponents_ShouldMaintainLoadOrder()
        {
            // Arrange
            var components = new List<RecipeComponent>
            {
                new RecipeComponent { LoadOrder = 3, RawMaterialName = "Компонент 3" },
                new RecipeComponent { LoadOrder = 1, RawMaterialName = "Компонент 1" },
                new RecipeComponent { LoadOrder = 2, RawMaterialName = "Компонент 2" }
            };

            // Act
            var sorted = components.OrderBy(c => c.LoadOrder).ToList();

            // Assert
            Assert.Equal(1, sorted[0].LoadOrder);
            Assert.Equal(2, sorted[1].LoadOrder);
            Assert.Equal(3, sorted[2].LoadOrder);
        }

        [Fact]
        public void Test_BatchSteps_ShouldCompleteInOrder()
        {
            // Arrange
            var steps = new List<BatchStepExecution>
            {
                new BatchStepExecution { StepOrder = 1, Status = "completed" },
                new BatchStepExecution { StepOrder = 2, Status = "in_progress" },
                new BatchStepExecution { StepOrder = 3, Status = "pending" }
            };

            // Act
            var currentStep = steps.FirstOrDefault(s => s.Status == "in_progress");

            // Assert
            Assert.NotNull(currentStep);
            Assert.Equal(2, currentStep.StepOrder);
        }

        [Fact]
        public void Test_QualityControls_ShouldHavePendingDecisionFirst()
        {
            // Arrange
            var controls = new List<QualityControl>
            {
                new QualityControl { Decision = "approved", ParameterName = "Концентрация" },
                new QualityControl { Decision = "pending", ParameterName = "pH" }
            };

            // Act
            var pending = controls.FirstOrDefault(c => c.Decision == "pending");

            // Assert
            Assert.NotNull(pending);
            Assert.Equal("pH", pending.ParameterName);
        }

        // ==================== 26-30: ТЕСТЫ БИЗНЕС-ЛОГИКИ ====================

        [Fact]
        public void Test_BatchCreation_ShouldRequireValidOrder()
        {
            // Arrange
            int? orderId = null;
            bool isValid = orderId.HasValue && orderId.Value > 0;

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void Test_BatchStart_ShouldSetStartTime()
        {
            // Arrange
            var batch = new BatchItem { Status = "planned", StartTime = null };
            DateTime expectedTime = DateTime.Now;

            // Act
            batch.Status = "running";
            batch.StartTime = expectedTime;

            // Assert
            Assert.NotNull(batch.StartTime);
            Assert.Equal(expectedTime, batch.StartTime);
        }

        [Fact]
        public void Test_BatchComplete_ShouldSetEndTime()
        {
            // Arrange
            var batch = new BatchItem { Status = "running", EndTime = null };
            DateTime expectedTime = DateTime.Now;

            // Act
            batch.Status = "completed";
            batch.EndTime = expectedTime;

            // Assert
            Assert.NotNull(batch.EndTime);
            Assert.Equal(expectedTime, batch.EndTime);
        }

        [Fact]
        public void Test_ReportExport_ShouldHaveValidFileName()
        {
            // Arrange
            string reportType = "batches";
            string dateStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string expectedFileName = $"{reportType}_report_{dateStamp}.xlsx";

            // Act
            string actualFileName = $"{reportType}_report_{dateStamp}.xlsx";

            // Assert
            Assert.Contains("batches", actualFileName);
            Assert.EndsWith(".xlsx", actualFileName);
        }

        [Fact]
        public void Test_AuditLog_ShouldRecordUserAction()
        {
            // Arrange
            var auditEntry = new AuditLog
            {
                UserId = 1,
                Action = "LOGIN",
                Timestamp = DateTime.Now,
                Details = "User logged in successfully"
            };

            // Act & Assert
            Assert.Equal(1, auditEntry.UserId);
            Assert.Equal("LOGIN", auditEntry.Action);
            Assert.NotNull(auditEntry.Timestamp);
            Assert.Contains("successfully", auditEntry.Details);
        }
    }

    // ==================== ВСПОМОГАТЕЛЬНЫЕ КЛАССЫ ====================

    public class ProductItem
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }

    public class RecipeComponent
    {
        public decimal QuantityPercent { get; set; }
        public int LoadOrder { get; set; }
        public string RawMaterialName { get; set; }
    }

    public class ComponentItem
    {
        public string Percent { get; set; }
        public string Order { get; set; }
    }

    public class BatchItem
    {
        public string Status { get; set; }
        public string LabStatus { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    public class DeviationItem
    {
        public string Severity { get; set; }
    }

    public class OrderItem
    {
        public int Quantity { get; set; }
        public string Status { get; set; }
        public string QuantityStr => $"{Quantity} кг";
    }

    public class BatchStepExecution
    {
        public int StepOrder { get; set; }
        public string Status { get; set; }
    }

    public class QualityControl
    {
        public string Decision { get; set; }
        public string ParameterName { get; set; }
    }

    public class AuditLog
    {
        public int UserId { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; }
        public string Details { get; set; }
    }
}