using Comissoes.Models;

namespace Comissoes.Services;

public interface IComissaoService
{
    CalculoComissaoResponse CalcularComissoes(VendasRequest request);
    (decimal Percentual, decimal ValorComissao, string Regra) CalcularComissaoItem(decimal valorVenda);
    VendasRequest ObterDadosExemplo();
}
