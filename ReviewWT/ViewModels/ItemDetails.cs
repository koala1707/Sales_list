using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReviewWT.ViewModels
{
    public class ItemDetails
    {
        public Models.Item item { get; set; }
        public int itemId { get; set; }
        public int unitsSold { get; set; }
        public int  customerEffect { get; set; }
        public Models.ItemCategory category { get; set; }
    }
}
