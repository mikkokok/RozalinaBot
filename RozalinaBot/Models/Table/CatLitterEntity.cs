using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace RozalinaBot.Models.Table
{
    class CatLitterEntity :  TableEntity
    {
        public DateTime LitterDateTime { get; set; }

        public CatLitterEntity()
        {
            PartitionKey = "cat";
            RowKey = "Litter";
        }
    }
}
