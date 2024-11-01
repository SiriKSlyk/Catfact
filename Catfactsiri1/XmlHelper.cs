using System.Net;
using System.Text;
using System.Xml;
using Azure.Storage.Blobs;
using Newtonsoft.Json;

namespace Catfactsiri1
{
    public static class XmlHelper
    {
        public static string SendRequest()
        {
            const string url = "https://catfact.ninja/facts";

            var request = WebRequest.Create(url);
            request.Method = "GET";

            var webRequest = request.GetResponse();
            var webStream = webRequest.GetResponseStream();

            var reader = new StreamReader(webStream);
            return reader.ReadToEnd();
        }

        public static XmlDocument ConvertJsonToXml(string json)
        {
            const string rootTag = "data";
            return JsonConvert.DeserializeXmlNode(json, rootTag);
        }
        public static async Task<bool> WriteToBlobStorage(string storageAcccountName, string containerName, string blobName, string content)
        {
            string StorageAccountConnString = Environment.GetEnvironmentVariable("StorageAccountConnectionString");
            var blobServiceClient = new BlobServiceClient(StorageAccountConnString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var blobClient = containerClient.GetBlobClient(blobName);

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            
            try
            {
                await blobClient.UploadAsync(stream, true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GenerateFileNameWithTimeStamp()
        {
            DateTime now = DateTime.Now;
            const string extention = "xml";
            return $"catfacts{now.ToString("ddMM-HH:mm")}.{extention}";
        }
    }
}