using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace ReviewWT.Models
{
    public partial class Item
    {
        public Item()
        {
            ItemMarkupHistories = new HashSet<ItemMarkupHistory>();
            ItemsInOrders = new HashSet<ItemsInOrder>();
            Reviews = new HashSet<Review>();
        }
        
        [Key]
        public int ItemId { get; set; }

        [Required]
        [Range(2, Int32.MaxValue)]
        [Display(Name = "Item Name")]
        [StringLength(50, ErrorMessage = "Words must be less than 50")]
        public string ItemName { get; set; }
        
        [Display(Name = "Item Description")]
        [StringLength(150, ErrorMessage = "Words must be less than 150")]
        public string ItemDescription { get; set; }
        
        [Required]
        [Range(1,10000)]
        [Display(Name = "Item Cost")]
        [DataType(DataType.Currency)]
        [Column(TypeName ="decimal(20,1)")]
        public decimal ItemCost { get; set; }
        
        [Display(Name = "Item Image")]
        public string ItemImage { get; set; }

        [Display(Name = "Category ID")]
        public int? CategoryId { get; set; }

        //public List<Item> items { get; set; }

        public virtual ItemCategory Category { get; set; }
        public virtual ICollection<ItemMarkupHistory> ItemMarkupHistories { get; set; }
        public virtual ICollection<ItemsInOrder> ItemsInOrders { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
