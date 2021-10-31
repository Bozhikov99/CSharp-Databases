using P01_StudentSystem.Data;
using System;

namespace P01_StudentSystem
{
    public class Program
    {
        static void Main(string[] args)
        {
            StudentSystemContext ctx = new StudentSystemContext();
            ctx.Database.EnsureCreated();

            Console.WriteLine("Success!");

            ctx.Database.EnsureDeleted();
        }
    }
}
