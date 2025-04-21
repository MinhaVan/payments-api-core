using AutoMapper;
using Core.Domain.ViewModels;
using Core.Domain.Models;
using Core.Domain.Enums;

namespace Core.Service.Configurations;

public class PlanoMapper : Profile
{
    public PlanoMapper()
    {
        #region Plano
        CreateMap<PlanoViewModel, Plano>()
        .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Ativo ? StatusEntityEnum.Ativo : StatusEntityEnum.Deletado))
            .ReverseMap()
            .ForMember(dest => dest.Ativo, opt => opt.MapFrom(src => src.Status == StatusEntityEnum.Ativo));
        CreateMap<PlanoAdicionarViewModel, Plano>().ReverseMap();
        #endregion
    }
}