namespace Thunders.TechTest.ApiService.Application.Messages
{
    public record TollUsageEvent(
    DateTime UsageDateTime,
    string PlazaId,
    string City,
    string State,
    decimal Amount,
    string VehicleType
);

    public record ScheduleReportCommand(
        Guid ReportId,
        string ReportType,
        object Parameters
    );

    public record ReportGeneratedEvent(
        Guid ReportId,
        object Result
    );
}
