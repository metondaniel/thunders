using Thunders.TechTest.ApiService.Domain.Enums;

namespace Thunders.TechTest.ApiService.Domain.Aggregates
{
    public class Report
    {
        public Guid Id { get; set; }
        public string ReportType { get; set; }
        public string Data { get; set; }
        public ReportStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime GeneratedAt { get; set; }
    }
}
