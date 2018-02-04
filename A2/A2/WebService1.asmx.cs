using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace A2
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {

        private static TrieTree tree;
        //private PerformanceCounter memP = new PerformanceCounter("Memory", "Available MBytes");

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        //[WebMethod]
        //public float GetAvailablemBytes()
        //{
        //    float memUsage = memP.NextValue();
        //    return memUsage;
        //}
        

        [WebMethod]
        public String buildWikiTree(int num)
        {
            string line;
            tree = new TrieTree();
            string fileDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + "\\test.txt";
            System.IO.StreamReader file = new System.IO.StreamReader(@fileDir);
            int counter = 0;
            while ((line = file.ReadLine()) != null && counter < num)
            {
                tree.insertWord(line);
                counter++;
                //if(memP.NextValue() < 500)
                //{
                //    break;
                //}
            }

            file.Close();
            return "Done";
        }

        [WebMethod]
        public string Download()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("blob1");


            if (container.Exists())
            {
                foreach (IListBlobItem item in container.ListBlobs(null, false))
                {
                    if (item.GetType() == typeof(CloudBlockBlob))
                    {
                        //CloudBlockBlob blob = (CloudBlockBlob)item;
                        //var fileStream = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + "\\test.txt";
                        //blob.DownloadToStream(fileStream);
                        
                    }
                }
            }

            CloudBlockBlob blockBlob = container.GetBlockBlobReference("wiki_data.txt");
            

            string name = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + "\\test.txt";

            using (var fileStream = System.IO.File.OpenWrite(@name))
            {
                blockBlob.DownloadToStream(fileStream);
            }
           
            return name;
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public string getResults(String term)
        {
            List<string> list = tree.GetResults(term);
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(list);
            return json;
        }
    }
}
