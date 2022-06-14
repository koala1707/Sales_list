using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReviewWT.Models.CodeFirst
{
    public class ExampleItem
    {
        [Key]
        public int itemID { get; set; }

        [Required]
        [Display(Name ="Item Name")]
        [StringLength(100, ErrorMessage ="The item name must be less than 100 characters")]
        public string itemName { get; set; }
        [Required]
        [Range(1,10000)]
        [Display(Name ="Price")]
        [DataType(DataType.Currency)]
        [Column(TypeName ="decimal(18,2)")]
        public decimal itemPrice { get; set; }
    }
}
