using System.Xml;
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
            string json = XmlUtils.SendRequest();

            XmlDocument xml = XmlUtils.ConvertJsonToXml(json);

            const string storageAccountName = "catfactsiri";
            const string containerName = "catfact";

            const string blobName = "catfacts.xml";

            bool resualt = XmlUtils.WriteToBlobStorage(storageAccountName, containerName, blobName, xml.OuterXml).Result;

            _logger.LogInformation($"Data was written with return: ");

            return new OkObjectResult(xml.OuterXml);
        }
    }
}
