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

                string inputJson = File.ReadAllText("../../../Datasets/categories.json");

                string result = ImportCategories(dbContext, inputJson);
                Console.WriteLine(result);

                dbContext.Database.EnsureDeleted();
            }
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            InitializeMapper();

            IEnumerable<CategoryInputDto> importedCategories = JsonConvert
                .DeserializeObject<IEnumerable<CategoryInputDto>>(inputJson)
                .Where(cid => !string.IsNullOrEmpty(cid.Name));

            IEnumerable<Category> categories = mapper.Map<IEnumerable<Category>>(importedCategories);

            context.Categories
                .AddRange(categories);

            context.SaveChanges();

            return $"Successfully imported {categories.Count()}";
        }

        private static void InitializeMapper()
        {
            MapperConfiguration configuration = new MapperConfiguration(cfg => cfg.AddProfile<ProductShopProfile>());
            mapper = new Mapper(configuration);
        }
    }
}