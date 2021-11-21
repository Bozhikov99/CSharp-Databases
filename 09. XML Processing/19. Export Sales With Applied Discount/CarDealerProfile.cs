using AutoMapper;
using CarDealer.DTO.Output;
using CarDealer.Models;
using System.Linq;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            CreateMap<Customer, CustomerOutputDTO>()
                .ForMember(d => d.Name, y => y.MapFrom(s => s.Name))
                .ForMember(d => d.BoughtCars, y => y.MapFrom(s => s.Sales.Count()))
                .ForMember(d => d.SpentMoney, y => y.MapFrom(s => s.Sales.Sum(sale => sale.Car.PartCars.Sum(pc => pc.Part.Price))));
        }
    }
}
