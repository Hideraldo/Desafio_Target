using System.Text.Json.Serialization;

namespace Movimentacoes.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TipoMovimentacao
{
    Entrada = 1,
    Saida = 2
}
