using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using TestJobService;

namespace Tests
{
    [TestClass]
    public sealed class TestHttp
    {
        string _hostName = "testHostName";
        string _apiUri = "https://test.com/sysInfo";

        [TestMethod]
        public async Task TestPost()
        {
            HttpRequestMessage? request = null;

            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, _) =>
                {
                    request = req;
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            var httpClient = new HttpClient(handlerMock.Object);
            var options = new Mock<IOptionsMonitor<Settings>>();
            var settings = new Settings() { ApiUri = _apiUri };
            options.Setup(x => x.CurrentValue).Returns(settings);
            var logger = new Mock<ILogger<Worker>>();

            var service = new Worker(logger.Object, options.Object, httpClient);

            var infoModel = new InfoModel() { HostName = _hostName };
            var info = JsonSerializer.Serialize(infoModel);

            await service.SendMessageAsync(info, CancellationToken.None);

            var requestString = await request?.Content?.ReadAsStringAsync();
            var sentInfo = JsonSerializer.Deserialize<InfoModel>(requestString);

            Assert.AreEqual(HttpMethod.Post, request.Method);
            Assert.AreEqual(_apiUri, request.RequestUri.AbsoluteUri);
            Assert.AreEqual(_hostName, sentInfo.HostName);
        }
    }
}
