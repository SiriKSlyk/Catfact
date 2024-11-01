using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Catfactsiri1
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("Function1")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            string json = XmlHelper.SendRequest();
            string xml = XmlHelper.ConvertJsonToXml(json).OuterXml;

            const string storageAccountName = "catfactsiri";
            const string containerName = "catfact";

            string blobName = XmlHelper.GenerateFileNameWithTimeStamp();
            bool status = XmlHelper.WriteToBlobStorage(storageAccountName, containerName, blobName, xml).Result;

            if (status)
            {
                _logger.LogInformation($"Blob written with filename: {blobName}");
            }
            else
            {
                _logger.LogInformation($"Failed to write {blobName} to storage container");
            }

            return new OkObjectResult(xml);
        }
    }
}