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

                string inputJson = File.ReadAllText("../../../Datasets/products.json");

                string result = ImportProducts(dbContext, inputJson);
                Console.WriteLine(result);

                dbContext.Database.EnsureDeleted();
            }
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            Product[] importedProducts = JsonConvert.DeserializeObject<Product[]>(inputJson);

            context.Products.AddRange(importedProducts);
            context.SaveChanges();

            return $"Successfully imported {importedProducts.Length}";
        }
    }
}