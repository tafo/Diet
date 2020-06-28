using System.Collections.Generic;

namespace Diet.Api.Services.Nutritionix
{
    public class ServiceConfiguration
    {
        public string Endpoint { get; set; }
        public string AppId { get; set; }
        public string AppKey { get; set; }

        public Dictionary<string, string> GetRequestHeader()
        {
            return new Dictionary<string, string> {{"x-app-id", AppId}, {"x-app-key", AppKey}};
        }
    }
}