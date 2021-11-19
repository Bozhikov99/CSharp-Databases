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
                    ImportSuppliers(context, supplierXmlInput);
                    string result = ImportParts(context, partXmlInput);
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

        private static XmlSerializer GetSerializer(Type type, string rootElement)
        {
            XmlRootAttribute root = new XmlRootAttribute(rootElement);
            XmlSerializer serializer = new XmlSerializer(type, root);
            return serializer;
        }
    }
}