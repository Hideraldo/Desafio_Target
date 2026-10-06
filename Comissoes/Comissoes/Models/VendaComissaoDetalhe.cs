namespace Comissoes.Models;

public class VendaComissaoDetalhe
{
    public decimal ValorVenda { get; set; }
    public decimal PercentualComissao { get; set; }
    public decimal ValorComissao { get; set; }
    public string RegraAplicada { get; set; } = string.Empty;
}
