using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Diet.Api.Services.Nutritionix;
using Diet.Api.Services.Nutritionix.DataTransferObjects;
using Flurl.Http;
using Microsoft.Extensions.Options;

namespace Diet.Api.Services
{
    public class CaloriesService : ICaloriesService
    {
        private readonly ServiceConfiguration _serviceConfiguration;

        public CaloriesService(IOptions<ServiceConfiguration> serviceConfiguration)
        {
            _serviceConfiguration = serviceConfiguration.Value;
        }
        
        public async Task<decimal?> GetCaloriesAsync(string foodName)
        {
            var outputDto = await _serviceConfiguration.Endpoint
                .AllowHttpStatus(HttpStatusCode.NotFound)
                .WithHeaders(_serviceConfiguration.GetRequestHeader())
                .PostJsonAsync(new {query = foodName})
                .ReceiveJson<GetCaloriesOutputDto>();

            var foodInformation = outputDto.Foods.FirstOrDefault();

            return foodInformation?.Calories;
        }
    }
}