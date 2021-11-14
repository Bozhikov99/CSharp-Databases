using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
                try
                {
                    string result = GetUsersWithProducts(dbContext);
                    Console.WriteLine(result);
                    char i914 = result[914];
                    Console.WriteLine(i914);
                }
                catch (Exception ex)
                {
                    dbContext.Database.EnsureDeleted();
                    Console.WriteLine(ex.Message);
                }


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
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var users = context.Users
                .Where(u => u.ProductsSold.Any(ps => ps.BuyerId != null))
                .Include(u => u.ProductsSold)
                .ToArray()
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    u.Age,
                    soldProducts = new
                    {
                        count = u.ProductsSold
                        .Where(p => p.BuyerId != null)
                        .Count(),
                        products = u.ProductsSold
                        .Where(p => p.BuyerId != null)
                        .Select(p => new
                        {
                            p.Name,
                            p.Price
                        })
                        .ToList()
                    }

                })
                .OrderByDescending(u => u.soldProducts.count)
                .ToList();


            var withUserCount = new
            {
                usersCount = users.Count,
                users,

            };

            var json = JsonConvert.SerializeObject(withUserCount, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            });
            return json;
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
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };
        }
    }
}