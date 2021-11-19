using AutoMapper;
using ProductShop.Data;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (ProductShopContext dbContext = new ProductShopContext())
            {
                dbContext.Database.EnsureCreated();
                Console.WriteLine("Database Created.");

                string inputXmlUsers = File.ReadAllText("../../../Datasets/users.xml");
                string inputXmlProducts = File.ReadAllText("../../../Datasets/products.xml");
                try
                {
                    ImportUsers(dbContext, inputXmlUsers);
                    string result = ImportProducts(dbContext, inputXmlProducts);
                    Console.WriteLine(result);

                    dbContext.Database.EnsureDeleted();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    dbContext.Database.EnsureDeleted();
                    throw;
                }
            }
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            XmlRootAttribute root = new XmlRootAttribute("Users");
            XmlSerializer serializer = new XmlSerializer(typeof(UserInputDTO[]), root);

            using StringReader stringReader = new StringReader(inputXml);
            UserInputDTO[] dtos = (UserInputDTO[])serializer.Deserialize(stringReader);

            IMapper mapper = GetMapper();
            User[] users = mapper.Map<User[]>(dtos);

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Length}";
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            XmlRootAttribute root = new XmlRootAttribute("Products");
            XmlSerializer serializer = new XmlSerializer(typeof(ProductInputDTO[]), root);

            using StringReader reader = new StringReader(inputXml);

            ProductInputDTO[] dtos = (ProductInputDTO[])serializer.Deserialize(reader);

            IMapper mapper = GetMapper();
            Product[] products = mapper.Map<Product[]>(dtos);

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {context.Products.Count()}";
        }

        private static IMapper GetMapper()
        {
            MapperConfiguration config = new MapperConfiguration(cfg => cfg.AddProfile<ProductShopProfile>());
            return new Mapper(config);
        }
    }
}