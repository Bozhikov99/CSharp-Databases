using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using CarDealer.DTO;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            CreateMap<SupplierInputDTO, Supplier>();

            CreateMap<PartInputDTO, Part>();

            CreateMap<CustomerDTO, Customer>();

            CreateMap<SalesDTO, Sale>();

            CreateMap<Customer, OrderedCustomersOutputDTO>()
                .ForMember(d => d.BirthDate, y => y.MapFrom(s => s.BirthDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)));
        }
    }
}
