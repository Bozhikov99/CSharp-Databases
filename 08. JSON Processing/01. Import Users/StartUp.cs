using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
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
            User[] users = JsonConvert.DeserializeObject<User[]>(inputJson);

            context
                .Users
                .AddRange(users);

            context.SaveChanges();

            return $"Successfully imported {users.Length}";
        }
    }
}