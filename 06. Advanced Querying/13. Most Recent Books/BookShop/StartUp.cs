namespace BookShop
{
    using BookShop.Models;
    using BookShop.Models.Enums;
    using Castle.DynamicProxy.Generators;
    using Data;
    using Initializer;
    using Remotion.Linq.Parsing.Structure.IntermediateModel;
    using System;
    using System.Linq;
    using System.Text;
    using System.Xml;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new BookShopContext())
            {
                DbInitializer.ResetDatabase(db);

                string result = GetMostRecentBooks(db);
                Console.WriteLine(result);
            }
        }
        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder output = new StringBuilder();

            var categories = context
                .Categories
                .OrderBy(c => c.Name)
                .Select(c => new
                {
                    c.Name,
                    Books = context.BooksCategories.Where(b => b.CategoryId == c.CategoryId)
                        .Select(b => b.Book)
                        .OrderByDescending(b => b.ReleaseDate)
                        .Take(3)
                        .ToArray()
                })
                .ToArray();

            foreach (var c in categories)
            {
                output.AppendLine($"--{c.Name}");

                foreach (var b in c.Books)
                {
                    output.AppendLine($"{b.Title} ({b.ReleaseDate.Value.Year})");
                }
            }

            return output.ToString()
                .TrimEnd();
        }
    }
}
