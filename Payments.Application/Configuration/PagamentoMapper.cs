using AutoMapper;
using Payments.Domain.Models;
using Payments.Domain.ApiModel;
using Payments.Domain.Enums;

namespace Payments.Application.Configurations;

public class PagamentoMapper : Profile
{
    public PagamentoMapper()
    {
        #region PagamentoWebHook Asaas
        CreateMap<PagamentoWebHookAsaasRequest, Pagamento>()
            .ForMember(dest => dest.DataVencimento, opt => opt.MapFrom(src => src.Payment.DueDate.GetValueOrDefault().ToUniversalTime()))
            .ForMember(dest => dest.DataPagamento, opt => opt.MapFrom(src => src.Payment.ClientPaymentDate.GetValueOrDefault().ToUniversalTime()))
            .ForMember(dest => dest.StatusPagamento, opt => opt.MapFrom(src => GetStatusPayment(src.Payment.Status)))
            .ForMember(dest => dest.AssinaturaId, opt => opt.MapFrom(src => int.Parse(src.Payment.ExternalReference ?? "0")))
            .ForMember(dest => dest.PagamentoIdExternal, opt => opt.MapFrom(src => src.Payment.Id))
            .ForMember(dest => dest.AssinaturaExternal, opt => opt.MapFrom(src => src.Payment.Subscription))
            .ForMember(dest => dest.TipoFaturamento, opt => opt.MapFrom(src => src.Payment.BillingType))
            .ForMember(dest => dest.FaturaURL, opt => opt.MapFrom(src => src.Payment.InvoiceUrl))
            .ForMember(dest => dest.NumeroFatura, opt => opt.MapFrom(src => src.Payment.InvoiceNumber))
            .ForMember(dest => dest.Valor, opt => opt.MapFrom(src => src.Payment.Value))
            .ReverseMap();
        #endregion
    }

    private static PagamentoStatusEnum GetStatusPayment(string? status)
    {
        return status switch
        {
            "CREATED" => PagamentoStatusEnum.NEW,
            "CONFIRMED" => PagamentoStatusEnum.CONFIRMED,
            "REFUNDED" => PagamentoStatusEnum.REFUNDED,
            "OVERDUE" => PagamentoStatusEnum.OVERDUE,
            "RECEIVED" => PagamentoStatusEnum.RECEIVED,
            "PENDING" => PagamentoStatusEnum.PROCESSING,
            "AWAITING_RISK_ANALYSIS" => PagamentoStatusEnum.PROCESSING,
            "APPROVED_BY_RISK_ANALYSIS" => PagamentoStatusEnum.APPROVED,
            "REPROVED_BY_RISK_ANALYSIS" => PagamentoStatusEnum.REPROVED,
            "PAYMENT_CREATED" => PagamentoStatusEnum.NEW,
            "PAYMENT_CONFIRMED" => PagamentoStatusEnum.CONFIRMED,
            "PAYMENT_REFUNDED" => PagamentoStatusEnum.REFUNDED,
            "PAYMENT_OVERDUE" => PagamentoStatusEnum.OVERDUE,
            "PAYMENT_RECEIVED" => PagamentoStatusEnum.RECEIVED,
            "PAYMENT_PENDING" => PagamentoStatusEnum.PROCESSING,
            "PAYMENT_AWAITING_RISK_ANALYSIS" => PagamentoStatusEnum.PROCESSING,
            "PAYMENT_APPROVED_BY_RISK_ANALYSIS" => PagamentoStatusEnum.APPROVED,
            "PAYMENT_REPROVED_BY_RISK_ANALYSIS" => PagamentoStatusEnum.REPROVED,
            _ => PagamentoStatusEnum.UNDEFINED
        };
    }
}