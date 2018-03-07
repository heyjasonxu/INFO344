using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace WorkerRole1
{
    class Link : TableEntity
    {
        public Link(string date, string url, string title, string text)
        {
            this.PartitionKey = date;
            this.RowKey = Guid.NewGuid().ToString();
            this.url = url;
            this.title = title;
            this.text = text;
        }

        public Link() { }

        public string url { set; get; }

        public string title { set; get; }

        public string text { set; get; }

    }
}
