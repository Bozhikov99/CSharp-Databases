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

                string result = GetBooksByAuthor(db, input);
                Console.WriteLine(result);
            }
        }
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            string[] booksTitles = context.Books
               .Where(b => b.Author
                .LastName
                .ToLower()
                .StartsWith(input.ToLower()))
               .OrderBy(b => b.BookId)
               .Select(b => $"{b.Title} ({b.Author.FirstName} {b.Author.LastName})")
               .ToArray();

            return string.Join(Environment.NewLine, booksTitles);
        }
    }
}
