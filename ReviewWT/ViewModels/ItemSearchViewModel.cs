using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ReviewWT.ViewModels
{
    public class ItemSearchViewModel
    {
        public string searchText { get; set; }
        public int? year { get; set; }
        public SelectList years { get; set; }
        public List<ItemDetails> items  { get; set; }
        public List<string> itemList { get; set; }


        //public List<Models.CustomerOrder> customerOrders { get; set; }
        //public List<Models.ItemsInOrder> itemsInOrders { get; set; }
        //public SelectList categories { get; set; }
        //public interface IGrouping<Models.ItemsInOrder> itemsInOrders { get; set; }
        // public List<Models.Item> items { get; set; }
    }
}
