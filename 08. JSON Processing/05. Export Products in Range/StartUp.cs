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
                ImportUsers(dbContext, userInputJson);
                ImportProducts(dbContext, productInputJson);
                string result = GetProductsInRange(dbContext);
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
        public static string GetProductsInRange(ProductShopContext context)
        {
            InitializeMapper();
            var productsQuery = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .ToList();

            IEnumerable<ProductInRangeOutputDto> prodsToSerialize = mapper.Map<IEnumerable<ProductInRangeOutputDto>>(productsQuery);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            return JsonConvert.SerializeObject(prodsToSerialize, settings);
    }
        private static void InitializeMapper()
        {
            MapperConfiguration configuration = new MapperConfiguration(cfg => cfg.AddProfile<ProductShopProfile>());
            mapper = new Mapper(configuration);
        }
    }
}