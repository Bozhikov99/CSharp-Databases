namespace BookShop
{
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
                int year = int.Parse(Console.ReadLine());
                Console.WriteLine(GetBooksNotReleasedIn(db, year));
            }
        }
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            StringBuilder output = new StringBuilder();

            context
                .Books
                .Where(b => b.ReleaseDate.Value.Year!=year)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToList()
                .ForEach(b => output.AppendLine(b));

            return output
                .ToString()
                .Trim();
        }
    }
}
