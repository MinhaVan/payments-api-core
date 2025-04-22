using AutoMapper;
using Payments.Domain.ViewModels;
using Payments.Domain.Models;

namespace Payments.Application.Configurations;

public class RotaMapper : Profile
{
    public RotaMapper()
    {
        #region Rota
        CreateMap<RotaHistoricoViewModel, RotaHistorico>().ReverseMap();
        CreateMap<Rota2ViewModel, Rota>().ReverseMap();
        CreateMap<RotaDetalheViewModel, Rota>().ReverseMap();

        CreateMap<RotaViewModel, Rota>().ReverseMap();
        CreateMap<RotaAdicionarViewModel, Rota>().ReverseMap();
        CreateMap<RotaAtualizarViewModel, Rota>().ReverseMap();
        #endregion
    }
}