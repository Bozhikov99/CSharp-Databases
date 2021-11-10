using FastFood.Services.DTO.Categories;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastFood.Services.Interfaces
{
    public interface ICategoryService
    {
        void Create(CreateCategoryDTO dto);

        ICollection<ListAllCategoriesDTO> All();
    }
}
