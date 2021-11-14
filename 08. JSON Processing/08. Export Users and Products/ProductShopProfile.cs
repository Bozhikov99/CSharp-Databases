using AutoMapper;
using ProductShop.DTOs.Input;
using ProductShop.DTOs.Output;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            CreateMap<UserInputDto, User>();

            CreateMap<ProductInputDto, Product>();

            CreateMap<CategoryInputDto, Category>();

            CreateMap<CategoryProductInputDto, CategoryProduct>();

            CreateMap<Product, ProductInRangeOutputDto>()
                .ForMember(x=>x.Seller, y=>y.MapFrom(s=>$"{s.Seller.FirstName} {s.Seller.LastName}"));
        }
    }
}
