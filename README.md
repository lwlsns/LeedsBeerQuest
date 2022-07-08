
# Leeds Beer Quest

  

This solution uses Azure Functions to create an API for the Leeds Beer Quest challenge.

The data is stored in Cosmos Db. I've modified the data to include the 'Point' geometry data type, this should make searches based on location more efficient.

Swagger UI: https://leedsbeerfunction.azurewebsites.net/api/swagger/ui

Get all venues: https://leedsbeerfunction.azurewebsites.net/api/Venues

Get venues within a certain distance : https://leedsbeerfunction.azurewebsites.net/api/GetVenuesWithinDistance?distance=200&position=53.795647,-1.5485017

Get venues with a given tag: https://leedsbeerfunction.azurewebsites.net/api/GetVenuesWithTag?tag=food&distance=200&position=53.795647,-1.5485017

With more time I would like to:

 - Configure the project so that it could be run locally by others without the connection string
 - Mock the CosmosClient so that we're able to unit test the happy paths
 - Order the results by distance from the given position. Cosmos Db doesn't allow ordering on a calculate value, so this might need to be done in the API. Depending on the size of data we might need to page the data, starting by searching for venues closer to us, and then gradually expanding the search.
 - Deploy the project as infrastructure as code from the GitHub repo



