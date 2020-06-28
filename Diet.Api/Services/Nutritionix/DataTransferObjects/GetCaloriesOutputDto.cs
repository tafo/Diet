using System.Collections.Generic;
using Newtonsoft.Json;

namespace Diet.Api.Services.Nutritionix.DataTransferObjects
{
    public class GetCaloriesOutputDto
    {
        public IList<Food> Foods { get; set; } = new List<Food>();
        public class Food
        {
            [JsonProperty("nf_calories")]
            public decimal Calories { get; set; }
        }
    }
}