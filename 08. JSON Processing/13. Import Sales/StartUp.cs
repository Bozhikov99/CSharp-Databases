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
                string carsInput = File.ReadAllText("../../../Datasets/cars.json");
                string customersInput = File.ReadAllText("../../../Datasets/customers.json");
                string salesInput = File.ReadAllText("../../../Datasets/sales.json");

                try
                {
                    Console.WriteLine(ImportSuppliers(dbContext, suppliersInput));
                    Console.WriteLine(ImportParts(dbContext, partsInput));
                    Console.WriteLine(ImportCars(dbContext, carsInput));
                    Console.WriteLine(ImportCustomers(dbContext, customersInput));
                    Console.WriteLine(ImportSales(dbContext, salesInput));
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
                .Where(dto => supplierIds.Contains(dto.SupplierId));

            IEnumerable<Part> parts = mapper.Map<IEnumerable<Part>>(partDtos);

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count()}."; ;
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            IMapper mapper = GetMapper();

            IEnumerable<CarInputDTO> carDtos = JsonConvert.DeserializeObject<IEnumerable<CarInputDTO>>(inputJson);
            List<Car> cars = new List<Car>();

            foreach (var c in carDtos)
            {
                Car current = new Car
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                };

                foreach (var partId in c?.PartsId.Distinct())
                {
                    current.PartCars.Add(new PartCar
                    {
                        PartId = partId
                    });
                }

                cars.Add(current);
            }

            context.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count()}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            IMapper mapper = GetMapper();
            IEnumerable<CustomerDTO> customerDTOs = JsonConvert.DeserializeObject<IEnumerable<CustomerDTO>>(inputJson);
            IEnumerable<Customer> customers = mapper.Map<IEnumerable<Customer>>(customerDTOs);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count()}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            IMapper mapper = GetMapper();
            IEnumerable<SalesDTO> salesDTOs = JsonConvert.DeserializeObject<IEnumerable<SalesDTO>>(inputJson);
            IEnumerable<Sale> sales = mapper.Map<IEnumerable<Sale>>(salesDTOs);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count()}.";
        }

        private static IMapper GetMapper()
        {
            MapperConfiguration config = new MapperConfiguration(cfg => cfg.AddProfile<CarDealerProfile>());
            return new Mapper(config);
        }
    }
}