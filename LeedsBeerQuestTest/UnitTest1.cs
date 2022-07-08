using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Primitives;
using Moq;
using Microsoft.Extensions.DependencyInjection;

namespace LeedsBeerQuestTest;

[TestClass]
public class VenuesApiTest
{
    public HttpRequest HttpRequestSetup(Dictionary<String, StringValues> query, string body)
    {
        //arrange
        var reqMock = new Mock<HttpRequest>();

        reqMock.Setup(req => req.Query).Returns(new QueryCollection(query));
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(body);
        writer.Flush();
        stream.Position = 0;
        reqMock.Setup(req => req.Body).Returns(stream);
        return reqMock.Object;
    }    
    
    [TestMethod]
    public async Task HttpTrigger_With_Query()
    {
        var query = new Dictionary<String, StringValues>();
        query.TryAdd("name", "Lewis");
        var body = "";

        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();

        var factory = serviceProvider.GetService<ILoggerFactory>();

        var logger = factory.CreateLogger<LeedsBeerQuest.VenuesApi>();

        var result = await LeedsBeerQuest.VenuesApi.Hello(req: HttpRequestSetup(query, body), log: logger);

        var resultObject = (OkObjectResult)result;
        Assert.AreEqual("Hello, Lewis", resultObject.Value);
    }
}