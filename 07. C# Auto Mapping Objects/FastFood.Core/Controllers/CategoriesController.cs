namespace FastFood.Core.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using FastFood.Services.DTO.Categories;
    using FastFood.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels.Categories;

    public class CategoriesController : Controller
    {
        private readonly IMapper mapper;
        private readonly ICategoryService categoryService;

        public CategoriesController(IMapper mapper, ICategoryService categoryService)
        {
            this.mapper = mapper;
            this.categoryService = categoryService;
        }

        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Create(CreateCategoryInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Create");
            }

            CreateCategoryDTO categoryDto = mapper.Map<CreateCategoryDTO>(model);

            categoryService.Create(categoryDto);

            return RedirectToAction("All");
        }

        public IActionResult All()
        {

            ICollection<ListAllCategoriesDTO> categoryDtos = categoryService
                .All();

            List<CategoryAllViewModel> categoryViewModels = mapper
                .Map<ICollection<ListAllCategoriesDTO>,
                  ICollection<CategoryAllViewModel>>(categoryDtos)
                .ToList();

            return View("All", categoryViewModels);
        }
    }
}
