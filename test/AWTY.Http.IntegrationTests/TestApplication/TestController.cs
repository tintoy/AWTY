using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.IO;

namespace AWTY.Http.IntegrationTests.TestApplication
{
    /// <summary>
    ///     The controller for common test actions.
    /// </summary>
    [Route("test")]
    public class TestController
        : Controller
    {
        readonly ILogger _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        ///     Return plain-text data.
        /// </summary>
        /// <param name="length">
        ///     The number of unicode characters to return.
        /// </param>
        /// <returns>
        ///     The action result.
        /// </returns>
        [HttpGet, Route("data")]
        public IActionResult GetData(int length = 10)
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

        /// <summary>
        ///     Return plain-text data.
        /// </summary>
        /// <param name="length">
        ///     The number of unicode characters to return.
        /// </param>
        /// <returns>
        ///     The action result.
        /// </returns>
        [HttpPost, Route("post-data")]
        public async Task<IActionResult> PostData()
        {
            _logger.LogInformation("Got request!");

            foreach (var header in Request.Headers)
            {
                _logger.LogInformation("RequestHeader: '{HeaderName}' = '{HeaderValue}'",
                    header.Key,
                    header.Value
                );
            }

            string requestBody = "Nope!";
            try
            {
                using (StreamReader reader = new StreamReader(Request.Body))
                {
                    requestBody = await reader.ReadToEndAsync();
                }
                Response.Headers["Content-Length"] = requestBody.Length.ToString();
            }
            catch (Exception eHandleRequestBody)
            {
                _logger.LogError(new EventId(999, "Damn"), eHandleRequestBody, "Nope, didn't work: {Message}", eHandleRequestBody.Message);
            }

            return Content(requestBody, "text/plain");
        }
    }
}
