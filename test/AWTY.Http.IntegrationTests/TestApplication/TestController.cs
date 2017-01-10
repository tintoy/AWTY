using System;
using Microsoft.AspNetCore.Mvc;

namespace AWTY.Http.IntegrationTests.TestApplication
{
    [Route("test")]
    public class TestController
        : Controller
    {
        [Route("data")]
        public IActionResult Data(int length = 10)
        {
            if (length < 0)
            {
                ModelState.AddModelError(nameof(length),
                    "length cannot be less than 0"
                );

                return BadRequest(ModelState);
            }

            Response.Headers["Content-Length"] = length.ToString();

            return Content(
                new String('X', length)
            );
        }
    }
}