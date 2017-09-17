
namespace StoresApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public partial class Transaction
    {
        public int Id { get; set; }
        public Nullable<int> ItemId { get; set; }
        public Nullable<int> StoreId { get; set; }
        public Nullable<int> ItemStoreId { get; set; }

       
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime Date { get; set; }

        [Display(Name = "Quantity")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter Positive integer Number")]
        public int TQ { get; set; }
        public string Type { get; set; }
    
        public virtual Item Item { get; set; }
        public virtual ItemStore ItemStore { get; set; }
        public virtual Store Store { get; set; }
    }
}
