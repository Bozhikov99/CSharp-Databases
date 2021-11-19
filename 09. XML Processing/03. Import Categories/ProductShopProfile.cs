﻿using AutoMapper;
using ProductShop.Dtos.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            CreateMap<UserInputDTO, User>();

            CreateMap<ProductInputDTO, Product>();

            CreateMap<CategoriesInputDTO, Category>();
        }
    }
}
