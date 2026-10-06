using Movimentacoes.Models;

namespace Movimentacoes.Services;

public interface IEstoqueService
{
    IEnumerable<Produto> ObterTodosOsProdutos();
    Produto? ObterProdutoPorCodigo(int codigoProduto);
    MovimentacaoResponse LancarMovimentacao(MovimentacaoRequest request);
    IEnumerable<MovimentacaoResponse> ObterHistorico();
    void ResetarEstoque();
}
