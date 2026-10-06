namespace Comissoes.Models;

public class CalculoComissaoResponse
{
    public int TotalVendasGeral { get; set; }
    public decimal ValorTotalVendasGeral { get; set; }
    public decimal ValorTotalComissaoGeral { get; set; }
    public List<ComissaoVendedorResponse> Vendedores { get; set; } = new();
}
