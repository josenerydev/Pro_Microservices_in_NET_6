using GoogleMapInfo;

using Grpc.Core;

using microservice_map_info.Protos;

using Prometheus;

using System.Diagnostics;

namespace microservice_map_info.Services
{
    public class DistanceInfoService : DistanceInfo.DistanceInfoBase
    {
        private static readonly ActivitySource activitySource = new ActivitySource("microservice_map_info.DistanceInfoService");
        private static readonly Counter googleApiCount = Metrics.CreateCounter("google_api_calls_total", "Number of times Google geolocation api is called.");
        private readonly ILogger<DistanceInfoService> _logger;
        private readonly GoogleDistanceApi _googleDistanceApi;

        public DistanceInfoService(ILogger<DistanceInfoService> logger, GoogleDistanceApi googleDistanceApi)
        {
            _logger = logger;
            _googleDistanceApi = googleDistanceApi;
        }

        public override async Task<DistanceData> GetDistance(Cities cities, ServerCallContext context)
        {
            var totalMIles = "0";
            googleApiCount.Inc();
            var distanceData = await _googleDistanceApi.GetMapDistance(cities.OriginCity, cities.DestinationCity);

            foreach (var distanceDataRow in distanceData.rows)
            {
                foreach (var element in distanceDataRow.elements)
                {
                    totalMIles = element.distance.text;
                }
            }

            return new DistanceData { Miles = totalMIles };
        }
    }
}
