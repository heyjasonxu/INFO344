using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;
using HtmlAgilityPack;
using Microsoft.WindowsAzure.Storage.Queue;

namespace WebRole1
{
    public class WebCrawler
    {
        private string URL;
        private List<string> articles;
        private HashSet<string> viewed;
        private Queue<string> linkQueue;
        private bool isCrawling;
        private List<string> lastTenUrl;


        public WebCrawler(string URL)
        {
            this.URL = URL;
            articles = new List<string>();
            viewed = new HashSet<string>();
            linkQueue = new Queue<string>();
            isCrawling = false;
            lastTenUrl = new List<string>();
        }

        

        public void sitemapArticles()
        {
            string url = @"https://www.cnn.com/sitemaps/sitemap-index.xml";
            XmlDocument xmlDoc = getXMLFromUrl(url);
            XmlNodeList g = xmlDoc.GetElementsByTagName("loc");
            string limit = "2017-12";
            string cnnArticle = "https://www.cnn.com/sitemaps/sitemap-articles-";
            List<string> l = new List<string>();
            foreach (XmlNode node in g)
            {
                string urlText = node.InnerText;
                if (urlText.StartsWith(cnnArticle))
                {
                    l.Add(urlText);
                    if (urlText.Contains(limit))
                    {
                        break;
                    }
                }
            }
            getArticles(l);
        }

        public void getArticles(List<string> monthList)
        {
            foreach(string monthXML in monthList)
            {
                XmlDocument doc = getXMLFromUrl(monthXML);
                XmlNodeList g = doc.GetElementsByTagName("loc");
                foreach(XmlNode i in g)
                {
                    articles.Add(i.InnerText);
                }
            }
        }

        public void CrawlUrl(string url, CloudQueue queue)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(new WebClient().DownloadString(url));
            var metaTags = htmlDoc.DocumentNode.SelectNodes("//a[@href]");
            List<string> list = new List<string>();
            isCrawling = true;
            foreach(HtmlNode i in metaTags)
            {
                if (isCrawling == false)
                {
                    break;
                }
                foreach (HtmlAttribute att in i.Attributes)
                {
                    if(isCrawling == false)
                    {
                        break;
                    }
                    if(att.Name.Equals("href"))
                    {
                        string link = att.Value;
                        
                        if ((link.StartsWith("/") || link.Contains("cnn.com")) && !viewed.Contains(link))
                        {
                            link = fixUrl(link);
                            addUrlToList(link);
                            list.Add(link);
                            linkQueue.Enqueue(link);
                            viewed.Add(link);
                            CloudQueueMessage message = new CloudQueueMessage(link);
                            queue.AddMessage(message);
                        }
                    }                    
                }
                //if (linkQueue.Count != 0)
                //{
                //    CrawlUrl(linkQueue.Dequeue(), queue);
                //}
            }
            while (linkQueue.Count != 0)
            {
                string q = linkQueue.Dequeue();
                
                CrawlUrl(q, queue);
            }
        }

        private void addUrlToList(string url)
        {
            if(lastTenUrl.Count < 10)
            {
                lastTenUrl.Insert(0, url);
            } else
            {
                lastTenUrl.RemoveAt(9);
                lastTenUrl.Insert(0, url);
            }
        }

        private string fixUrl(string url)
        {
            if(url.StartsWith("/")) {
                return "https://cnn.com" + url;
            }
            return url;
        }

        public void stopCrawling()
        {
            isCrawling = false;
        }

        public int urlCount()
        {
            return viewed.Count();
        }

        public List<string> getLastTenUrl()
        {
            return lastTenUrl;
        }

        private XmlDocument getXMLFromUrl(string url)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(url);
            return xmlDoc;
        }
    }
}