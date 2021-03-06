using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
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
                    ImportSuppliers(context, supplierXmlInput);
                    ImportParts(context, partXmlInput);
                    string result = ImportCars(context, carXmlInput);
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

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            XmlSerializer serializer = GetSerializer(typeof(SuppliersInputDTO[]), "Suppliers");
            using StringReader reader = new StringReader(inputXml);

            SuppliersInputDTO[] dtos = (SuppliersInputDTO[])serializer.Deserialize(reader);
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

        private static XmlSerializer GetSerializer(Type type, string rootElement)
        {
            XmlRootAttribute root = new XmlRootAttribute(rootElement);
            XmlSerializer serializer = new XmlSerializer(type, root);
            return serializer;
        }
    }
}