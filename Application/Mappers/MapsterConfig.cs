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
            .Map(dest => dest.Rating, src => src.Rating);
    }
}