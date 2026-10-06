using Microsoft.AspNetCore.Mvc;
using Movimentacoes.Models;
using Movimentacoes.Services;

namespace Movimentacoes.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MovimentacoesController : ControllerBase
{
    private readonly IEstoqueService _estoqueService;

    public MovimentacoesController(IEstoqueService estoqueService)
    {
        _estoqueService = estoqueService;
    }

    /// <summary>
    /// Lança uma movimentação de estoque (entrada ou saída) para um produto do depósito.
    /// </summary>
    /// <remarks>
    /// Exemplo de requisição para Entrada:
    /// 
    ///     POST /api/movimentacoes
    ///     {
    ///        "id": 1,
    ///        "codigoProduto": 101,
    ///        "tipo": "Entrada",
    ///        "quantidade": 50,
    ///        "descricao": "Entrada por compra de fornecedor"
    ///     }
    /// 
    /// Exemplo de requisição para Saída:
    /// 
    ///     POST /api/movimentacoes
    ///     {
    ///        "id": 2,
    ///        "codigoProduto": 102,
    ///        "tipo": "Saida",
    ///        "quantidade": 15,
    ///        "descricao": "Saída por venda ao cliente"
    ///     }
    /// </remarks>
    /// <param name="request">Dados da movimentação a ser lançada</param>
    /// <returns>Resultado da movimentação com identificador único, descrição e quantidade final em estoque</returns>
    [HttpPost]
    [ProducesResponseType(typeof(MovimentacaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<MovimentacaoResponse> Lancar([FromBody] MovimentacaoRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var resultado = _estoqueService.LancarMovimentacao(request);
            return Ok(resultado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Consulta todos os produtos cadastrados e a quantidade atual em estoque.
    /// </summary>
    /// <returns>Lista dos produtos e seus estoques</returns>
    [HttpGet("produtos")]
    [ProducesResponseType(typeof(IEnumerable<Produto>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Produto>> ObterProdutos()
    {
        var produtos = _estoqueService.ObterTodosOsProdutos();
        return Ok(produtos);
    }

    /// <summary>
    /// Consulta um produto específico pelo código para verificar sua quantidade atual em estoque.
    /// </summary>
    /// <param name="codigoProduto">Código do produto (ex: 101, 102, 103, 104, 105)</param>
    /// <returns>Dados do produto e estoque atual</returns>
    [HttpGet("produtos/{codigoProduto:int}")]
    [ProducesResponseType(typeof(Produto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Produto> ObterProdutoPorCodigo(int codigoProduto)
    {
        var produto = _estoqueService.ObterProdutoPorCodigo(codigoProduto);
        if (produto == null)
        {
            return NotFound(new { mensagem = $"Produto com código {codigoProduto} não encontrado." });
        }

        return Ok(produto);
    }

    /// <summary>
    /// Consulta o histórico completo de movimentações de estoque realizadas no depósito.
    /// </summary>
    /// <returns>Histórico de movimentações ordenadas da mais recente para a mais antiga</returns>
    [HttpGet("historico")]
    [ProducesResponseType(typeof(IEnumerable<MovimentacaoResponse>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<MovimentacaoResponse>> ObterHistorico()
    {
        var historico = _estoqueService.ObterHistorico();
        return Ok(historico);
    }

    /// <summary>
    /// Restaura o estoque de todos os produtos para a quantidade original do desafio.
    /// </summary>
    /// <returns>Mensagem de confirmação</returns>
    [HttpPost("resetar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult ResetarEstoque()
    {
        _estoqueService.ResetarEstoque();
        return Ok(new { mensagem = "Estoque restaurado para os valores iniciais com sucesso." });
    }
}
