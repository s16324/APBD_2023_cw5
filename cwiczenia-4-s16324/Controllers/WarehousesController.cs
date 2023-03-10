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
