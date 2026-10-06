using System.ComponentModel.DataAnnotations;

namespace Movimentacoes.Models;

public class MovimentacaoRequest
{
    /// <summary>
    /// Número identificador único da movimentação (opcional: se não informado ou zero, será gerado automaticamente).
    /// </summary>
    public long? Id { get; set; }

    /// <summary>
    /// Código do produto a ser movimentado (ex: 101, 102, 103, 104, 105).
    /// </summary>
    [Required(ErrorMessage = "O código do produto é obrigatório.")]
    public int CodigoProduto { get; set; }

    /// <summary>
    /// Tipo da movimentação: Entrada ou Saida.
    /// </summary>
    [Required(ErrorMessage = "O tipo da movimentação é obrigatório (Entrada ou Saida).")]
    public TipoMovimentacao Tipo { get; set; }

    /// <summary>
    /// Quantidade de mercadoria a ser movimentada (deve ser maior que zero).
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "A quantidade movimentada deve ser maior que zero.")]
    public int Quantidade { get; set; }

    /// <summary>
    /// Descrição para identificar o tipo da movimentação realizada.
    /// </summary>
    [Required(ErrorMessage = "A descrição para identificar o tipo da movimentação realizada é obrigatória.")]
    public string Descricao { get; set; } = string.Empty;
}
