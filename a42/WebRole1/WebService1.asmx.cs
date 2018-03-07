using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;

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

        private static WebCrawler wc;
        private static StorageAccount acc;
        [WebMethod]
        public void StartCrawling()
        {
            CheckCrawler();
            acc = new StorageAccount();
            wc.CrawlUrl("https://www.cnn.com", acc.getQueue());
           
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
            acc.getQueue().Clear();
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
            //list.Add(queueSize());
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(list);
            return json;
        }
        
        
        public int queueSize()
        {
            if(acc == null)
            {
                acc = new StorageAccount();
            }
            acc.getQueue().FetchAttributes();
            return (int) acc.getQueue().ApproximateMessageCount;
        }

        private void CheckCrawler()
        {
            if (wc == null)
            {
                wc = new WebCrawler("https://cnn.com/robots.txt");
            }
        }



    }
}
