using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CMS_ShoppingCart.Models.Entities
{
    public class PagesDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Body{ get; set; }
        public int Sorting { get; set; }
        public bool HasSidebar { get; set; }
    }
}