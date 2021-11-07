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
                int lengthCheck = int.Parse(Console.ReadLine());
                int result = CountBooks(db, lengthCheck);
                Console.WriteLine(result);
            }
        }
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            return context
                .Books
                .Where(b=>b.Title.Length>lengthCheck)
                .ToArray()
                .Length;
        }
    }
}
