
namespace P03_FootballBetting
{
    using System;
    using Data;
 
    public class Program
    {
        static void Main(string[] args)
        {
            FootballBettingContext dbContext = new FootballBettingContext();

            dbContext.Database.EnsureCreated();

            Console.WriteLine("Database is created successfully!");

            dbContext.Database.EnsureDeleted();
        }
    }
}
