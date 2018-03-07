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
    public class StorageAccount
    {

        private CloudStorageAccount storageAccount;
        private CloudQueue queue;
        private CloudTable table;

        public StorageAccount()
        {
            storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            queue = queueClient.GetQueueReference("a3-links");

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            table = tableClient.GetTableReference("a3links");
        }
        
        public CloudQueue getQueue()
        {
            return queue;
        }

        public CloudTable getTable()
        {
            return table;
        }
    }
}