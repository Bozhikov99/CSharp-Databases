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

                string inputJson = File.ReadAllText("../../../Datasets/users.json");

                string result = ImportUsers(dbContext, inputJson);
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

        private static void InitializeMapper()
        {
            MapperConfiguration configuration = new MapperConfiguration(cfg => cfg.AddProfile<ProductShopProfile>());
            mapper = new Mapper(configuration);
        }
    }
}