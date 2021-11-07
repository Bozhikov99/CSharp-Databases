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

                string result = CountCopiesByAuthor(db);
                Console.WriteLine(result);
            }
        }
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authorsCopies = context
                .Authors
                .Select(a => new
                {
                    a.FirstName,
                    a.LastName,
                    Copies = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(a => a.Copies)
                .ToArray();

            return string.Join(Environment.NewLine, authorsCopies.Select(a => $"{a.FirstName} {a.LastName} - {a.Copies}"));
        }
    }
}
