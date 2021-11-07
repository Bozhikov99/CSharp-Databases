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

                Console.WriteLine(RemoveBooks(db));
            }
        }
        public static int RemoveBooks(BookShopContext context)
        {
            IQueryable<Book> books = context
                .Books
                .Where(b => b.Copies < 4200);

            int booksRemoved = books.ToArray()
                .Length;

            foreach (Book book in books)
            {
                IQueryable<BookCategory> bookCategories = context.BooksCategories
                    .Where(bc => bc.Book == book);

                context.RemoveRange(bookCategories);
            }

            context.Books
                .RemoveRange(books);

            context.SaveChanges();

            return booksRemoved;
        }
    }
}
