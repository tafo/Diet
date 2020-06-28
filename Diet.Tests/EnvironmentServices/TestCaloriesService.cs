using System.Collections.Generic;
using System.Threading.Tasks;
using Diet.Api.Services;

namespace Diet.Tests.EnvironmentServices
{
    public class TestCaloriesService : ICaloriesService
    {
        public Dictionary<string, decimal?> Calories = new Dictionary<string, decimal?>();

        public async Task<decimal?> GetCaloriesAsync(string foodName)
        {
            return await Task.FromResult(Calories.GetValueOrDefault(foodName, null));
        }
    }
}