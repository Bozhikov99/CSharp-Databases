using AutoMapper;
using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;
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
                string inputXmlCategories = File.ReadAllText("../../../Datasets/categories.xml");
                string inputXmlCategoriesProducts = File.ReadAllText("../../../Datasets/categories-products.xml");
                try
                {
                    ImportUsers(dbContext, inputXmlUsers);
                    ImportProducts(dbContext, inputXmlProducts);
                    ImportCategories(dbContext, inputXmlCategories);
                    ImportCategoryProducts(dbContext, inputXmlCategoriesProducts);
                    string result = GetSoldProducts(dbContext);
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

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            XmlRootAttribute root = new XmlRootAttribute("Categories");
            XmlSerializer serializer = new XmlSerializer(typeof(CategoriesInputDTO[]), root);

            using StringReader reader = new StringReader(inputXml);
            IMapper mapper = GetMapper();

            CategoriesInputDTO[] dtos = (CategoriesInputDTO[])serializer.Deserialize(reader);
            Category[] categories = mapper.Map<Category[]>(dtos);

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Length}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            XmlRootAttribute root = new XmlRootAttribute("CategoryProducts");
            XmlSerializer serializer = new XmlSerializer(typeof(CategoriesProductsInputDTO[]), root);

            using StringReader reader = new StringReader(inputXml);
            IMapper mapper = GetMapper();

            CategoriesProductsInputDTO[] dtos = (CategoriesProductsInputDTO[])serializer.Deserialize(reader);
            CategoryProduct[] categoryProducts = mapper.Map<CategoryProduct[]>(dtos);

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Length}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            XmlRootAttribute root = new XmlRootAttribute("Products");
            XmlSerializer serializer = new XmlSerializer(typeof(ProductInRangeOutputDTO[]), root);

            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);

            IMapper mapper = GetMapper();

            Product[] products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Take(10)
                .ToArray();

            ProductInRangeOutputDTO[] dtos = mapper.Map<ProductInRangeOutputDTO[]>(products);
            serializer.Serialize(writer, dtos, GetNamespaces());

            return $"{builder.ToString().TrimEnd()}";
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            XmlRootAttribute root = new XmlRootAttribute("Users");
            XmlSerializer serializer = new XmlSerializer(typeof(UserProductsOutputDTO[]), root);

            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);
            IMapper mapper = GetMapper();

            User[] users = context.Users
                .Where(u => u.ProductsSold.Any())
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .ToArray();

            UserProductsOutputDTO[] dtos = mapper.Map<UserProductsOutputDTO[]>(users);
            serializer.Serialize(writer, dtos, GetNamespaces());

            return builder.ToString().TrimEnd();
        }

        private static XmlSerializerNamespaces GetNamespaces()
        {
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            return namespaces;
        }

        private static IMapper GetMapper()
        {
            MapperConfiguration config = new MapperConfiguration(cfg => cfg.AddProfile<ProductShopProfile>());
            return new Mapper(config);
        }
    }
}