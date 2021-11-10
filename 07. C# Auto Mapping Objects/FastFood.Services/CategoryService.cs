using AutoMapper;
using AutoMapper.QueryableExtensions;
using FastFood.Data;
using FastFood.Models;
using FastFood.Services.DTO.Categories;
using FastFood.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FastFood.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly FastFoodContext dbContext;
        private readonly IMapper mapper;

        public CategoryService(FastFoodContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public ICollection<ListAllCategoriesDTO> All()
            => dbContext.Categories
            .ProjectTo<ListAllCategoriesDTO>
            (mapper.ConfigurationProvider)
            .ToList();

        public void Create(CreateCategoryDTO dto)
        {
            Category category = mapper.Map<Category>(dto);
            dbContext.Categories.Add(category);
            dbContext.SaveChanges();
        }
    }
}
