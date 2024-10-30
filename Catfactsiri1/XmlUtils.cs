using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Azure.Storage.Blobs;
using Newtonsoft.Json;

namespace Catfactsiri1
{
    public static class XmlUtils
    {
        
        
        public static string SendRequest()
        {
            string url = "https://catfact.ninja/facts";

            var request = WebRequest.Create(url);
            request.Method = "GET";

            var webRequest = request.GetResponse();
            var webStream = webRequest.GetResponseStream();

            var reader = new StreamReader(webStream);
            return reader.ReadToEnd();
        }

        public static XmlDocument ConvertJsonToXml(string json)
        {
            XmlDocument xml = JsonConvert.DeserializeXmlNode(json, "data");
            return xml;
        }
        public static async Task<bool> WriteToBlobStorage(string storageAcccountName, string containerName, string blobName, string content)
        {
            string StorageAccountConnString = Environment.GetEnvironmentVariable("StorageAccountConnString");
            var blobServiceClient = new BlobServiceClient(StorageAccountConnString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var blobClient = containerClient.GetBlobClient(blobName);

            //using FileStream uploadFileStream = File.OpenRead(blobName);


            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            await blobClient.UploadAsync(stream, true);

            return true;
        }
    }
}
