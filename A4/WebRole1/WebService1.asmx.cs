using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Script.Serialization;
using System.Web.Services;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;

namespace WebRole1
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
        private static WebCrawler wc = new WebCrawler("https://cnn.com/robots.txt");
        private static Storage acc = new Storage("DefaultEndpointsProtocol=https;AccountName=info344uwjx;AccountKey=n9u8q8z0P4CEcX6bMfR1wJAs/THsybQi+hYinHEEc8pSwtOv0lTyW6j+rQqrBSPIrTM1uPXNIr7+27JRLt0NHg==;BlobEndpoint=https://info344uwjx.blob.core.windows.net/;QueueEndpoint=https://info344uwjx.queue.core.windows.net/;TableEndpoint=https://info344uwjx.table.core.windows.net/;FileEndpoint=https://info344uwjx.file.core.windows.net/;");
        //private static Hashtable cache;
        private static Cache cache = new Cache();
        private PerformanceCounter mem = new PerformanceCounter("Memory", "Available MBytes");
        private PerformanceCounter cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public void StartCrawling()
        {
            CheckCrawler();
            acc = new Storage("DefaultEndpointsProtocol=https;AccountName=info344uwjx;AccountKey=n9u8q8z0P4CEcX6bMfR1wJAs/THsybQi+hYinHEEc8pSwtOv0lTyW6j+rQqrBSPIrTM1uPXNIr7+27JRLt0NHg==;BlobEndpoint=https://info344uwjx.blob.core.windows.net/;QueueEndpoint=https://info344uwjx.queue.core.windows.net/;TableEndpoint=https://info344uwjx.table.core.windows.net/;FileEndpoint=https://info344uwjx.file.core.windows.net/;");
            wc.CrawlUrl("https://www.cnn.com", acc.getQueue("a3-links"));

        }

        [WebMethod]
        public void StopCrawling()
        {
            CheckCrawler();
            wc.stopCrawling();
        }

        [WebMethod]
        public void ClearIndex()
        {
            acc.getQueue("a3-links").Clear();
        }

        [WebMethod]
        public void GetPageTitle()
        {
            CheckCrawler();
        }

        [WebMethod]
        public string getData()
        {
            CheckCrawler();
            List<dynamic> list = new List<dynamic>();
            list.Add(wc.urlCount());
            list.Add(wc.getLastTenUrl());
            list.Add(QueueCount());
            list.Add(getMem());
            list.Add(wc.getLastTenError());
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(list);
            return json;
        }
        
        public string QueueCount()
        {
            //Storage store = new Storage("DefaultEndpointsProtocol=https;AccountName=info344uwjx;AccountKey=n9u8q8z0P4CEcX6bMfR1wJAs/THsybQi+hYinHEEc8pSwtOv0lTyW6j+rQqrBSPIrTM1uPXNIr7+27JRLt0NHg==;BlobEndpoint=https://info344uwjx.blob.core.windows.net/;QueueEndpoint=https://info344uwjx.queue.core.windows.net/;TableEndpoint=https://info344uwjx.table.core.windows.net/;FileEndpoint=https://info344uwjx.file.core.windows.net/;");
            CheckCrawler();
            CloudQueue q = acc.getQueue("a3-links");
            q.FetchAttributes();
            return q.ApproximateMessageCount + "";
        }


        public float getMem()
        {
            //Storage store = new Storage("DefaultEndpointsProtocol=https;AccountName=info344uwjx;AccountKey=n9u8q8z0P4CEcX6bMfR1wJAs/THsybQi+hYinHEEc8pSwtOv0lTyW6j+rQqrBSPIrTM1uPXNIr7+27JRLt0NHg==;BlobEndpoint=https://info344uwjx.blob.core.windows.net/;QueueEndpoint=https://info344uwjx.queue.core.windows.net/;TableEndpoint=https://info344uwjx.table.core.windows.net/;FileEndpoint=https://info344uwjx.file.core.windows.net/;");
            float memUsage = mem.NextValue();
            return memUsage;
        }

        public float getCPU()
        {
            float cpuUsage = cpu.NextValue();
            return cpuUsage;
        }

        [WebMethod]
        public string queryTable(string term)
        {
            List<string> list = new List<string>();
            List<string> ranked = new List<string>();
            if (cache[term] != null)
            {
                ranked = (List<string>) cache[term];
            } else
            {

                String[] words = term.Split(' ');
                foreach (string word in words)
                {
                    var query = from Term in acc.getTable("a4terms").CreateQuery<Term>()
                                where Term.PartitionKey == word
                                select Term;
                    foreach (Term entity in query)
                    {
                        list.Add(entity.title);
                    }
                }
                var hist = list
                    .GroupBy(x => x)
                    .OrderByDescending(grp => grp.Count())
                    .Select(grp => new { term = grp.Key, Count = grp.Count() }
                    );

                for (int i = 0; i < Math.Min(hist.Count(), 10); i++)
                {
                    ranked.Add(hist.ElementAt(i).term);
                }
                cache.Insert(term, ranked, null, DateTime.Now.AddMinutes(30), TimeSpan.Zero);
            }
            
            
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(ranked);
            return json;
        }

        

        private void CheckCrawler()
        {
            if (wc == null)
            {
                wc = new WebCrawler("https://cnn.com/robots.txt");
            }
            if(acc == null)
            {
                acc = new Storage("DefaultEndpointsProtocol=https;AccountName=info344uwjx;AccountKey=n9u8q8z0P4CEcX6bMfR1wJAs/THsybQi+hYinHEEc8pSwtOv0lTyW6j+rQqrBSPIrTM1uPXNIr7+27JRLt0NHg==;BlobEndpoint=https://info344uwjx.blob.core.windows.net/;QueueEndpoint=https://info344uwjx.queue.core.windows.net/;TableEndpoint=https://info344uwjx.table.core.windows.net/;FileEndpoint=https://info344uwjx.file.core.windows.net/;");
            }
        }
    }
}
