using AutoMapper;
using Payments.Domain.ViewModels;
using Payments.Domain.Models;

namespace Payments.Application.Configurations;

public class VeiculoMapper : Profile
{
    public VeiculoMapper()
    {
        CreateMap<VeiculoViewModel, Veiculo>().ReverseMap();
        CreateMap<VeiculoAdicionarViewModel, Veiculo>().ReverseMap();
        CreateMap<VeiculoAtualizarViewModel, Veiculo>().ReverseMap();
    }
}