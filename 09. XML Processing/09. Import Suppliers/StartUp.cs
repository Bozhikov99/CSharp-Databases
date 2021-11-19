using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.IO;
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

                try
                {
                    string supplierXmlInput = File.ReadAllText("../../../Datasets/suppliers.xml");
                    string result = ImportSuppliers(context, supplierXmlInput);
                    Console.WriteLine(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    context.Database.EnsureDeleted();
                    throw;
                }

                context.Database.EnsureDeleted();
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

        private static XmlSerializer GetSerializer(Type type, string rootElement)
        {
            XmlRootAttribute root = new XmlRootAttribute(rootElement);
            XmlSerializer serializer = new XmlSerializer(type, root);
            return serializer;
        }
    }
}