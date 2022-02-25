﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DailyProduction.Model;
using Azure.Data.Tables;
using Azure;
using Azure.Data.Tables.Models;

namespace DailyProduction.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DailyProductionController : ControllerBase
    {
        private readonly ILogger<DailyProductionController> _logger;
        private TableClient tableClient;

        public DailyProductionController(ILogger<DailyProductionController> logger)
        {
            var serviceUri = "https://ibasstorage.table.core.windows.net/IBASProduction2020";
            var tableName = "IBASProduction2020";
            var accountName = "ibasstorage";
            var storageKey = "mQCl1FPEBxBmdmBYoWNREIc9b41DHjDy/LkUont9eVvlOCIYL9HQZFaEesY3i/52vHxnFR/YRZKF8kCjpnXIXw==";
           

            this.tableClient = new TableClient(
                new Uri(serviceUri),
                tableName,
                new TableSharedKeyCredential(accountName, storageKey)
            );

            _logger = logger;
        }
        
        [HttpGet]
        public IEnumerable<DailyProductionDTO> Get()
        {
            var production = new List<DailyProductionDTO> ();
            Pageable<TableEntity> entities = this.tableClient.Query<TableEntity>();

            foreach (var entity in entities)
            {
                var dto = new DailyProductionDTO
                {
                    Date = DateTime.Parse(entity.RowKey),
                    Model = (BikeModel)Enum.ToObject(typeof(BikeModel), Int32.Parse(entity.PartitionKey)),
                    ItemsProduced = (int)entity.GetInt32("itemsProduced")
                };
            production.Add(dto);
            }
            return production;
        }
    }
}
