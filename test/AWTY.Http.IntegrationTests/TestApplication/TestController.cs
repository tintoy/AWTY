using Microsoft.AspNetCore.Mvc;
using System;

namespace AWTY.Http.IntegrationTests.TestApplication
{
    /// <summary>
    ///     The controller for common test actions.
    /// </summary>
    [Route("test")]
    public class TestController
        : Controller
    {
        /// <summary>
        ///     Return plain-text data.
        /// </summary>
        /// <param name="length">
        ///     The number of unicode characters to return.
        /// </param>
        /// <returns>
        ///     The action result.
        /// </returns>
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
