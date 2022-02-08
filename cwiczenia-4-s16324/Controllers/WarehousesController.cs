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
    [Route("api/warehouses")]
    public class WarehousesController : ControllerBase
    {
        private IDatabaseService _dbService;

        public WarehousesController(IDatabaseService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost]
        public IActionResult RegisterProduct(Request request)
        {
            //Check Product Existance
            //Check Warehouse Existence
            //Check Amount

            //Check Order Existance (by IdProduct, Amount and CreatedAt)
            //Check if Order not Completed

            //If above true, set Order.FulfilledAt
            //Insert Product_warehouse record
            //Return Product_Warehouse.Id


            //_dbService.AddProduct(product)
            //return Ok(_dbService.RegisterProduct(request));
            int res = _dbService.RegisterProduct(request);
            IActionResult ret;
            if (res > 0)
            {
                ret = Ok(res);
            }
            else
            {
                ret = NotFound(new ReturnCodes().GetErrorMessage(res));
            }
            return ret;
        }

    }
}
