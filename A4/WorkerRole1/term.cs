using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace WorkerRole1
{
    public class Term : TableEntity
    {
        public Term(string date, string term, string title, string text)
        {
            this.PartitionKey = term;
            this.RowKey = Guid.NewGuid().ToString();
            this.date = date;
            this.title = title;
            this.text = text;
        }

        public Term() { }

        public string date { set; get; }

        public string title { set; get; }

        public string text { set; get; }

    }
}

