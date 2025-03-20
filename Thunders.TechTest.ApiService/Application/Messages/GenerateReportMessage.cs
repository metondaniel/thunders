namespace Thunders.TechTest.ApiService.Application.Messages
{
    public record GenerateReportMessage(
        Guid ReportId,
        string ReportType,
        object Parameters
    );
}
