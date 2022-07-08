using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace LeedsBeerQuest
{
    public class VenuesApi
    {

        [FunctionName("HttpTrigger")]
        [OpenApiOperation(operationId: "Hello", tags: new[] { "name" })]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public static async Task<IActionResult> Hello(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Hello")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}";

            return new OkObjectResult(responseMessage);
        }

        [FunctionName("GetVenues")]
        [OpenApiOperation(operationId: "GetVenues", tags: new[] { "name" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public static async Task<IActionResult> GetVenues(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Venues")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
           
            return new OkResult();
        }

        [FunctionName("GetVenuesWithinDistance")]
        [OpenApiOperation(operationId: "GetVenuesWithinDistance", tags: new[] { "name" })]
        [OpenApiParameter(name: "distance", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The distance in meters to search for venues")]
        [OpenApiParameter(name: "position", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The position to search from (lat,long)")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public static async Task<IActionResult> GetVenuesWithinDistance(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetVenuesWithinDistance")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
           
            return new OkResult();
        }

        [FunctionName("GetVenuesWithTag")]
        [OpenApiOperation(operationId: "GetVenuesWithTag", tags: new[] { "name" })]
        [OpenApiParameter(name: "tag", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The tag to search for")]
        [OpenApiParameter(name: "distance", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "The distance in meters to search for venues")]
        [OpenApiParameter(name: "position", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The position to search from (lat,long)")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public static async Task<IActionResult> GetVenuesWithTag(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetVenuesWithTag")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
           
            return new OkResult();
        }
    }
}

