using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RozalinaBot.Models
{
    internal class CatLitterEntity : TableEntity
    {
        public DateTime LitterDateTime { get; set; }

    }
}