using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace PriceGeneratorFunction;

public static class CalculateRevenueAfterFees
{
    [FunctionName("CalculateRevenueAfterFees")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "CalculateRevenueAfterFees")] HttpRequest req, ILogger log)
    {
        decimal price;
        decimal percentageFees;
        decimal fixedFees;
        int decimals;
        
        try
        {
            price = decimal.Parse(req.Query["price"]);
            percentageFees = decimal.Parse(req.Query["percentage_fees"]);
            fixedFees = decimal.Parse(req.Query["fixed_fees"]); 
        }
        catch
        {
            return new BadRequestObjectResult("Please pass the specified parameters on the query string.");
        }
        
        
        try
        {
            decimals = int.Parse(req.Query["decimal_places"]);
        }
        catch
        {
            decimals = int.Parse(decimal.Parse("2").ToString());
        }
        
        //   string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
     
        // ReSharper disable once CommentTypo
        //Use BIDMAS as order of operations
        var result = price * (decimal.One - (percentageFees / 100));

        result = result - fixedFees;
        
        return (ActionResult)new OkObjectResult( Math.Round(result, decimals, MidpointRounding.AwayFromZero));
    }
}