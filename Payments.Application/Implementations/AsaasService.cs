using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Payments.Domain.ApiModel;
using Payments.Domain.Interfaces.Repository;
using Payments.Domain.Interfaces.Services;
using Payments.Domain.Models;
using Payments.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Payments.Application.Implementations;

public class AsaasService : IAsaasService
{
    private readonly IMapper _mapper;
    private readonly IBaseRepository<Assinatura> _assinaturaRepository;
    private readonly IBaseRepository<Pagamento> _paymentRepository;
    private readonly ILogger _logger;
    public AsaasService(
        IBaseRepository<Pagamento> paymentRepository,
        IBaseRepository<Assinatura> assinaturaRepository,
        IMapper mapper,
        ILogger<AsaasService> logger)
    {
        _assinaturaRepository = assinaturaRepository;
        _paymentRepository = paymentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<bool> PagamentoHookAsync(PagamentoWebHookAsaasRequest payment)
    {
        try
        {
            _logger.Log(LogLevel.Information, $"[PagamentoHook - Asaas] [{payment.Payment?.Id}] - Requisição recebida. Payload: {JsonConvert.SerializeObject(payment, Formatting.None)}");
            var response = await HandlerAsync(payment);

            _logger.Log(LogLevel.Information, $"[PagamentoHook - Asaas] [{payment.Payment?.Id}] - Integração finalizada com sucesso!");
            return response;
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, $"[PagamentoHook - Asaas] [{payment.Payment?.Id}] - Ocorreu um erro ao tentar integrar o pagamento do Asaas - Payment: {JsonConvert.SerializeObject(payment, Formatting.None)} | Detail of error: {JsonConvert.SerializeObject(ex, Formatting.None)}");
            return false;
        }
    }

    #region Privaate Methods
    private async Task<bool> HandlerAsync(PagamentoWebHookAsaasRequest payment) =>
        payment.Event switch
        {
            "PAYMENT_CREATED" => await InserirPagamentoAsync(payment),
            "PAYMENT_CONFIRMED" => await AtualizarPagamentoAsync(payment),
            "PAYMENT_REFUNDED" => await AtualizarPagamentoAsync(payment),
            "PAYMENT_OVERDUE" => await AtualizarPagamentoAsync(payment),
            "PAYMENT_RECEIVED" => await AtualizarPagamentoAsync(payment),
            "PAYMENT_AWAITING_RISK_ANALYSIS" => await AtualizarPagamentoAsync(payment),
            "PAYMENT_APPROVED_BY_RISK_ANALYSIS" => await AtualizarPagamentoAsync(payment),
            "PAYMENT_REPROVED_BY_RISK_ANALYSIS" => await AtualizarPagamentoAsync(payment),
            _ => false
        };

    private async Task<bool> InserirPagamentoAsync(PagamentoWebHookAsaasRequest payment)
    {
        var payload = _mapper.Map<Pagamento>(payment);
        _logger.Log(LogLevel.Information, $"[InserirPagamentoAsync] [{payload.PagamentoIdExternal}] Iniciando método para integrar pagamento: '{JsonConvert.SerializeObject(payload)}'");

        if (string.IsNullOrEmpty(payload.PagamentoIdExternal))
            throw new BusinessRuleException($"External ID inválido!");

        var pagamentos = await _paymentRepository.BuscarAsync(x => x.PagamentoIdExternal == payload.PagamentoIdExternal);
        if (pagamentos.Count() > 1)
            return true;

        payload.Status = Domain.Enums.StatusEntityEnum.Ativo;
        await _paymentRepository.AdicionarAsync(payload);

        _logger.Log(LogLevel.Information, $"[InserirPagamentoAsync] [{payload.PagamentoIdExternal}] Integração realizada com sucesso!");
        return true;
    }

    private async Task<bool> AtualizarPagamentoAsync(PagamentoWebHookAsaasRequest payment)
    {
        var payload = _mapper.Map<Pagamento>(payment);
        _logger.Log(LogLevel.Information, $"[AtualizarPagamentoAsync] [{payload.PagamentoIdExternal}] Iniciando método para atualizar pagamento: '{JsonConvert.SerializeObject(payload)}'");

        if (string.IsNullOrEmpty(payload.PagamentoIdExternal))
            throw new BusinessRuleException($"External ID inválido!");

        var pagamento = await _paymentRepository.BuscarUmAsync(x => x.PagamentoIdExternal == payload.PagamentoIdExternal);
        _logger.Log(LogLevel.Information, $"[AtualizarPagamentoAsync] [{payload.PagamentoIdExternal}] Pagamento capturado! Data: '{JsonConvert.SerializeObject(pagamento)}'");

        if (pagamento is null)
        {
            _logger.Log(LogLevel.Information, $"[AtualizarPagamentoAsync] [{payload.PagamentoIdExternal}] Pagamento não encontrado!");
            throw new BusinessRuleException($"Pagamento '{payload.PagamentoIdExternal}' não encontrado.");
        }

        payload.Id = pagamento.Id;
        payload.Status = Domain.Enums.StatusEntityEnum.Ativo;
        payload.DataCriacao = pagamento.DataCriacao;
        await _paymentRepository.AtualizarAsync(payload);

        if (payment.Event == "PAYMENT_RECEIVED" || payment.Event == "PAYMENT_CONFIRMED")
        {
            var assinatura = await _assinaturaRepository.ObterPorIdAsync(payload.AssinaturaId);
            assinatura.Vencimento = DateTime.UtcNow.AddMonths(1);
            await _assinaturaRepository.AtualizarAsync(assinatura);
        }

        _logger.Log(LogLevel.Information, $"[AtualizarPagamentoAsync] [{payload.PagamentoIdExternal}] Pagamento atualizado com sucesso!");
        return true;
    }
    #endregion
}