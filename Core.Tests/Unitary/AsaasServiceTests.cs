using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Bogus;
using Core.Domain.ApiModel;
using Core.Domain.Interfaces.Repository;
using Core.Domain.Models;
using Core.Service.Configurations;
using Core.Service.Implementations;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Core.Tests.Unitary;
public class AsaasServiceTests
{
    private readonly Mock<IBaseRepository<Pagamento>> _paymentRepository = new();
    private readonly Mock<IBaseRepository<Assinatura>> _assinaturaRepository = new();
    private readonly Mock<ILogger<AsaasService>> _logger = new();
    private readonly IMapper _mapper;
    public readonly AsaasService _asaasService;
    public AsaasServiceTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new AjusteAlunoRotaMapper());
            cfg.AddProfile(new AlunoMapper());
            cfg.AddProfile(new AssinaturaMapper());
            cfg.AddProfile(new EnderecoMapper());
            cfg.AddProfile(new MotoristaMapper());
            cfg.AddProfile(new PagamentoMapper());
            cfg.AddProfile(new PaginadoMapper());
            cfg.AddProfile(new PlanoMapper());
            cfg.AddProfile(new RotaMapper());
            cfg.AddProfile(new TokenMapper());
            cfg.AddProfile(new UserMapper());
            cfg.AddProfile(new VeiculoMapper());
        });

        _mapper = config.CreateMapper();

        _asaasService = new AsaasService(_paymentRepository.Object, _assinaturaRepository.Object, _mapper, _logger.Object);
    }

    [Fact]
    public async Task PagamentoHookAsync_PaymentCreated_ShouldInsertPayment()
    {
        // Arrange
        var pagamentoWebHook = new Faker<PagamentoWebHookAsaasRequest>()
            .RuleFor(x => x.Event, y => "PAYMENT_CREATED")
            .RuleFor(x => x.Payment, y =>
                new Faker<PaymentDetails>()
                    .RuleFor(x => x.Id, y => y.Random.String())
                    .Generate()
            )
            .Generate();

        // Act
        var result = await _asaasService.PagamentoHookAsync(pagamentoWebHook);

        // Assert
        Assert.True(result);
        _paymentRepository.Verify(r => r.AdicionarAsync(It.IsAny<Pagamento>()), Times.Once);
    }

    [Fact]
    public async Task PagamentoHookAsync_PaymentConfirmed_ShouldUpdatePayment()
    {
        // Arrange
        var pagamentoWebHook = new Faker<PagamentoWebHookAsaasRequest>()
            .RuleFor(x => x.Event, y => "PAYMENT_CONFIRMED")
            .RuleFor(x => x.Payment, y =>
                new Faker<PaymentDetails>()
                    .RuleFor(x => x.Id, y => y.Random.String())
                    .Generate()
            )
            .Generate();

        var pagamento = new Faker<Pagamento>()
            .Generate();

        _paymentRepository.Setup(r => r.BuscarUmAsync(It.IsAny<Expression<Func<Pagamento, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<Pagamento, object>>[]>()))
            .ReturnsAsync(pagamento);

        _assinaturaRepository.Setup(x => x.ObterPorIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Faker<Assinatura>().Generate());

        // Act
        var result = await _asaasService.PagamentoHookAsync(pagamentoWebHook);

        // Assert
        Assert.True(result);
        _paymentRepository.Verify(r => r.AtualizarAsync(It.IsAny<Pagamento>()), Times.Once);
        _assinaturaRepository.Verify(r => r.AtualizarAsync(It.IsAny<Assinatura>()), Times.Once);
    }

    [Fact]
    public async Task PagamentoHookAsync_InvalidExternalId_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var pagamentoWebHook = new Faker<PagamentoWebHookAsaasRequest>()
            .RuleFor(x => x.Event, y => "PAYMENT_CREATED")
            .RuleFor(x => x.Payment, y =>
                new Faker<PaymentDetails>()
                    .RuleFor(x => x.Id, y => null)
                    .Generate()
            )
            .Generate();

        // Act
        var result = await _asaasService.PagamentoHookAsync(pagamentoWebHook);

        // Assert
        Assert.False(result);
        _paymentRepository.Verify(r => r.BuscarUmAsync(It.IsAny<Expression<Func<Pagamento, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<Pagamento, object>>[]>()), Times.Never);
    }

    [Fact]
    public async Task PagamentoHookAsync_PaymentNotFound_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var pagamentoWebHook = new Faker<PagamentoWebHookAsaasRequest>()
            .RuleFor(x => x.Event, y => "PAYMENT_CONFIRMED")
            .RuleFor(x => x.Payment, y =>
                new Faker<PaymentDetails>()
                    .RuleFor(x => x.Id, y => y.Random.String())
                    .Generate()
            )
            .Generate();

        _paymentRepository.Setup(r => r.BuscarUmAsync(It.IsAny<Expression<Func<Pagamento, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<Pagamento, object>>[]>()))
            .ReturnsAsync(null as Pagamento);

        // Act 
        var result = await _asaasService.PagamentoHookAsync(pagamentoWebHook);

        // Assert
        Assert.False(result);
        _paymentRepository.Verify(r => r.BuscarUmAsync(It.IsAny<Expression<Func<Pagamento, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<Pagamento, object>>[]>()), Times.Once);
        _paymentRepository.Verify(r => r.AtualizarAsync(It.IsAny<Pagamento>()), Times.Never);
    }

    [Fact]
    public async Task PagamentoHookAsync_UnknownEvent_ShouldReturnFalse()
    {
        // Arrange
        var pagamentoWebHook = new Faker<PagamentoWebHookAsaasRequest>()
            .RuleFor(x => x.Event, y => "UNKNOWN_EVENT")
            .RuleFor(x => x.Payment, y =>
                new Faker<PaymentDetails>()
                    .RuleFor(x => x.Id, y => y.Random.String())
                    .Generate()
            )
            .Generate();

        // Act
        var result = await _asaasService.PagamentoHookAsync(pagamentoWebHook);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task PagamentoHookAsync_ErrorDuringProcessing_ShouldReturnFalse()
    {
        // Arrange
        var pagamentoWebHook = new Faker<PagamentoWebHookAsaasRequest>()
            .RuleFor(x => x.Event, y => "PAYMENT_CREATED")
            .RuleFor(x => x.Payment, y =>
                new Faker<PaymentDetails>()
                    .RuleFor(x => x.Id, y => null)
                    .Generate()
            )
            .Generate();

        _paymentRepository.Setup(r => r.BuscarUmAsync(It.IsAny<Expression<Func<Pagamento, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<Pagamento, object>>[]>()))
            .ThrowsAsync(new Exception("Error"));

        // Act
        var result = await _asaasService.PagamentoHookAsync(pagamentoWebHook);

        // Assert
        Assert.False(result);
    }
}