using AutoMapper;
using Payments.Domain.ViewModels;
using Payments.Domain.Models;
using Payments.Domain.Enums;

namespace Payments.Application.Configurations;

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