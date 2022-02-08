using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cwiczenia_4_s16324.Services
{
    public class ReturnCodes
    {

        IDictionary<int, string> ReturnMessages = new Dictionary<int, string>(){
            {-1, "Product doesn't exist in database"},
            {-2, "Warehouse doesn't exist in database"},
            {-3, "Requested amount has to be >0"},
            {-4, "Didn't find suitable order (by IdProduct and Amount, placed before Request CreatedAt Date)"},
            {-5, "Order already fulfilled"},
            {-6, "Error while order fulfillment"},
            {-7, "Error while order fulfillment"}
        };

        public ReturnCodes()
        {

        }

        public string GetErrorMessage(int i)
        {
            return ReturnMessages[i];
        }

    }
}
