using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Catfactsiri1
{
    public class Function
    {
        private readonly ILogger _logger;
        private DateTime now;

        public Function(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function>();
            DateTimeOffset now = DateTimeOffset.UtcNow;
            long unixTimestamp = now.ToUnixTimeMilliseconds();


        }

        [Function("Function")]
        public void Run([TimerTrigger("* */1 30 10 *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
            if (myTimer.ScheduleStatus is not null)
            {
                string json = XmlUtils.SendRequest();

                XmlDocument xml = XmlUtils.ConvertJsonToXml(json);

                const string storageAccountName = "catfactsiri";
                const string containerName = "catfact";

                now = DateTime.Now;

                string blobName = $"catfacts{now.ToString("ddMM-HHmm")}.xml";


                bool resualt = XmlUtils.WriteToBlobStorage(storageAccountName, containerName, blobName, xml.OuterXml).Result;
                _logger.LogInformation($"Blob written with filename: {blobName}");
            }
        }
    }
}
