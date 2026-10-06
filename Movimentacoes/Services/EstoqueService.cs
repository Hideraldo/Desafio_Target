using System.Collections.Concurrent;
using Movimentacoes.Models;

namespace Movimentacoes.Services;

public class EstoqueService : IEstoqueService
{
    private readonly object _lockObj = new();
    private readonly Dictionary<int, Produto> _produtos = new();
    private readonly List<MovimentacaoResponse> _historico = new();
    private long _proximoId = 1000;

    public EstoqueService()
    {
        InicializarProdutos();
    }

    private void InicializarProdutos()
    {
        _produtos.Clear();
        _historico.Clear();
        _proximoId = 1000;

        var produtosIniciais = new List<Produto>
        {
            new() { CodigoProduto = 101, DescricaoProduto = "Caneta Azul", Estoque = 150 },
            new() { CodigoProduto = 102, DescricaoProduto = "Caderno Universitário", Estoque = 75 },
            new() { CodigoProduto = 103, DescricaoProduto = "Borracha Branca", Estoque = 200 },
            new() { CodigoProduto = 104, DescricaoProduto = "Lápis Preto HB", Estoque = 320 },
            new() { CodigoProduto = 105, DescricaoProduto = "Marcador de Texto Amarelo", Estoque = 90 }
        };

        foreach (var p in produtosIniciais)
        {
            _produtos[p.CodigoProduto] = p;
        }
    }

    public IEnumerable<Produto> ObterTodosOsProdutos()
    {
        lock (_lockObj)
        {
            return _produtos.Values
                .OrderBy(p => p.CodigoProduto)
                .Select(p => new Produto
                {
                    CodigoProduto = p.CodigoProduto,
                    DescricaoProduto = p.DescricaoProduto,
                    Estoque = p.Estoque
                })
                .ToList();
        }
    }

    public Produto? ObterProdutoPorCodigo(int codigoProduto)
    {
        lock (_lockObj)
        {
            if (_produtos.TryGetValue(codigoProduto, out var produto))
            {
                return new Produto
                {
                    CodigoProduto = produto.CodigoProduto,
                    DescricaoProduto = produto.DescricaoProduto,
                    Estoque = produto.Estoque
                };
            }

            return null;
        }
    }

    public MovimentacaoResponse LancarMovimentacao(MovimentacaoRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request), "A requisição não pode ser nula.");

        if (request.Quantidade <= 0)
            throw new ArgumentException("A quantidade movimentada deve ser maior que zero.", nameof(request.Quantidade));

        if (string.IsNullOrWhiteSpace(request.Descricao))
            throw new ArgumentException("A descrição para identificar o tipo da movimentação é obrigatória.", nameof(request.Descricao));

        lock (_lockObj)
        {
            if (!_produtos.TryGetValue(request.CodigoProduto, out var produto))
            {
                throw new KeyNotFoundException($"Produto com código {request.CodigoProduto} não encontrado no estoque do depósito.");
            }

            // Define ou valida o identificador único
            long idMovimentacao;
            if (request.Id.HasValue && request.Id.Value > 0)
            {
                idMovimentacao = request.Id.Value;
                if (_historico.Any(h => h.Id == idMovimentacao))
                {
                    throw new InvalidOperationException($"Já existe uma movimentação cadastrada com o identificador único {idMovimentacao}.");
                }
            }
            else
            {
                _proximoId++;
                idMovimentacao = _proximoId;
            }

            int estoqueAnterior = produto.Estoque;
            int estoqueFinal;

            switch (request.Tipo)
            {
                case TipoMovimentacao.Entrada:
                    estoqueFinal = estoqueAnterior + request.Quantidade;
                    produto.Estoque = estoqueFinal;
                    break;

                case TipoMovimentacao.Saida:
                    if (estoqueAnterior < request.Quantidade)
                    {
                        throw new InvalidOperationException(
                            $"Estoque insuficiente para a saída do produto '{produto.DescricaoProduto}'. " +
                            $"Estoque disponível: {estoqueAnterior}, quantidade solicitada: {request.Quantidade}.");
                    }
                    estoqueFinal = estoqueAnterior - request.Quantidade;
                    produto.Estoque = estoqueFinal;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(request.Tipo), "Tipo de movimentação inválido. Use 'Entrada' ou 'Saida'.");
            }

            var movimentacao = new MovimentacaoResponse
            {
                Id = idMovimentacao,
                DataHora = DateTime.Now,
                CodigoProduto = produto.CodigoProduto,
                DescricaoProduto = produto.DescricaoProduto,
                Tipo = request.Tipo,
                Descricao = request.Descricao.Trim(),
                QuantidadeMovimentada = request.Quantidade,
                EstoqueAnterior = estoqueAnterior,
                EstoqueFinal = estoqueFinal,
                Mensagem = $"Movimentação de {request.Tipo} realizada com sucesso. Estoque final: {estoqueFinal} unidades."
            };

            _historico.Add(movimentacao);

            return movimentacao;
        }
    }

    public IEnumerable<MovimentacaoResponse> ObterHistorico()
    {
        lock (_lockObj)
        {
            return _historico.OrderByDescending(h => h.DataHora).ToList();
        }
    }

    public void ResetarEstoque()
    {
        lock (_lockObj)
        {
            InicializarProdutos();
        }
    }
}
