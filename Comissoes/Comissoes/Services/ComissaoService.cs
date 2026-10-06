using Comissoes.Models;

namespace Comissoes.Services;

public class ComissaoService : IComissaoService
{
    public (decimal Percentual, decimal ValorComissao, string Regra) CalcularComissaoItem(decimal valorVenda)
    {
        if (valorVenda < 100.00m)
        {
            return (0.00m, 0.00m, "Vendas abaixo de R$ 100,00 não geram comissão");
        }
        else if (valorVenda < 500.00m)
        {
            var comissao = Math.Round(valorVenda * 0.01m, 2, MidpointRounding.AwayFromZero);
            return (0.01m, comissao, "Vendas abaixo de R$ 500,00 geram 1% de comissão");
        }
        else
        {
            var comissao = Math.Round(valorVenda * 0.05m, 2, MidpointRounding.AwayFromZero);
            return (0.05m, comissao, "A partir de R$ 500,00 gera 5% de comissão");
        }
    }

    public CalculoComissaoResponse CalcularComissoes(VendasRequest request)
    {
        var response = new CalculoComissaoResponse();

        if (request?.Vendas == null || request.Vendas.Count == 0)
        {
            return response;
        }

        var gruposPorVendedor = request.Vendas
            .GroupBy(v => string.IsNullOrWhiteSpace(v.Vendedor) ? "Não Informado" : v.Vendedor.Trim())
            .OrderBy(g => g.Key);

        foreach (var grupo in gruposPorVendedor)
        {
            var vendedorResponse = new ComissaoVendedorResponse
            {
                Vendedor = grupo.Key
            };

            foreach (var venda in grupo)
            {
                var (percentual, valorComissao, regra) = CalcularComissaoItem(venda.Valor);

                vendedorResponse.Detalhes.Add(new VendaComissaoDetalhe
                {
                    ValorVenda = Math.Round(venda.Valor, 2),
                    PercentualComissao = percentual,
                    ValorComissao = valorComissao,
                    RegraAplicada = regra
                });
            }

            vendedorResponse.QuantidadeVendas = vendedorResponse.Detalhes.Count;
            vendedorResponse.TotalVendas = Math.Round(vendedorResponse.Detalhes.Sum(d => d.ValorVenda), 2);
            vendedorResponse.TotalComissao = Math.Round(vendedorResponse.Detalhes.Sum(d => d.ValorComissao), 2);

            response.Vendedores.Add(vendedorResponse);
        }

        response.TotalVendasGeral = response.Vendedores.Sum(v => v.QuantidadeVendas);
        response.ValorTotalVendasGeral = Math.Round(response.Vendedores.Sum(v => v.TotalVendas), 2);
        response.ValorTotalComissaoGeral = Math.Round(response.Vendedores.Sum(v => v.TotalComissao), 2);

        return response;
    }

    public VendasRequest ObterDadosExemplo()
    {
        return new VendasRequest
        {
            Vendas = new List<VendaItem>
            {
                new() { Vendedor = "João Silva", Valor = 1200.50m },
                new() { Vendedor = "João Silva", Valor = 950.75m },
                new() { Vendedor = "João Silva", Valor = 1800.00m },
                new() { Vendedor = "João Silva", Valor = 1400.30m },
                new() { Vendedor = "João Silva", Valor = 1100.90m },
                new() { Vendedor = "João Silva", Valor = 1550.00m },
                new() { Vendedor = "João Silva", Valor = 1700.80m },
                new() { Vendedor = "João Silva", Valor = 250.30m },
                new() { Vendedor = "João Silva", Valor = 480.75m },
                new() { Vendedor = "João Silva", Valor = 320.40m },

                new() { Vendedor = "Maria Souza", Valor = 2100.40m },
                new() { Vendedor = "Maria Souza", Valor = 1350.60m },
                new() { Vendedor = "Maria Souza", Valor = 950.20m },
                new() { Vendedor = "Maria Souza", Valor = 1600.75m },
                new() { Vendedor = "Maria Souza", Valor = 1750.00m },
                new() { Vendedor = "Maria Souza", Valor = 1450.90m },
                new() { Vendedor = "Maria Souza", Valor = 400.50m },
                new() { Vendedor = "Maria Souza", Valor = 180.20m },
                new() { Vendedor = "Maria Souza", Valor = 90.75m },

                new() { Vendedor = "Carlos Oliveira", Valor = 800.50m },
                new() { Vendedor = "Carlos Oliveira", Valor = 1200.00m },
                new() { Vendedor = "Carlos Oliveira", Valor = 1950.30m },
                new() { Vendedor = "Carlos Oliveira", Valor = 1750.80m },
                new() { Vendedor = "Carlos Oliveira", Valor = 1300.60m },
                new() { Vendedor = "Carlos Oliveira", Valor = 300.40m },
                new() { Vendedor = "Carlos Oliveira", Valor = 500.00m },
                new() { Vendedor = "Carlos Oliveira", Valor = 125.75m },

                new() { Vendedor = "Ana Lima", Valor = 1000.00m },
                new() { Vendedor = "Ana Lima", Valor = 1100.50m },
                new() { Vendedor = "Ana Lima", Valor = 1250.75m },
                new() { Vendedor = "Ana Lima", Valor = 1400.20m },
                new() { Vendedor = "Ana Lima", Valor = 1550.90m },
                new() { Vendedor = "Ana Lima", Valor = 1650.00m },
                new() { Vendedor = "Ana Lima", Valor = 75.30m },
                new() { Vendedor = "Ana Lima", Valor = 420.90m },
                new() { Vendedor = "Ana Lima", Valor = 315.40m }
            }
        };
    }
}
