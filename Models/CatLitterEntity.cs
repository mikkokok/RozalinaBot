using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RozalinaBot.Models
{
    class CatLitterEntity
    {
        public DateTime LitterDateTime { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }

        public CatLitterEntity()
        {
            PartitionKey = "cat";
            RowKey = "Litter";
        }
    }
}
