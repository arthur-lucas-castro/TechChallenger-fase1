using Atendimento.Application.Services.Interfaces;
using Catalogo.Application.DTOs;
using Catalogo.Application.Services.Interfaces;
using Compartilhado.Domain.Entities;
using Compartilhado.Domain.ValueObjects;
using Operacao.Application.DTOs;
using Operacao.Application.Services;
using Operacao.Domain.Entities;
using Operacao.Domain.Interfaces;

namespace Operacao.Tests.Application.Services;

public class OrdemServicoServiceTests
{
    private readonly Mock<IOrdemServicoRepositorio> _repositorioMock;
    private readonly Mock<IServicoService> _servicoServiceMock;
    private readonly Mock<IPecaService> _pecaServiceMock;
    private readonly Mock<IDomainEventDispatcher> _dispatcherMock;
    private readonly Mock<IClienteService> _clienteServiceMock;
    private readonly Mock<IVeiculoService> _veiculoServiceMock;
    private readonly OrdemServicoService _service;

    public OrdemServicoServiceTests()
    {
        _repositorioMock     = new Mock<IOrdemServicoRepositorio>();
        _servicoServiceMock  = new Mock<IServicoService>();
        _pecaServiceMock     = new Mock<IPecaService>();
        _dispatcherMock      = new Mock<IDomainEventDispatcher>();
        _clienteServiceMock  = new Mock<IClienteService>();
        _veiculoServiceMock  = new Mock<IVeiculoService>();

        _service = new OrdemServicoService(
            _repositorioMock.Object,
            _servicoServiceMock.Object,
            _pecaServiceMock.Object,
            _dispatcherMock.Object,
            _clienteServiceMock.Object,
            _veiculoServiceMock.Object);
    }

    private static OrdemServico CriarOrdemRecebida(int id = 1) => new()
    {
        Id = id, VeiculoId = 10, ClienteId = 20,
        Status = StatusOrdemServico.Recebida,
        DataCriacao = DateTime.UtcNow
    };

    private static OrdemServico CriarOrdemAguardandoAprovacao(int id = 1)
    {
        var os = CriarOrdemRecebida(id);
        os.FinalizarDiagnostico();
        os.ClearDomainEvents();
        return os;
    }

    // ── ObterPorIdAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task ObterPorIdAsync_Existe_RetornaDTO()
    {
        // Arrange
        _repositorioMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(CriarOrdemRecebida(1));

        // Act
        var resultado = await _service.ObterPorIdAsync(1);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(1, resultado.Id);
        Assert.Equal("Recebida", resultado.Status);
    }

    [Fact]
    public async Task ObterPorIdAsync_NaoExiste_RetornaNull()
    {
        // Arrange
        _repositorioMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((OrdemServico?)null);

        // Act
        var resultado = await _service.ObterPorIdAsync(99);

        // Assert
        Assert.Null(resultado);
    }

    // ── CriarAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task CriarAsync_ServicoNaoEncontrado_LancaKeyNotFoundException()
    {
        // Arrange
        _servicoServiceMock.Setup(s => s.ObterPorIdAsync(99)).ReturnsAsync((ServicoResponseDTO?)null);
        var dto = new OrdemServicoRequestDTO
        {
            VeiculoId = 10, ClienteId = 20,
            Servicos = [new ServicoSolicitadoRequestDTO { ServicoId = 99, Quantidade = 1 }],
            Pecas = []
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CriarAsync(dto));
    }

    [Fact]
    public async Task CriarAsync_PecaNaoEncontrada_LancaKeyNotFoundException()
    {
        // Arrange
        _servicoServiceMock.Setup(s => s.ObterPorIdAsync(1))
            .ReturnsAsync(new ServicoResponseDTO { Id = 1, PrecoVenda = 100m, Nome = "Alinhamento", TempoEstimadoEmMinutos = 30 });
        _pecaServiceMock.Setup(p => p.ObterPorIdAsync(99)).ReturnsAsync((Catalogo.Application.DTOs.PecaResponseDTO?)null);
        var dto = new OrdemServicoRequestDTO
        {
            VeiculoId = 10, ClienteId = 20,
            Servicos = [new ServicoSolicitadoRequestDTO { ServicoId = 1, Quantidade = 1 }],
            Pecas = [new PecaSolicitadaRequestDTO { PecaId = 99, Quantidade = 1 }]
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CriarAsync(dto));
    }

    [Fact]
    public async Task CriarAsync_Sucesso_ChamaInsertAsync()
    {
        // Arrange
        _servicoServiceMock.Setup(s => s.ObterPorIdAsync(1))
            .ReturnsAsync(new ServicoResponseDTO { Id = 1, PrecoVenda = 100m, Nome = "Alinhamento", TempoEstimadoEmMinutos = 30 });
        _repositorioMock.Setup(r => r.InsertAsync(It.IsAny<OrdemServico>())).ReturnsAsync(5);
        var dto = new OrdemServicoRequestDTO
        {
            VeiculoId = 10, ClienteId = 20,
            Servicos = [new ServicoSolicitadoRequestDTO { ServicoId = 1, Quantidade = 2 }],
            Pecas = []
        };

        // Act
        var id = await _service.CriarAsync(dto);

        // Assert
        Assert.Equal(5, id);
        _repositorioMock.Verify(r => r.InsertAsync(It.IsAny<OrdemServico>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_StatusInicialEhRecebida()
    {
        // Arrange
        _servicoServiceMock.Setup(s => s.ObterPorIdAsync(1))
            .ReturnsAsync(new ServicoResponseDTO { Id = 1, PrecoVenda = 80m, Nome = "Revisão", TempoEstimadoEmMinutos = 60 });
        _repositorioMock
            .Setup(r => r.InsertAsync(It.Is<OrdemServico>(os => os.Status == StatusOrdemServico.Recebida)))
            .ReturnsAsync(1);
        var dto = new OrdemServicoRequestDTO
        {
            VeiculoId = 10, ClienteId = 20,
            Servicos = [new ServicoSolicitadoRequestDTO { ServicoId = 1, Quantidade = 1 }],
            Pecas = []
        };

        // Act
        await _service.CriarAsync(dto);

        // Assert — InsertAsync recebeu entidade com status Recebida
        _repositorioMock.Verify(
            r => r.InsertAsync(It.Is<OrdemServico>(os => os.Status == StatusOrdemServico.Recebida)),
            Times.Once);
    }

    // ── IniciarDiagnosticoAsync ──────────────────────────────────────────────

    [Fact]
    public async Task IniciarDiagnosticoAsync_OrdemNaoEncontrada_RetornaFalse()
    {
        // Arrange
        _repositorioMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((OrdemServico?)null);

        // Act
        var resultado = await _service.IniciarDiagnosticoAsync(99);

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public async Task IniciarDiagnosticoAsync_Sucesso_ChamaCommit()
    {
        // Arrange
        var os = CriarOrdemRecebida(1);
        _repositorioMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(os);
        _repositorioMock.Setup(r => r.CommitAsync()).ReturnsAsync(true);

        // Act
        var resultado = await _service.IniciarDiagnosticoAsync(1);

        // Assert
        Assert.True(resultado);
        _repositorioMock.Verify(r => r.CommitAsync(), Times.Once);
    }

    // ── FinalizarDiagnosticoAsync ────────────────────────────────────────────

    [Fact]
    public async Task FinalizarDiagnosticoAsync_OrdemNaoEncontrada_RetornaFalse()
    {
        // Arrange
        _repositorioMock.Setup(r => r.GetByIdComItensAsync(99)).ReturnsAsync((OrdemServico?)null);

        // Act
        var resultado = await _service.FinalizarDiagnosticoAsync(99);

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public async Task FinalizarDiagnosticoAsync_Sucesso_ChamaCommitEDispatch()
    {
        // Arrange
        var os = CriarOrdemRecebida(1);
        _repositorioMock.Setup(r => r.GetByIdComItensAsync(1)).ReturnsAsync(os);
        _repositorioMock.Setup(r => r.CommitAsync()).ReturnsAsync(true);
        _dispatcherMock.Setup(d => d.DispatchAsync(It.IsAny<IEnumerable<IDomainEvent>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.FinalizarDiagnosticoAsync(1);

        // Assert — commit ANTES do dispatch
        _repositorioMock.Verify(r => r.CommitAsync(), Times.Once);
        _dispatcherMock.Verify(d => d.DispatchAsync(It.IsAny<IEnumerable<IDomainEvent>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FinalizarDiagnosticoAsync_Sucesso_LimpaEventosAposDispatch()
    {
        // Arrange
        var os = CriarOrdemRecebida(1);
        _repositorioMock.Setup(r => r.GetByIdComItensAsync(1)).ReturnsAsync(os);
        _repositorioMock.Setup(r => r.CommitAsync()).ReturnsAsync(true);
        _dispatcherMock.Setup(d => d.DispatchAsync(It.IsAny<IEnumerable<IDomainEvent>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.FinalizarDiagnosticoAsync(1);

        // Assert
        Assert.Empty(os.GetDomainEvents());
    }

    // ── AlterarStatusAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task AlterarStatusAsync_OrdemNaoEncontrada_RetornaFalse()
    {
        // Arrange
        _repositorioMock.Setup(r => r.GetByIdComItensAsync(99)).ReturnsAsync((OrdemServico?)null);

        // Act
        var resultado = await _service.AlterarStatusAsync(99, new AlterarStatusOrdemServicoDTO { Status = "EmDiagnostico" });

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public async Task AlterarStatusAsync_Sucesso_ChamaCommitEDispatch()
    {
        // Arrange — transição válida: Recebida → EmDiagnostico
        var os = CriarOrdemRecebida(1);
        _repositorioMock.Setup(r => r.GetByIdComItensAsync(1)).ReturnsAsync(os);
        _repositorioMock.Setup(r => r.CommitAsync()).ReturnsAsync(true);
        _dispatcherMock.Setup(d => d.DispatchAsync(It.IsAny<IEnumerable<IDomainEvent>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.AlterarStatusAsync(1, new AlterarStatusOrdemServicoDTO { Status = "EmDiagnostico" });

        // Assert
        _repositorioMock.Verify(r => r.CommitAsync(), Times.Once);
        _dispatcherMock.Verify(d => d.DispatchAsync(It.IsAny<IEnumerable<IDomainEvent>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── AdicionarServicoAsync ────────────────────────────────────────────────

    [Fact]
    public async Task AdicionarServicoAsync_OrdemNaoEncontrada_RetornaFalse()
    {
        // Arrange
        _repositorioMock.Setup(r => r.GetByIdComServicosAsync(99)).ReturnsAsync((OrdemServico?)null);

        // Act
        var resultado = await _service.AdicionarServicoAsync(99, new ServicoSolicitadoRequestDTO { ServicoId = 1, Quantidade = 1 });

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public async Task AdicionarServicoAsync_ServicoNaoEncontrado_LancaKeyNotFoundException()
    {
        // Arrange
        var os = CriarOrdemRecebida(1);
        _repositorioMock.Setup(r => r.GetByIdComServicosAsync(1)).ReturnsAsync(os);
        _servicoServiceMock.Setup(s => s.ObterPorIdAsync(99)).ReturnsAsync((ServicoResponseDTO?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.AdicionarServicoAsync(1, new ServicoSolicitadoRequestDTO { ServicoId = 99, Quantidade = 1 }));
    }

    [Fact]
    public async Task AdicionarServicoAsync_Sucesso_ChamaCommit()
    {
        // Arrange
        var os = CriarOrdemRecebida(1);
        _repositorioMock.Setup(r => r.GetByIdComServicosAsync(1)).ReturnsAsync(os);
        _servicoServiceMock.Setup(s => s.ObterPorIdAsync(1))
            .ReturnsAsync(new ServicoResponseDTO { Id = 1, PrecoVenda = 100m, Nome = "Revisão", TempoEstimadoEmMinutos = 30 });
        _repositorioMock.Setup(r => r.CommitAsync()).ReturnsAsync(true);

        // Act
        var resultado = await _service.AdicionarServicoAsync(1, new ServicoSolicitadoRequestDTO { ServicoId = 1, Quantidade = 2 });

        // Assert
        Assert.True(resultado);
        _repositorioMock.Verify(r => r.CommitAsync(), Times.Once);
    }

    // ── RemoverServicoAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task RemoverServicoAsync_OrdemNaoEncontrada_RetornaFalse()
    {
        // Arrange
        _repositorioMock.Setup(r => r.GetByIdComServicosAsync(99)).ReturnsAsync((OrdemServico?)null);

        // Act
        var resultado = await _service.RemoverServicoAsync(99, 1);

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public async Task RemoverServicoAsync_ServicoNaoNaOrdem_RetornaFalse()
    {
        // Arrange — ordem sem o serviço com servicoId=99
        var os = CriarOrdemRecebida(1);
        _repositorioMock.Setup(r => r.GetByIdComServicosAsync(1)).ReturnsAsync(os);

        // Act
        var resultado = await _service.RemoverServicoAsync(1, servicoId: 99);

        // Assert
        Assert.False(resultado);
        _repositorioMock.Verify(r => r.CommitAsync(), Times.Never);
    }

    // ── AdicionarPecaAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task AdicionarPecaAsync_PecaNaoEncontrada_LancaKeyNotFoundException()
    {
        // Arrange
        var os = CriarOrdemRecebida(1);
        _repositorioMock.Setup(r => r.GetByIdComPecasAsync(1)).ReturnsAsync(os);
        _pecaServiceMock.Setup(p => p.ObterPorIdAsync(99)).ReturnsAsync((Catalogo.Application.DTOs.PecaResponseDTO?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.AdicionarPecaAsync(1, new PecaSolicitadaRequestDTO { PecaId = 99, Quantidade = 1 }));
    }

    // ── ExcluirAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task ExcluirAsync_DelegaAoRepositorio()
    {
        // Arrange
        _repositorioMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        // Act
        var resultado = await _service.ExcluirAsync(1);

        // Assert
        Assert.True(resultado);
        _repositorioMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }
}
