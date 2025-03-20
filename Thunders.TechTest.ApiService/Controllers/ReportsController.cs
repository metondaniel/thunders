using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Thunders.TechTest.ApiService.Application.Messages;
using Thunders.TechTest.ApiService.Domain.Aggregates;
using Thunders.TechTest.ApiService.Domain.Enums;
using Thunders.TechTest.ApiService.Infrastructure;
using Thunders.TechTest.OutOfBox.Queues;

namespace Thunders.TechTest.WebAPI.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly IMessageSender _messageSender;
        private readonly AppDbContext _context;

        public ReportsController(
            IMessageSender messageSender,
            AppDbContext context)
        {
            _messageSender = messageSender;
            _context = context;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateReport([FromBody] ReportRequest request)
        {
            var reportId = Guid.NewGuid();

            await _messageSender.SendLocal(new GenerateReportMessage(
                reportId,
                request.ReportType,
                request.Parameters
            ));

            return Accepted(new
            {
                ReportId = reportId,
                StatusEndpoint = Url.Action("GetReportStatus", new { id = reportId })
            });
        }

        [HttpGet("{id}/status")]
        public async Task<IActionResult> GetReportStatus(Guid id)
        {
            var report = await _context.Reports.FindAsync(id);

            if (report == null)
                return NotFound();

            return report.Status switch
            {
                ReportStatus.Completed => Ok(new
                {
                    Status = "Completed",
                    Data = DeserializeReportData(report),
                    GeneratedAt = report.GeneratedAt
                }),
                ReportStatus.Failed => Ok(new
                {
                    Status = "Failed",
                    Error = report.Data,
                    FailedAt = report.GeneratedAt
                }),
                _ => Accepted(new
                {
                    Status = "Processing",
                    CreatedAt = report.CreatedAt
                })
            };
        }

        private object DeserializeReportData(Report report)
        {
            return report.ReportType switch
            {
                "HourlyCity" => JsonSerializer.Deserialize<decimal>(report.Data),
                "TopPlazas" => JsonSerializer.Deserialize<List<PlazaRevenue>>(report.Data),
                "VehicleTypes" => JsonSerializer.Deserialize<VehicleTypeCount>(report.Data),
                _ => report.Data
            };
        }
    }

    public record ReportRequest(
        string ReportType,
        object Parameters
    );

    public record PlazaRevenue(string PlazaId, decimal TotalRevenue);
    public record VehicleTypeCount(int Motorcycles, int Cars, int Trucks);
}