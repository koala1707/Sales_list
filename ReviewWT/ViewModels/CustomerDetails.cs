using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReviewWT.ViewModels
{
    public class CustomerDetails
    {
        public Models.CustomerOrder? customerDetails { get; set; }
        public int? customerId { get; set; }
        public int customerUnits { get; set; }  
        public decimal? totalCost { get; set; }

        
        //public string address { get; set; }

    }
}
