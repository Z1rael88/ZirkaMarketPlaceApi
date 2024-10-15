using Application.Dtos;
using Domain.Models;
using Mapster;

namespace Application.Mappers;

public static class MapsterConfig
{
    public static void ProductMappings()
    {
        TypeAdapterConfig<ProductDto, Product>
            .NewConfig()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.TotalAmountSold, src => src.TotalAmountSold)
            .Map(dest => dest.AvailableAmount, src => src.AvailableAmount)
            .Map(dest=>dest.Price , src=>src.Price)
            .Map(dest=>dest.PhotoUrl , src=>src.PhotoUrl)
            .Map(dest => dest.Rating, src => src.Rating);
    }
}