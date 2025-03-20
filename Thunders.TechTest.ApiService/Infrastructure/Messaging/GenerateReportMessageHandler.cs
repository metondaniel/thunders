using System.Text.Json;
using Rebus.Handlers;
using Thunders.TechTest.ApiService.Application.Messages;
using Thunders.TechTest.ApiService.Application.Queries;
using Thunders.TechTest.ApiService.Domain.Aggregates;
using Thunders.TechTest.ApiService.Domain.Enums;

namespace Thunders.TechTest.ApiService.Infrastructure.Messaging
{
    public class GenerateReportMessageHandler : IHandleMessages<GenerateReportMessage>
    {
        private readonly IReportQueries _queries;
        private readonly AppDbContext _context;

        public GenerateReportMessageHandler(IReportQueries queries, AppDbContext context)
        {
            _queries = queries;
            _context = context;
        }

        public async Task Handle(GenerateReportMessage message)
        {
            var report = new Report
            {
                Id = message.ReportId,
                ReportType = message.ReportType,
                Status = ReportStatus.Pending,
                GeneratedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Data = ""
            };

            await _context.Reports.AddAsync(report);
            await _context.SaveChangesAsync();

            try
            {
                var result = message.ReportType switch
                {
                    "HourlyCity" => await ProcessHourlyCityReport(message.Parameters),
                    "TopPlazas" => await ProcessTopPlazasReport(message.Parameters),
                    "VehicleTypes" => await ProcessVehicleTypesReport(message.Parameters),
                    _ => throw new ArgumentException("Invalid report type")
                };

                await SaveReportResults(message.ReportId, result);
            }
            catch (Exception ex)
            {
                await HandleError(message.ReportId, ex);
                throw;
            }
        }
        private async Task<object> ProcessTopPlazasReport(object parameters)
        {
            var (month, year, top) = (ValueTuple<int, int, int>)parameters;
            return await _queries.GetTopPlazasByMonthAsync(month, year, top);
        }

        private async Task<object> ProcessVehicleTypesReport(object parameters)
        {
            var (plazaId, start, end) = (ValueTuple<string, DateTime, DateTime>)parameters;
            return await _queries.GetVehicleTypesByPlazaAsync(plazaId, start, end);
        }
        private async Task SaveReportResults(Guid reportId, object result)
        {
            var report = await _context.Reports.FindAsync(reportId);
            if (report == null) throw new KeyNotFoundException("Report not found");

            report.Data = JsonSerializer.Serialize(result);
            report.Status = ReportStatus.Completed;
            await _context.SaveChangesAsync();
        }
        private async Task HandleError(Guid reportId, Exception ex)
        {
            var report = await _context.Reports.FindAsync(reportId);
            if (report == null) return;

            report.Data = $"Error: {ex.Message}";
            report.Status = ReportStatus.Failed;
            await _context.SaveChangesAsync();
        }
        private async Task<object> ProcessHourlyCityReport(object parameters)
        {
            var (city, hour) = (ValueTuple<string, DateTime>)parameters;
            return await _queries.GetTotalByHourAndCityAsync(city, hour);
        }
    }
}
