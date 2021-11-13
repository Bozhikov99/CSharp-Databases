using AutoMapper;
using ProductShop.DTOs.Input;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            CreateMap<UserInputDto, User>();
        }
    }
}
