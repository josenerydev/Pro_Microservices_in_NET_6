namespace microservice_map_info.Services
{
    public class QuoteSvc
    {
        private readonly IDistanceInfoSvc _distanceInfoSvc;

        public QuoteSvc(IDistanceInfoSvc distanceInfoSvc)
        {
            _distanceInfoSvc = distanceInfoSvc;
        }

        public async Task<Quote> CreateQuote(string originCity, string destinationCity)
        {
            var distanceInfo = await _distanceInfoSvc.GetDistanceAsync(originCity, destinationCity);

            var quote = new Quote
            {
                Id = 123,
                ExpectedDistance = distanceInfo.Item1,
                ExpectedDistanceType = distanceInfo.Item2
            };

            return quote;
        }
    }

    public class Quote
    {
        public long Id { get; set; }
        public int ExpectedDistance { get; set; }
        public string ExpectedDistanceType { get; set; }
    }
}
