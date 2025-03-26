using System;
using System.Collections.Generic;
using GenAIED_Sandesh.Controllers;
using GenAIED_Sandesh.Interfaces;
using GenAIED_Sandesh.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace GenAIED_Sandesh.Tests
{
    public class EmailClassifierControllerTests
    {
        private readonly Mock<ILogger<EmailClassifierController>> _mockLogger;
        private readonly Mock<IModelTrainer> _mockModelTrainer;
        private readonly Mock<IEmailExtractorService> _mockEmailExtractor;
        private readonly Mock<IOptions<List<InputData>>> _mockAppSettingsService;
        private readonly Mock<IWebHostEnvironment> _mockEnv;
        private readonly EmailClassifierController _controller;

        public EmailClassifierControllerTests()
        {
            _mockLogger = new Mock<ILogger<EmailClassifierController>>();
            _mockModelTrainer = new Mock<IModelTrainer>();
            _mockEmailExtractor = new Mock<IEmailExtractorService>();
            _mockAppSettingsService = new Mock<IOptions<List<InputData>>>();
            _mockEnv = new Mock<IWebHostEnvironment>();

            _mockAppSettingsService.Setup(a => a.Value).Returns(new List<InputData>());

            _controller = new EmailClassifierController(
                _mockLogger.Object,
                _mockModelTrainer.Object,
                _mockEmailExtractor.Object,
                _mockAppSettingsService.Object,
                _mockEnv.Object);
        }

        [Fact]
        public void CreateModelsAndSave_CallsModelTrainer_ReturnsTrue()
        {
            // Act
            var result = _controller.CreateModelsAndSave();

            // Assert
            _mockModelTrainer.Verify(m => m.CreateModelsAndSave(It.IsAny<List<InputData>>()), Times.Once);
            Assert.True(result);
        }

        [Fact]
        public void CreateModelsAndSave_WithEmptyTrainingData_DoesNotThrowException()
        {
            // Arrange
            _mockAppSettingsService.Setup(a => a.Value).Returns(new List<InputData>());

            // Act
            var result = _controller.CreateModelsAndSave();

            // Assert
            _mockModelTrainer.Verify(m => m.CreateModelsAndSave(It.IsAny<List<InputData>>()), Times.Once);
            Assert.True(result);
        }

        [Fact]
        public void PredictData_ReturnsValidPredictions()
        {
            // Arrange
            var mockPredictions = new List<PredictionOutput>
            {
                new PredictionOutput { PredictedRequestType = "Support", PredictedRequestTypeConfidenceScore = "0.95" },
                new PredictionOutput { PredictedRequestType = "Billing", PredictedRequestTypeConfidenceScore = "0.85" }
            };

            _mockModelTrainer.Setup(m => m.PredictData()).Returns(mockPredictions);

            // Act
            var result = _controller.PredictData();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Support", result[0].PredictedRequestType);
            Assert.Equal("0.95", result[0].PredictedRequestTypeConfidenceScore);
            Assert.Equal("Billing", result[1].PredictedRequestType);
            Assert.Equal("0.85", result[1].PredictedRequestTypeConfidenceScore);
        }

        [Fact]
        public void PredictData_WithNoPredictions_ReturnsEmptyList()
        {
            // Arrange
            _mockModelTrainer.Setup(m => m.PredictData()).Returns(new List<PredictionOutput>());

            // Act
            var result = _controller.PredictData();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void PredictData_WithException_ReturnsEmptyListAndLogsError()
        {
            // Arrange
            _mockModelTrainer.Setup(m => m.PredictData()).Throws(new Exception("Model error"));

            // Act
            var result = _controller.PredictData();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            // Verify that an error was logged
            _mockLogger.Verify(
                log => log.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("An error occurred while predicting data.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

        }
    }
}
