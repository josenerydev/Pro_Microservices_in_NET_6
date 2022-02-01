using PactNet;
using PactNet.Infrastructure.Outputters;

using System;
using System.Collections.Generic;

using Xunit;
using Xunit.Abstractions;

namespace ProviderTests
{
    public class DiscountServiceTests : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private bool _disposed = false;
        private readonly string _serviceUri;

        public DiscountServiceTests(ITestOutputHelper output)
        {
            _output = output;
            _serviceUri = "http://localhost:5095";
        }

        [Fact]
        public void PactWithOrderSvcShouldBeVerified()
        {
            var config = new PactVerifierConfig
            {
                Verbose = true,
                ProviderVersion = "2.0.0",
                CustomHeaders = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json; charset=utf-8"}
                },
                Outputters = new List<IOutput>
                {
                    new XUnitOutput(_output)
                }
            };

            new PactVerifier(config)
                .ServiceProvider("Discounts", _serviceUri)
                .HonoursPactWith("Orders")
                .PactUri(@"c:\temp\pact\OrderSvcConsumer\orders-discounts.json")
                .Verify();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    //
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}