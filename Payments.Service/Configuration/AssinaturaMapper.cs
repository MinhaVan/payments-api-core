using AutoMapper;
using Payments.Domain.ViewModels;
using Payments.Domain.Models;
using Payments.Domain.ApiModel;
using System;
using Payments.Domain.Enums;
using Payments.Domain.ViewModels.Assinatura;
using System.Linq;
using Payments.Domain.ViewModels.Rota;

namespace Payments.Application.Configurations;

public class AssinaturaMapper : Profile
{
    public AssinaturaMapper()
    {
        #region Assinatura
        CreateMap<CreditoViewModel, AssinaturaRequest>()
            .ForMember(x => x.Vencimento, opt => opt.MapFrom(z => DateTime.UtcNow))
            .ReverseMap();

        CreateMap<CartaoCreditoViewModel, CartaoCreditoRequest>().ReverseMap();
        CreateMap<InformacoesTitularCartaoViewModel, InformacoesTitularCartaoRequest>().ReverseMap();

        //
        CreateMap<Assinatura, AssinaturaViewModel>()
            .ForMember(x => x.Pagamentos, opt => opt.MapFrom(z => z.Pagamentos.OrderByDescending(x => x.DataCriacao)))
            .ReverseMap();

        CreateMap<Pagamento, PagamentoHistoricoViewModel>().ReverseMap();
        #endregion
    }
}