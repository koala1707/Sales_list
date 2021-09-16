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
        public int? category { get; set; }
        public SelectList categories { get; set; }
        public List<Models.Item> items  { get; set; }
    }
}
