using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTOs.Input;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        private static IMapper mapper;
        public static void Main(string[] args)
        {
            ProductShopContext dbContext = new ProductShopContext();

            using (dbContext)
            {
                dbContext.Database
                    .EnsureCreated();


                Console.WriteLine("Database Created!");

                string inputJson = File.ReadAllText("../../../Datasets/categories-products.json");

                string result = ImportCategoryProducts(dbContext, inputJson);
                Console.WriteLine(result);

                dbContext.Database.EnsureDeleted();
            }
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            InitializeMapper();

            IEnumerable<CategoryProductInputDto> importedCategoryProducts = JsonConvert
                .DeserializeObject<IEnumerable<CategoryProductInputDto>>(inputJson);

            IEnumerable<CategoryProduct> categoryProducts = mapper.Map<IEnumerable<CategoryProduct>>(importedCategoryProducts);

            context.CategoryProducts.
                AddRange(categoryProducts);

            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count()}";
        }

        private static void InitializeMapper()
        {
            MapperConfiguration configuration = new MapperConfiguration(cfg => cfg.AddProfile<ProductShopProfile>());
            mapper = new Mapper(configuration);
        }
    }
}