using AutoMapper;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System.Linq;
using System.Xml;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            CreateMap<UserInputDTO, User>();

            CreateMap<ProductInputDTO, Product>();

            CreateMap<CategoriesInputDTO, Category>();

            CreateMap<CategoriesProductsInputDTO, CategoryProduct>();

            CreateMap<Product, ProductInRangeOutputDTO>()
                .ForMember(x => x.Buyer, y => y.MapFrom(s => $"{s.Buyer.FirstName} {s.Buyer.LastName}"))
                .ForMember(x => x.Price, y => y.MapFrom(s => $"{s.Price:F2}"));

            CreateMap<Product, UserProductsProductOutputDTo>();

            CreateMap<User, UserProductsOutputDTO>()
                .ForMember(x => x.SoldProducts, y => y.MapFrom(s => s.ProductsSold));

            CreateMap<Category, CategoriesByCountOutputDTO>()
                .ForMember(x => x.Count, y => y.MapFrom(s => s.CategoryProducts.Select(cp => cp.Product).Count()))
                .ForMember(x => x.AveragePrice, y => y.MapFrom(s => s.CategoryProducts.Select(cp => cp.Product).Average(p => p.Price)))
                .ForMember(x => x.TotalRevenue, y => y.MapFrom(s => s.CategoryProducts.Select(cp => cp.Product.Price).Sum()));
        }
    }
}
