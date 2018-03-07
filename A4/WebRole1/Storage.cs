using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;

namespace WebRole1
{
    public class Storage
    {
        private CloudStorageAccount storageAccount;
        private CloudQueue queue;
        private CloudTable table;

        public Storage(string conString)
        {
            storageAccount = CloudStorageAccount.Parse(conString);
        }

        public CloudQueue getQueue(string name)
        {
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            queue = queueClient.GetQueueReference(name);
            return queue;
        }

        public CloudTable getTable(string name)
        {
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            table = tableClient.GetTableReference(name);
            return table;
        }
    }
}