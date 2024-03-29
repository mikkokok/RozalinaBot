﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using RozalinaBot.Models;
using RozalinaBot.Helpers;
using Microsoft.Extensions.Configuration;

namespace RozalinaBot.Collectors.StorageAccount
{

        internal class StorageCollector
        {
            private readonly string _storageConnectionString;

            public StorageCollector(string storageAccountConnectionString)
            {
                _storageConnectionString = storageAccountConnectionString;
                UpdateCatLitterTime().Wait();
            }

            public async Task UpdateCatLitterTime()
            {
                var time = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                    TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time"));
                var table = await GetTableConnection (_storageConnectionString, "catLitter");
                var catEntity = new CatLitterEntity
                {
                    LitterDateTime = time,
                    RowKey = "Litter",
                    PartitionKey = "cat"
                    
                };
                var tableOperation = TableOperation.InsertOrReplace(catEntity);
                await table.ExecuteAsync(tableOperation);
            }

            public async Task<string> GetCatLitterTime()
            {
                var table = await GetTableConnection(_storageConnectionString, "catLitter");
                var tableOperation = TableOperation.Retrieve<CatLitterEntity>("cat", "Litter");
                var tableResult = await table.ExecuteAsync(tableOperation);
                if (tableResult == null)
                {
                    var task = UpdateCatLitterTime();
                    task.Wait();
                    return await GetCatLitterTime();
                }
                var entity = (CatLitterEntity)tableResult.Result;
                return TimeConverter.ConverToString(entity.LitterDateTime);
            }

            private static async Task<CloudTable> GetTableConnection(string storageConnectionString, string tableName)
            {
                var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
                var tableClient = storageAccount.CreateCloudTableClient();
                var table = tableClient.GetTableReference(tableName);
                await table.CreateIfNotExistsAsync();
                return table;
            }
        
    }
}
