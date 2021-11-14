using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.DTOs.Input;
using ProductShop.DTOs.Output;
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

                string productInputJson = File.ReadAllText("../../../Datasets/products.json");
                string userInputJson = File.ReadAllText("../../../Datasets/users.json");
                string categoryInputJson = File.ReadAllText("../../../Datasets/categories.json");
                string categoryProductsInputJson = File.ReadAllText("../../../Datasets/categories-products.json");
                ImportUsers(dbContext, userInputJson);
                ImportProducts(dbContext, productInputJson);
                ImportCategories(dbContext, categoryInputJson);
                ImportCategoryProducts(dbContext, categoryProductsInputJson);
                string result = GetCategoriesByProductsCount(dbContext);
                Console.WriteLine(result);

                dbContext.Database.EnsureDeleted();
            }
        }
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            InitializeMapper();
            IEnumerable<UserInputDto> importedUsers = JsonConvert.DeserializeObject<IEnumerable<UserInputDto>>(inputJson);
            List<User> users = mapper.Map<List<User>>(importedUsers);
            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            InitializeMapper();
            IEnumerable<ProductInputDto> importedProducts = JsonConvert.DeserializeObject<IEnumerable<ProductInputDto>>(inputJson);
            List<Product> products = mapper.Map<List<Product>>(importedProducts);
            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
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
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context
                .Categories
                .Select(c => new
                {
                    Category = c.Name,
                    ProductsCount = c.CategoryProducts.Count,
                    AveragePrice = $"{c.CategoryProducts.Average(cp => cp.Product.Price):F2}",
                    TotalRevenue = $"{c.CategoryProducts.Sum(cp => cp.Product.Price):F2}"
                })
                .OrderByDescending(c=>c.ProductsCount)
                .ToList();

            return $"{JsonConvert.SerializeObject(categories, GetSettings())}";
        }
        private static void InitializeMapper()
        {
            MapperConfiguration configuration = new MapperConfiguration(cfg => cfg.AddProfile<ProductShopProfile>());
            mapper = new Mapper(configuration);
        }

        private static JsonSerializerSettings GetSettings()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };
        }
    }
}