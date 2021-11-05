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
                Console.WriteLine(GetBooksByAgeRestriction(db, cmd.ToLower()));
            }
        }
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            StringBuilder output = new StringBuilder();

            int cmdIndex = AgeRestriction.Minor.ToString().ToLower().Equals(command.ToLower()) ? 0 :
                AgeRestriction.Teen.ToString().ToLower().Equals(command.ToLower()) ? 1 :
                AgeRestriction.Adult.ToString().ToLower().Equals(command.ToLower()) ? 2 : 3;

            context
                .Books
                .Where(b => (int)b.AgeRestriction == cmdIndex)
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToList()
                .ForEach(b => output.AppendLine(b));

            return output
                .ToString()
                .Trim();
        }
    }
}
