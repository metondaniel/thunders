using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Thunders.TechTest.ApiService.Application.Messages;
using Thunders.TechTest.ApiService.Application.Queries;
using Thunders.TechTest.ApiService.Domain.Enums;
using Thunders.TechTest.ApiService.Infrastructure.Messaging;
using Thunders.TechTest.ApiService.Infrastructure;

namespace Thunders.TechTest.Tests.BehaviorTest
{
    public class GenerateReportMessageHandlerTests
    {
        private readonly AppDbContext _context;
        private readonly Mock<IReportQueries> _mockQueries = new();
        private readonly GenerateReportMessageHandler _handler;

        public GenerateReportMessageHandlerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new AppDbContext(options);
            _handler = new GenerateReportMessageHandler(_mockQueries.Object, _context);
        }

        [Fact]
        public async Task Handle_ValidHourlyCityReport_UpdatesStatusToCompleted()
        {
            // Arrange
            var message = new GenerateReportMessage(
                Guid.NewGuid(),
                "HourlyCity",
                ("São Paulo", DateTime.Now)
            );

            _mockQueries.Setup(q =>
                q.GetTotalByHourAndCityAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(1000m);

            // Act
            await _handler.Handle(message);

            // Assert
            var report = await _context.Reports.FindAsync(message.ReportId);
            Assert.Equal(ReportStatus.Completed, report.Status);
            Assert.NotNull(report.Data);
        }

        [Fact]
        public async Task Handle_InvalidReportType_UpdatesStatusToFailed()
        {
            // Arrange
            var message = new GenerateReportMessage(
                Guid.NewGuid(),
                "InvalidType",
                null
            );

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(message));
            var report = await _context.Reports.FindAsync(message.ReportId);
            Assert.Equal(ReportStatus.Failed, report.Status);
        }
    }
}
