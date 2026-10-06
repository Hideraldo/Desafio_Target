namespace Movimentacoes.Models;

public class MovimentacaoResponse
{
    /// <summary>
    /// Número identificador único da movimentação.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Data e hora do lançamento da movimentação.
    /// </summary>
    public DateTime DataHora { get; set; }

    /// <summary>
    /// Código do produto movimentado.
    /// </summary>
    public int CodigoProduto { get; set; }

    /// <summary>
    /// Descrição do produto movimentado.
    /// </summary>
    public string DescricaoProduto { get; set; } = string.Empty;

    /// <summary>
    /// Tipo da movimentação (Entrada ou Saida).
    /// </summary>
    public TipoMovimentacao Tipo { get; set; }

    /// <summary>
    /// Descrição para identificar o tipo da movimentação realizada.
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Quantidade de mercadoria movimentada.
    /// </summary>
    public int QuantidadeMovimentada { get; set; }

    /// <summary>
    /// Quantidade de estoque antes da movimentação.
    /// </summary>
    public int EstoqueAnterior { get; set; }

    /// <summary>
    /// Quantidade final do estoque do produto movimentado.
    /// </summary>
    public int EstoqueFinal { get; set; }

    /// <summary>
    /// Mensagem explicativa do resultado da operação.
    /// </summary>
    public string Mensagem { get; set; } = string.Empty;
}
