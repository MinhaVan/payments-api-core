using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Bogus;
using Payments.Domain.ApiModel;
using Payments.Domain.Enums;
using Payments.Domain.Interfaces.APIs;
using Payments.Domain.Interfaces.Repository;
using Payments.Domain.Models;
using Payments.Domain.ViewModels.Assinatura;
using Payments.Application.Exceptions;
using Payments.Application.Implementations;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Payments.Tests.Unitary
{
    public class AssinaturaServiceTests
    {
        private readonly Mock<IBaseRepository<Assinatura>> _assinaturaRepository = new();
        private readonly Mock<IBaseRepository<Usuario>> _usuarioRepository = new();
        private readonly Mock<IBaseRepository<Plano>> _planoRepository = new();
        private readonly Mock<IApiAsaas> _apiAsaas = new();
        private readonly Mock<IUserContext> _userContext = new();
        private readonly Mock<ILogger<AssinaturaService>> _logger = new();
        private readonly IMapper _mapper;
        private readonly AssinaturaService _assinaturaService;

        public AssinaturaServiceTests()
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                // Adicione sua configuração de mapeamento aqui
            }).CreateMapper();

            _assinaturaService = new AssinaturaService(
                _mapper,
                _logger.Object,
                _userContext.Object,
                _assinaturaRepository.Object,
                _usuarioRepository.Object,
                _planoRepository.Object,
                _apiAsaas.Object);
        }

        // [Fact]
        // public async Task AtualizarFormaPagamentoAsync_ValidRequest_ShouldUpdatePaymentMethod()
        // {
        //     // Arrange
        //     var usuarioId = 1;
        //     var empresaId = 1;
        //     var assinaturaDb = new Faker<Assinatura>()
        //         .RuleFor(x => x.UsuarioId, y => y.Random.Int(1, 9999))
        //         .RuleFor(x => x.Status, y => StatusEntityEnum.Ativo)
        //         .RuleFor(x => x.TipoPagamento, y => TipoPagamentoEnum.Boleto)
        //         .RuleFor(x => x.Pagamentos, y =>
        //             new Faker<Pagamento>()
        //                 .RuleFor(x => x.StatusPagamento, y => (int)PagamentoStatusEnum.CONFIRMED)
        //                 .Generate(1)
        //         )
        //         .Generate();

        //     _userContext.Setup(u => u.UserId).Returns(usuarioId);
        //     _userContext.Setup(u => u.Empresa).Returns(empresaId);
        //     _assinaturaRepository.Setup(r => r.BuscarUmAsync(It.IsAny<Expression<Func<Assinatura, bool>>>(), It.IsAny<Expression<Func<Assinatura, object>>[]>())).ReturnsAsync(assinaturaDb);
        //     _usuarioRepository.Setup(r => r.BuscarUmAsync(It.IsAny<Expression<Func<Usuario, bool>>>(), It.IsAny<Expression<Func<Usuario, object>>[]>())).ReturnsAsync(new Faker<Usuario>().Generate());

        //     var requisicao = new Faker<AtualizarFormaPagamento>()
        //         .RuleFor(x => x.NovoTipoPagamento, y => TipoPagamentoEnum.Pix)
        //         .RuleFor(x => x.Pix, y =>
        //             new Faker<PixViewModel>()
        //                 .RuleFor(x => x.UsuarioId, y => usuarioId)
        //                 .Generate())
        //         .Generate();

        //     // Act
        //     await _assinaturaService.AtualizarFormaPagamentoAsync(requisicao);

        //     // Assert
        //     _assinaturaRepository.Verify(r => r.AtualizarAsync(It.IsAny<Assinatura>()), Times.Once);
        // }

        [Fact]
        public async Task AtualizarFormaPagamentoAsync_PendingPayments_ShouldThrowBusinessRuleException()
        {
            // Arrange
            var usuarioId = 1;
            var empresaId = 1;
            var assinaturaDb = new Assinatura
            {
                UsuarioId = usuarioId,
                Status = StatusEntityEnum.Ativo,
                TipoPagamento = TipoPagamentoEnum.Boleto,
                Pagamentos = new List<Pagamento> { new Pagamento { StatusPagamento = (int)PagamentoStatusEnum.PROCESSING } }
            };

            _userContext.Setup(u => u.UserId).Returns(usuarioId);
            _userContext.Setup(u => u.Empresa).Returns(empresaId);
            _assinaturaRepository.Setup(r => r.BuscarUmAsync(It.IsAny<Expression<Func<Assinatura, bool>>>(), It.IsAny<Expression<Func<Assinatura, object>>[]>())).ReturnsAsync(assinaturaDb);

            var requisicao = new AtualizarFormaPagamento
            {
                NovoTipoPagamento = TipoPagamentoEnum.Pix,
                Pix = new PixViewModel { UsuarioId = usuarioId }
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleException>(() => _assinaturaService.AtualizarFormaPagamentoAsync(requisicao));
        }

        // [Fact]
        // public async Task ObterHistoricoAsync_ValidRequest_ShouldReturnAssinaturaViewModel()
        // {
        //     // Arrange
        //     var usuarioId = 1;
        //     var empresaId = 1;
        //     var alunoId = 1;

        //     var assinaturaDb = new Assinatura
        //     {
        //         UsuarioId = usuarioId,
        //         AlunoId = alunoId,
        //         Status = StatusEntityEnum.Ativo
        //     };

        //     _userContext.Setup(u => u.UserId).Returns(usuarioId);
        //     _userContext.Setup(u => u.Empresa).Returns(empresaId);
        //     _assinaturaRepository.Setup(r => r.BuscarUmAsync(It.IsAny<Expression<Func<Assinatura, bool>>>(), It.IsAny<Expression<Func<Assinatura, object>>[]>())).ReturnsAsync(assinaturaDb);

        //     // Act
        //     var result = await _assinaturaService.ObterHistoricoAsync(alunoId);

        //     // Assert
        //     Assert.NotNull(result);
        //     _assinaturaRepository.Verify(r => r.BuscarUmAsync(It.IsAny<Expression<Func<Assinatura, bool>>>(), It.IsAny<Expression<Func<Assinatura, object>>[]>()), Times.Once);
        // }

        [Fact]
        public async Task AtualizarFormaPagamentoAsync_InvalidUser_ShouldThrowArgumentException()
        {
            // Arrange
            var usuarioId = 1;
            _userContext.Setup(u => u.UserId).Returns(usuarioId);
            _assinaturaRepository.Setup(r => r.BuscarUmAsync(It.IsAny<Expression<Func<Assinatura, bool>>>(), It.IsAny<Expression<Func<Assinatura, object>>[]>())).ReturnsAsync((Assinatura)null);

            var requisicao = new AtualizarFormaPagamento
            {
                NovoTipoPagamento = TipoPagamentoEnum.Pix,
                Pix = new PixViewModel { UsuarioId = usuarioId }
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _assinaturaService.AtualizarFormaPagamentoAsync(requisicao));
        }

        [Fact]
        public async Task AtualizarFormaPagamentoAsync_InvalidPaymentType_ShouldThrowBusinessRuleException()
        {
            // Arrange
            var usuarioId = 1;
            var empresaId = 1;
            var assinaturaDb = new Assinatura
            {
                UsuarioId = usuarioId,
                Status = StatusEntityEnum.Ativo,
                TipoPagamento = TipoPagamentoEnum.Credito,
                Pagamentos = new List<Pagamento> { new Pagamento { StatusPagamento = (int)PagamentoStatusEnum.CONFIRMED } }
            };

            _userContext.Setup(u => u.UserId).Returns(usuarioId);
            _userContext.Setup(u => u.Empresa).Returns(empresaId);
            _assinaturaRepository.Setup(r => r.BuscarUmAsync(It.IsAny<Expression<Func<Assinatura, bool>>>(), It.IsAny<Expression<Func<Assinatura, object>>[]>())).ReturnsAsync(assinaturaDb);

            var requisicao = new AtualizarFormaPagamento
            {
                NovoTipoPagamento = (TipoPagamentoEnum)999 // Tipo inválido
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleException>(() => _assinaturaService.AtualizarFormaPagamentoAsync(requisicao));
        }
    }
}
