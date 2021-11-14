using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            CarDealerContext dbContext = new CarDealerContext();

            using (dbContext)
            {
                dbContext.Database.EnsureCreated();
                Console.WriteLine("Database created successfully!");

                string suppliersInput = File.ReadAllText("../../../Datasets/suppliers.json");

                try
                {
                    string result = ImportSuppliers(dbContext, suppliersInput);
                    Console.WriteLine(result);
                }
                catch (Exception ex)
                {
                    dbContext.Database.EnsureDeleted();
                    Console.WriteLine(ex.Message);
                    throw;
                }

                dbContext.Database.EnsureDeleted();
            }

        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            IMapper mapper = GetMapper();

            IEnumerable<SupplierInputDTO> supplierDtos = JsonConvert.DeserializeObject<IEnumerable<SupplierInputDTO>>(inputJson);
            IEnumerable<Supplier> suppliers = mapper.Map<IEnumerable<Supplier>>(supplierDtos);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count()}.";
        }

        private static IMapper GetMapper()
        {
            MapperConfiguration config = new MapperConfiguration(cfg => cfg.AddProfile<CarDealerProfile>());
            return new Mapper(config);
        }
    }
}