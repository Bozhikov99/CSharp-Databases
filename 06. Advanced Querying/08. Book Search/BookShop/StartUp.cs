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

                string result = GetBookTitlesContaining(db, input);
                Console.WriteLine(result);
            }
        }
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
//            StringBuilder output = new StringBuilder();

            return string.Join(Environment.NewLine, context
                  .Books
                  .OrderBy(b => b.Title)
                  .Where(b => b.Title
                      .ToLower()
                      .Contains(input.ToLower()))
                  .Select(b => b.Title)
                  .ToList());

//            return output
//                .ToString()
//                .TrimEnd();
        }
    }
}
