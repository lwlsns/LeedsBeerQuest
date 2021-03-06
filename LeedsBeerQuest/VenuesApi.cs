using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace LeedsBeerQuest
{
    public class VenuesApi
    {

        [FunctionName("HttpTrigger")]
        [OpenApiOperation(operationId: "Hello", tags: new[] { "name" })]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "A hello message")]
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
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<Venue>), Description = "A list of all venues")]
        public static async Task<IActionResult> GetVenues(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Venues")] HttpRequest req,
            [CosmosDB(
                databaseName: "venues",
                containerName: "venuecontainer",
                Connection  = "CosmosDBConnectionString")]
                CosmosClient  client,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function GetVenues processed a request.");
            
            Container container = client.GetDatabase("venues").GetContainer("venuecontainer");

            QueryDefinition queryDefinition = new QueryDefinition(
                @"SELECT *
                FROM c");

            List<Venue> lstVenues = new List<Venue>();

            using (FeedIterator<Venue> resultSet = container.GetItemQueryIterator<Venue>(queryDefinition))
            {
                while (resultSet.HasMoreResults)
                {
                    FeedResponse<Venue> response = await resultSet.ReadNextAsync();
                    lstVenues.AddRange(response.Resource);
                }
            }

            return new JsonResult(lstVenues);
        }

        [FunctionName("GetVenuesWithinDistance")]
        [OpenApiOperation(operationId: "GetVenuesWithinDistance", tags: new[] { "name" })]
        [OpenApiParameter(name: "distance", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "The distance in meters to search for venues")]
        [OpenApiParameter(name: "position", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The position to search from (lat,long)")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<Venue>), Description = "A list of venues within a given distance")]
        public static async Task<IActionResult> GetVenuesWithinDistance(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetVenuesWithinDistance")] HttpRequest req,
            [CosmosDB(
                databaseName: "venues",
                containerName: "venuecontainer",
                Connection  = "CosmosDBConnectionString")]
                CosmosClient  client,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function GetVenuesWithinDistance processed a request.");

            string position = req.Query["position"];
            string distance = req.Query["distance"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            position = position ?? data?.position;
            distance = distance ?? data?.distance;

            if(string.IsNullOrEmpty(position))
            {
                return new BadRequestResult();
            }

            string query = @"SELECT ST_DISTANCE(c.location, {'type': 'Point', 'coordinates':[@lat,@lng]}) AS Distance, 
                c.id, c.name, c.category, c.url, c.date, c.excerpt, c.thumbnail, c.location, c.address, c.phone, c.twitter, 
                c.stars_beer, c.stars_atmosphere, c.stars_amenities, c.stars_value, c.tags 
                FROM c";

            if(!string.IsNullOrEmpty(distance))
            {
                query += " WHERE ST_DISTANCE(c.location, {'type': 'Point', 'coordinates':[@lat,@lng]}) < @distance";
            }

            Container container = client.GetDatabase("venues").GetContainer("venuecontainer");

            string[] parts = position.Split(',');


            QueryDefinition queryDefinition = new QueryDefinition(query)
                .WithParameter("@lat", double.Parse(parts[0].Trim()))
                .WithParameter("@lng", double.Parse(parts[1].Trim()))
                .WithParameter("@distance", int.Parse(distance));
                

            List<Venue> lstVenues = new List<Venue>();

            using (FeedIterator<Venue> resultSet = container.GetItemQueryIterator<Venue>(queryDefinition))
            {
                while (resultSet.HasMoreResults)
                {
                    FeedResponse<Venue> response = await resultSet.ReadNextAsync();
                    lstVenues.AddRange(response.Resource);
                }
            }

            return new JsonResult(lstVenues);
        }

        [FunctionName("GetVenuesWithTag")]
        [OpenApiOperation(operationId: "GetVenuesWithTag", tags: new[] { "name" })]
        [OpenApiParameter(name: "tag", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The tag to search for")]
        [OpenApiParameter(name: "distance", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "The distance in meters to search for venues")]
        [OpenApiParameter(name: "position", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The position to search from (lat,long)")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<Venue>), Description = "A list of venues with the given tag")]
        public static async Task<IActionResult> GetVenuesWithTag(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetVenuesWithTag")] HttpRequest req,
            [CosmosDB(
                databaseName: "venues",
                containerName: "venuecontainer",
                Connection  = "CosmosDBConnectionString")]
                CosmosClient  client,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function GetVenuesWithTag processed a request.");
           
            string position = req.Query["position"];
            string distance = req.Query["distance"];
            string tag = req.Query["tag"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            position = position ?? data?.position;
            distance = distance ?? data?.distance;
            tag = tag ?? data?.tag;

            if(string.IsNullOrEmpty(position) || string.IsNullOrEmpty(tag))
            {
                return new BadRequestResult();
            }

            string query = @"SELECT ST_DISTANCE(c.location, {'type': 'Point', 'coordinates':[@lat,@lng]}) AS Distance, 
                c.id, c.name, c.category, c.url, c.date, c.excerpt, c.thumbnail, c.location, c.address, c.phone, c.twitter, 
                c.stars_beer, c.stars_atmosphere, c.stars_amenities, c.stars_value, c.tags 
                FROM c
                WHERE CONTAINS(c.tags, @tag, true)";

            if(!string.IsNullOrEmpty(distance))
            {
                query += " AND ST_DISTANCE(c.location, {'type': 'Point', 'coordinates':[@lat,@lng]}) < @distance";
            }

            Container container = client.GetDatabase("venues").GetContainer("venuecontainer");

            string[] parts = position.Split(',');

            QueryDefinition queryDefinition = new QueryDefinition(query)
                .WithParameter("@lat", double.Parse(parts[0].Trim()))
                .WithParameter("@lng", double.Parse(parts[1].Trim()))
                .WithParameter("@distance", int.Parse(distance))
                .WithParameter("@tag", tag);
            List<Venue> lstVenues = new List<Venue>();

            using (FeedIterator<Venue> resultSet = container.GetItemQueryIterator<Venue>(queryDefinition))
            {
                while (resultSet.HasMoreResults)
                {
                    FeedResponse<Venue> response = await resultSet.ReadNextAsync();
                    lstVenues.AddRange(response.Resource);
                }
            }

            return new JsonResult(lstVenues);
        }
    }
}

