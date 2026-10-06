using System.Text.Json.Serialization;

namespace Comissoes.Models;

public class VendasRequest
{
    [JsonPropertyName("vendas")]
    public List<VendaItem> Vendas { get; set; } = new();
}
