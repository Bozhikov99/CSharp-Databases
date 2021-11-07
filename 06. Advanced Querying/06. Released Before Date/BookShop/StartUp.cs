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

                Console.WriteLine(GetBooksReleasedBefore(db, input));
            }
        }
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            StringBuilder output = new StringBuilder();
            DateTime dateTime = DateTime.ParseExact(date, "dd-MM-yyyy", null);

            context
                .Books
                .OrderByDescending(b => b.ReleaseDate)
                .Where(b => b.ReleaseDate < dateTime)
                .Select(b => $"{b.Title} - {b.EditionType} - ${b.Price:F2}")
                .ToList()
                .ForEach(b => output.AppendLine(b));

            return output
                .ToString()
                .TrimEnd();
        }
    }
}
