namespace BookShop
{
    using BookShop.Models;
    using BookShop.Models.Enums;
    using Castle.DynamicProxy.Generators;
    using Data;
    using Initializer;
    using System;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new BookShopContext())
            {
                DbInitializer.ResetDatabase(db);
                string input = Console.ReadLine();

                Console.WriteLine(GetBooksByCategory(db, input));
            }
        }
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            StringBuilder output = new StringBuilder();
            string[] categories = input
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(c => c.ToLower())
                    .ToArray();

            context
                .Books
                .OrderBy(b => b.Title)
                .Where(x => x.BookCategories
                    .Any(bc => categories.Contains(bc.Category.Name.ToLower())))
                .Select(b => b.Title)
                .ToList()
                .ForEach(b => output.AppendLine(b));

            return output
                .ToString()
                .TrimEnd();
        }
    }
}
