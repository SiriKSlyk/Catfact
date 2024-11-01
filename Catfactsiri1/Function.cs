using System.Xml;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Catfactsiri1
{
    public class Function
    {
        private readonly ILogger _logger;
        
        public Function(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function>();
            DateTimeOffset now = DateTimeOffset.UtcNow;
            long unixTimestamp = now.ToUnixTimeMilliseconds();
        }

        [Function("Function")]
        public void Run([TimerTrigger("* */1 * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
            if (myTimer.ScheduleStatus is not null)
            {
                const string storageAccountName = "catfactsiri";
                const string containerName = "catfact";
                string blobName = XmlHelper.GenerateFileNameWithTimeStamp();

                string json = XmlHelper.SendRequest();
                XmlDocument xml = XmlHelper.ConvertJsonToXml(json);

                bool status = XmlHelper.WriteToBlobStorage(storageAccountName, containerName, blobName, xml.OuterXml).Result;
                if (status)
                {
                    _logger.LogInformation($"Blob written with filename: {blobName}");
                }
                else
                {
                    _logger.LogInformation($"Failed to write {blobName} to storage container");
                }
            }
        }
    }
}