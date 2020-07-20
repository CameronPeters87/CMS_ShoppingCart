using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CMS_ShoppingCart.Models.Entities
{
    public class SidebarDTO
    {
        [Key]
        public int Id { get; set; }
        public string Body { get; set; }
    }
}