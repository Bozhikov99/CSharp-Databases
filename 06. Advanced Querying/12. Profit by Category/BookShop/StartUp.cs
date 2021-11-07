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

                Console.WriteLine(GetTotalProfitByCategory(db));
                Console.WriteLine(GetTotalProfitByCategory(db).Length);
            }
        }
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categories = context
                .Categories
                .Select(c => new
                {
                    c.Name,
                    Profit = c.CategoryBooks
                        .Sum(cb => cb.Book.Copies * cb.Book.Price)
                })
                .OrderByDescending(c => c.Profit)
                .ThenBy(c => c.Name)
                .ToArray();

            return string.Join(Environment.NewLine, categories.Select(c => $"{c.Name} ${c.Profit:F2}"));
        }
    }
}
