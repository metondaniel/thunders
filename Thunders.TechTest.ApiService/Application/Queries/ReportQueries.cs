using System.Data;
using Dapper;
using Thunders.TechTest.ApiService.Domain.Enums;

namespace Thunders.TechTest.ApiService.Application.Queries
{
    public class ReportQueries : IReportQueries
    {
        private readonly IDbConnection _connection;

        public ReportQueries(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<decimal> GetTotalByHourAndCityAsync(string city, DateTime hour)
        {
            const string sql = @"
                SELECT ISNULL(SUM(Amount), 0)
                FROM TollUsages WITH (NOLOCK)
                WHERE City = @city
                AND UsageDateTime >= @startTime
                AND UsageDateTime < @endTime";

            return await _connection.ExecuteScalarAsync<decimal>(sql, new
            {
                city,
                startTime = hour,
                endTime = hour.AddHours(1)
            });
        }

        public async Task<IEnumerable<PlazaRevenue>> GetTopPlazasByMonthAsync(int month, int year, int top)
        {
            const string sql = @"
                SELECT TOP(@top) 
                    PlazaId,
                    SUM(Amount) AS TotalRevenue
                FROM TollUsages WITH (NOLOCK)
                WHERE MONTH(UsageDateTime) = @month
                AND YEAR(UsageDateTime) = @year
                GROUP BY PlazaId
                ORDER BY TotalRevenue DESC";

            return await _connection.QueryAsync<PlazaRevenue>(sql, new
            {
                month,
                year,
                top
            });
        }

        public async Task<VehicleTypeCount> GetVehicleTypesByPlazaAsync(string plazaId, DateTime start, DateTime end)
        {
            const string sql = @"
                SELECT 
                    VehicleType,
                    COUNT(*) AS Count
                FROM TollUsages WITH (NOLOCK)
                WHERE PlazaId = @plazaId
                AND UsageDateTime BETWEEN @start AND @end
                GROUP BY VehicleType";

            var counts = await _connection.QueryAsync<VehicleCount>(sql, new
            {
                plazaId,
                start,
                end
            });

            return new VehicleTypeCount(
                Motorcycles: counts.FirstOrDefault(c => c.VehicleType == VehicleType.Motorcycle)?.Count ?? 0,
                Cars: counts.FirstOrDefault(c => c.VehicleType == VehicleType.Car)?.Count ?? 0,
                Trucks: counts.FirstOrDefault(c => c.VehicleType == VehicleType.Truck)?.Count ?? 0
            );
        }

        private class VehicleCount
        {
            public VehicleType VehicleType { get; set; }
            public int Count { get; set; }
        }
    }
}
