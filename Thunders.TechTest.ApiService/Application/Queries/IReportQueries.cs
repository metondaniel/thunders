using Dapper;
using System.Data;
using Thunders.TechTest.OutOfBox.Database;

namespace Thunders.TechTest.ApiService.Application.Queries
{
    public interface IReportQueries
    {
        Task<decimal> GetTotalByHourAndCityAsync(string city, DateTime hour);
        Task<IEnumerable<PlazaRevenue>> GetTopPlazasByMonthAsync(int month, int year, int top);
        Task<VehicleTypeCount> GetVehicleTypesByPlazaAsync(string plazaId, DateTime start, DateTime end);
    }

    public record PlazaRevenue(string PlazaId, decimal TotalRevenue);
    public record VehicleTypeCount(int Motorcycles, int Cars, int Trucks);
}
