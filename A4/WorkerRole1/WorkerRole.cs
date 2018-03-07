using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("WorkerRole1 is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("WorkerRole1 has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("WorkerRole1 is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("WorkerRole1 has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=info344uwjx;AccountKey=n9u8q8z0P4CEcX6bMfR1wJAs/THsybQi+hYinHEEc8pSwtOv0lTyW6j+rQqrBSPIrTM1uPXNIr7+27JRLt0NHg==;BlobEndpoint=https://info344uwjx.blob.core.windows.net/;QueueEndpoint=https://info344uwjx.queue.core.windows.net/;TableEndpoint=https://info344uwjx.table.core.windows.net/;FileEndpoint=https://info344uwjx.file.core.windows.net/;");

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("a3-links");
            await queue.CreateIfNotExistsAsync();

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("a4terms");
            await table.CreateIfNotExistsAsync();

            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                //Trace.TraceInformation("Working");
                //await Task.Delay(1000);
                Thread.Sleep(50);
                CloudQueueMessage message2 = queue.GetMessage(TimeSpan.FromMinutes(5));
                if (message2 != null)
                {
                    Trace.TraceInformation(message2.AsString);
                    string url = message2.AsString;
                    if (url.EndsWith("html") || url.EndsWith("htm"))
                    {
                        AddToTable(url, table);
                    }
                    queue.DeleteMessage(message2);


                }
            }
        }

        private void AddToTable(string url, CloudTable table)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            try
            {
                htmlDoc.LoadHtml(new WebClient().DownloadString(url));
                //var title = htmlDoc.DocumentNode.SelectNodes("//h1[@class='pg-headline']");
                var title = htmlDoc.DocumentNode.SelectNodes("//h1[@class='pg-headline']");
                var HtmlDate = htmlDoc.DocumentNode.SelectNodes("//p[@class='update-time']");
                var text = htmlDoc.DocumentNode.SelectNodes("//section[@id='body-text']");


                if (title != null && HtmlDate != null)
                {
                    string[] ud = HtmlDate[0].InnerText.Split(' ');
                    string date = ud[5] + " " + ud[6] + " " + ud[7];
                    if (checkDate(ud[5], ud[7]))
                    {
                        String[] terms = title[0].InnerText.Split(' ');
                        HashSet<string> set = new HashSet<string>();
                        foreach (string term in terms)
                        {
                            string termLow = term.ToLower();
                            if (!set.Contains(termLow))
                            {
                                set.Add(termLow);
                                //string partKey = ud[6] + " " + ud[7];
                                Term l = new Term(date, termLow, title[0].InnerText, text[0].InnerText);
                                TableOperation to = TableOperation.Insert(l);
                                table.Execute(to);
                            }
                        }

                    }
                }
            } catch
            {

            }

           
        }

        private bool checkDate(string month, string year)
        {
            if (year.Equals("2018"))
            {
                return true;
            }
            return false;
        }
    }
}
