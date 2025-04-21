using AutoMapper;
using Core.Domain.ViewModels;
using Core.Domain.Models;

namespace Core.Service.Configurations;

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