using AutoMapper;
using ProductShop.Data;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System;
using System.IO;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        private static IMapper mapper;
        public static void Main(string[] args)
        {
            using (ProductShopContext dbContext = new ProductShopContext())
            {
                dbContext.Database.EnsureCreated();
                Console.WriteLine("Database Created.");
                InitializeMapper();

                string inputXml = File.ReadAllText("../../../Datasets/users.xml");
                try
                {
                    string result = ImportUsers(dbContext, inputXml);
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
            User[] users = mapper.Map<User[]>(dtos);

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Length}";
        }

        private static void InitializeMapper()
        {
            MapperConfiguration config = new MapperConfiguration(cfg => cfg.AddProfile<ProductShopProfile>());
            mapper = new Mapper(config);
        }
    }
}