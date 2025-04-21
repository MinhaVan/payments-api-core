using AutoMapper;
using Core.Domain.Models;
using Core.Domain.ViewModels.Rota;

namespace Core.Service.Configurations;

public class AjusteAlunoRotaMapper : Profile
{
    public AjusteAlunoRotaMapper()
    {
        #region RotaAjusteEndereco
        CreateMap<RotaAjusteEnderecoViewModel, AjusteAlunoRota>().ReverseMap();
        #endregion
    }
}