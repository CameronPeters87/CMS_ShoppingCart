using CMS_ShoppingCart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS_ShoppingCart.Models.ViewModels
{
    public class CategoryVM
    {
        public CategoryVM()
        {

        }

        public CategoryVM(CategoryDTO dto)
        {
            Id = dto.Id;
            Name = dto.Name;
            Slug = dto.Slug;
            Sorting = dto.Sorting;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Sorting { get; set; }
    }
}