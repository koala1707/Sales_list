using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReviewWT.ViewModels
{
    public class CategoryDetailViewModel
    {
        [Key]
        public int categoryID { get; set; }
        public string categoryName { get; set; }
        public int itemCount { get; set; }
    }
}
