using AutoMapper;
using Core.Domain.ViewModels;
using Core.Domain.Models;

namespace Core.Service.Configurations;

public class VeiculoMapper : Profile
{
    public VeiculoMapper()
    {
        CreateMap<VeiculoViewModel, Veiculo>().ReverseMap();
        CreateMap<VeiculoAdicionarViewModel, Veiculo>().ReverseMap();
        CreateMap<VeiculoAtualizarViewModel, Veiculo>().ReverseMap();
    }
}