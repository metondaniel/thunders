using Thunders.TechTest.ApiService.Domain.Enums;

namespace Thunders.TechTest.Domain
{
    public class TollUsage
    {
        public Guid Id { get; set; }
        public DateTime UsageDateTime { get; set; }
        public string PlazaId { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public decimal Amount { get; set; }
        public VehicleType VehicleType { get; set; }
    }
}