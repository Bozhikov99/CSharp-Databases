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
                string cmd = Console.ReadLine();
                Console.WriteLine(GetGoldenBooks(db));
            }
        }
        public static string GetGoldenBooks(BookShopContext context)
        {
            StringBuilder output = new StringBuilder();

            context
                .Books
                .Where(b => (int)b.EditionType == 2&&b.Copies<5000)
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
