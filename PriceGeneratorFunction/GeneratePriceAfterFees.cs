using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace PriceGeneratorFunction;

public static class GeneratePriceAfterFees
{
    [FunctionName("GeneratePriceAfterFees")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GeneratePriceAfterFees")] HttpRequest req, ILogger log)
    {
        decimal goalPrice;
        decimal percentageFees;
        decimal fixedFees;
        int decimals;

        int decimalPlacesToUse;
        
        try
        {
            goalPrice = decimal.Parse(req.Query["goal_price"]);
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

        var result = decimal.Add(goalPrice, fixedFees) / (decimal.One - (percentageFees / 100));
        
        return (ActionResult)new OkObjectResult(Math.Round(result, decimals, MidpointRounding.AwayFromZero));
        
    }
}