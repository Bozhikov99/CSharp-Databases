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
            }
        }
        public static void IncreasePrices(BookShopContext context)
        {
            IQueryable<Book> books = context
                .Books
                .Where(b => b.ReleaseDate.Value.Year < 2010);

            foreach (Book book in books)
            {
                book.Price += 5;
            }
        }
    }
}
