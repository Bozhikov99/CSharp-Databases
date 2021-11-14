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
                string partsInput = File.ReadAllText("../../../Datasets/parts.json");

                try
                {
                    ImportSuppliers(dbContext, suppliersInput);
                    string result = ImportParts(dbContext, partsInput);
                    Console.WriteLine(result);
                }
                catch (Exception ex)
                {
                    dbContext.Database.EnsureDeleted();
                    Console.WriteLine(ex.Message);
                    dbContext.Database.EnsureDeleted();
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

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            IMapper mapper = GetMapper();

            int[] supplierIds = context.Suppliers
                .Select(s => s.Id)
                .ToArray();

            IEnumerable<PartInputDTO> partDtos = JsonConvert.DeserializeObject<IEnumerable<PartInputDTO>>(inputJson)
                .Where(dto=>supplierIds.Contains(dto.SupplierId));

            IEnumerable<Part> parts = mapper.Map<IEnumerable<Part>>(partDtos);

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count()}."; ;
        }

        private static IMapper GetMapper()
        {
            MapperConfiguration config = new MapperConfiguration(cfg => cfg.AddProfile<CarDealerProfile>());
            return new Mapper(config);
        }
    }
}