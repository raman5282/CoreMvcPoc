
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CoreMvcPoc.Api
{
    internal class Http403Result : IActionResult
    {
         private readonly IActionResult _result;

        public Http403Result(IActionResult result)
        {
            _result = result;
        }
        public async Task ExecuteResultAsync(ActionContext context)
        { 
            var objectResult = new ObjectResult(_result)
            {
                StatusCode =  (int)HttpStatusCode.Unauthorized
            };

            await objectResult.ExecuteResultAsync(context);
        }
    }
}