using cwiczenia_4_s16324.Models;
using cwiczenia_4_s16324.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cwiczenia_4_s16324.Controllers
{
    [ApiController]
    [Route("api/warehouses2")]
    public class WarehousesController2 : ControllerBase
    {
        private IDatabaseService _dbService;

        public WarehousesController2(IDatabaseService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost]
        public IActionResult RegisterProduct(Request request)
        {
            return NotFound(_dbService.RegisterProductByProcedure(request));
        }

    }
    
}
