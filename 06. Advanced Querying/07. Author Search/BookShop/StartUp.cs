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

                Console.WriteLine(GetAuthorNamesEndingIn(db, input));
            }
        }
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            StringBuilder output = new StringBuilder();

            context
                .Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => $"{a.FirstName} {a.LastName}")
                .OrderBy(a => a)
                .ToList()
                .ForEach(a => output.AppendLine(a));

            return output
                .ToString()
                .TrimEnd();
        }
    }
}
