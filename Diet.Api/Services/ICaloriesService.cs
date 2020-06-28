using System.Threading.Tasks;

namespace Diet.Api.Services
{
    public interface ICaloriesService
    {
        public Task<decimal?> GetCaloriesAsync(string foodName);
    }
}