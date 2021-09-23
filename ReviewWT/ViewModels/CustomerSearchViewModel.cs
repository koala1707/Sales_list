using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReviewWT.ViewModels
{
    public class CustomerSearchViewModel
    {
        public int id { get; set; }
        public int? purchasedYear { get; set; }
        public List<CustomerDetails> customers { get; set; }
        //public List<PurchasedItemDetails> items { get; set; }
        public PurchasedItemDetails itemDetails { get; set; }
        public string purchasedItemName { get; set; }
        public string purchasedItemImage { get; set; }
    }
}
