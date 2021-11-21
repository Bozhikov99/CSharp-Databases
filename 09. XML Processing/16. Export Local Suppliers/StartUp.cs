using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.DTO.Output;
using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (CarDealerContext context = new CarDealerContext())
            {
                context.Database.EnsureCreated();
                Console.WriteLine("Database Created");

                try
                {
                    string supplierXmlInput = File.ReadAllText("../../../Datasets/suppliers.xml");
                    string partXmlInput = File.ReadAllText("../../../Datasets/parts.xml");
                    string carXmlInput = File.ReadAllText("../../../Datasets/cars.xml");
                    string customerXmlInput = File.ReadAllText("../../../Datasets/customers.xml");
                    string salesXmlInput = File.ReadAllText("../../../Datasets/sales.xml");
                    ImportSuppliers(context, supplierXmlInput);
                    ImportParts(context, partXmlInput);
                    ImportCars(context, carXmlInput);
                    ImportCustomers(context, customerXmlInput);
                    ImportSales(context, salesXmlInput);
                    string result = GetLocalSuppliers(context);
                    Console.WriteLine(result);
                    context.Database.EnsureDeleted();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    context.Database.EnsureDeleted();
                    throw;
                }

            }
        }
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            Supplier[] suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .ToArray();

            ICollection<SupplierOutputDTO> dTOs = new HashSet<SupplierOutputDTO>();

            foreach (Supplier s in suppliers)
            {
                SupplierOutputDTO dTO = new SupplierOutputDTO()
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                };

                dTOs.Add(dTO);
            }

            XmlSerializer serializer = GetSerializer(typeof(HashSet<SupplierOutputDTO>), "suppliers");
            StringBuilder sb = new StringBuilder();
            using StringWriter writer = new StringWriter(sb);

            serializer.Serialize(writer, dTOs, GetNamespaces());

            return $"{sb.ToString().TrimEnd()}";
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            string make = "BMW";
            Car[] cars = context.Cars
                .Where(c => c.Make == make)
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .ToArray();

            ICollection<BmwOutputDTO> bmws = new HashSet<BmwOutputDTO>();

            foreach (Car c in cars)
            {
                BmwOutputDTO currentDto = new BmwOutputDTO()
                {
                    Id = c.Id,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                };

                bmws.Add(currentDto);
            }

            XmlSerializer serializer = GetSerializer(typeof(HashSet<BmwOutputDTO>), "cars");
            XmlSerializerNamespaces namespaces = GetNamespaces();

            StringBuilder sb = new StringBuilder();
            using StringWriter writer = new StringWriter(sb);
            serializer.Serialize(writer, bmws, namespaces);

            return $"{sb.ToString().TrimEnd()}";
        }
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            ICollection<Car> cars = context.Cars
                .Where(c => c.TravelledDistance > 2000000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ToArray();

            ICollection<CarDistanceOutputDTO> dTOs = new HashSet<CarDistanceOutputDTO>();

            foreach (Car c in cars)
            {
                CarDistanceOutputDTO current = new CarDistanceOutputDTO()
                {
                    TravelledDistance = c.TravelledDistance,
                    Make = c.Make,
                    Model = c.Model
                };

                dTOs.Add(current);
            }

            XmlSerializer serializer = GetSerializer(typeof(HashSet<CarDistanceOutputDTO>), "cars");

            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);

            serializer.Serialize(writer, dTOs, GetNamespaces());

            return $"{sb.ToString().TrimEnd()}";
        }
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            XmlSerializer serializer = GetSerializer(typeof(HashSet<SuppliersInputDTO>), "Suppliers");
            using StringReader reader = new StringReader(inputXml);

            HashSet<SuppliersInputDTO> dtos = (HashSet<SuppliersInputDTO>)serializer.Deserialize(reader);
            List<Supplier> suppliers = new List<Supplier>();

            foreach (SuppliersInputDTO dTO in dtos)
            {
                Supplier current = new Supplier()
                {
                    Name = dTO.Name,
                    IsImporter = dTO.IsImporter
                };

                suppliers.Add(current);
            }

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            XmlSerializer serializer = GetSerializer(typeof(PartInputDTO[]), "Parts");
            StringReader reader = new StringReader(inputXml);

            PartInputDTO[] dTOs = (PartInputDTO[])serializer.Deserialize(reader);
            List<Part> parts = new List<Part>();

            int[] supplierIds = context.Suppliers
                .Select(s => s.Id)
                .ToArray();

            foreach (PartInputDTO dTO in dTOs)
            {
                if (!supplierIds.Contains(dTO.SupplierId))
                {
                    continue;
                }
                Part current = new Part()
                {
                    Name = dTO.Name,
                    Price = dTO.Price,
                    Quantity = dTO.Quantity,
                    SupplierId = dTO.SupplierId
                };

                parts.Add(current);
            }

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            XmlSerializer serializer = GetSerializer(typeof(CarInputDTO[]), "Cars");
            using StringReader reader = new StringReader(inputXml);

            CarInputDTO[] dTOs = (CarInputDTO[])serializer.Deserialize(reader);
            ICollection<Car> cars = new HashSet<Car>();

            foreach (CarInputDTO dTO in dTOs)
            {
                Car current = new Car()
                {
                    Make = dTO.Make,
                    Model = dTO.Model,
                    TravelledDistance = dTO.TravelledDistance
                };

                ICollection<PartCar> parts = new HashSet<PartCar>();

                foreach (int partId in dTO.Parts.Select(p => p.Id).Distinct())
                {
                    Part part = context.Parts
                        .Find(partId);

                    if (part != null)
                    {
                        PartCar currentPart = new PartCar()
                        {
                            Part = part,
                            Car = current
                        };

                        parts.Add(currentPart);

                    }

                }

                cars.Add(current);
                context.PartCars.AddRange(parts);
            }


            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            XmlSerializer serializer = GetSerializer(typeof(CustomerInputDTO[]), "Customers");
            using StringReader reader = new StringReader(inputXml);

            CustomerInputDTO[] dTOs = (CustomerInputDTO[])serializer.Deserialize(reader);
            ICollection<Customer> customers = new HashSet<Customer>();

            foreach (CustomerInputDTO dTO in dTOs)
            {
                Customer current = new Customer()
                {
                    Name = dTO.Name,
                    BirthDate = dTO.BirthDate,
                    IsYoungDriver = dTO.IsYoungDriver
                };

                customers.Add(current);
            }

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            XmlSerializer serializer = GetSerializer(typeof(HashSet<SaleInputDTO>), "Sales");
            using StringReader reader = new StringReader(inputXml);

            ICollection<SaleInputDTO> dTOs = (HashSet<SaleInputDTO>)serializer.Deserialize(reader);
            ICollection<Sale> sales = new HashSet<Sale>();

            foreach (SaleInputDTO dTO in dTOs)
            {
                Car car = context.Cars.Find(dTO.CarId);

                if (car != null)
                {
                    Sale current = new Sale()
                    {
                        CarId = dTO.CarId,
                        Discount = dTO.Discount,
                        CustomerId = dTO.CustomerId
                    };

                    sales.Add(current);
                }
            }

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}";
        }

        private static XmlSerializer GetSerializer(Type type, string rootElement)
        {
            XmlRootAttribute root = new XmlRootAttribute(rootElement);
            XmlSerializer serializer = new XmlSerializer(type, root);
            return serializer;
        }

        private static XmlSerializerNamespaces GetNamespaces()
        {
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            return namespaces;
        }
    }
}