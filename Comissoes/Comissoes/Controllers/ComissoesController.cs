using Microsoft.AspNetCore.Mvc;
using Comissoes.Models;
using Comissoes.Services;

namespace Comissoes.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ComissoesController : ControllerBase
{
    private readonly IComissaoService _comissaoService;

    public ComissoesController(IComissaoService comissaoService)
    {
        _comissaoService = comissaoService;
    }

    /// <summary>
    /// Calcula a comissão de cada vendedor a partir da lista de vendas enviada no corpo da requisição.
    /// </summary>
    /// <remarks>
    /// Regras de comissão por venda:
    /// - Abaixo de R$ 100,00: 0% de comissão
    /// - Abaixo de R$ 500,00: 1% de comissão
    /// - A partir de R$ 500,00: 5% de comissão
    /// </remarks>
    /// <param name="request">Lista de vendas com vendedor e valor</param>
    /// <returns>Cálculo consolidado e detalhado por vendedor</returns>
    [HttpPost("calcular")]
    [ProducesResponseType(typeof(CalculoComissaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<CalculoComissaoResponse> Calcular([FromBody] VendasRequest request)
    {
        if (request == null || request.Vendas == null || request.Vendas.Count == 0)
        {
            return BadRequest(new { mensagem = "A lista de vendas não pode estar vazia." });
        }

        var resultado = _comissaoService.CalcularComissoes(request);
        return Ok(resultado);
    }

    /// <summary>
    /// Executa o cálculo com os dados de vendas de exemplo do desafio.
    /// </summary>
    /// <returns>Resultado do cálculo com a equipe comercial de exemplo</returns>
    [HttpGet("exemplo")]
    [ProducesResponseType(typeof(CalculoComissaoResponse), StatusCodes.Status200OK)]
    public ActionResult<CalculoComissaoResponse> ObterCalculoExemplo()
    {
        var dadosExemplo = _comissaoService.ObterDadosExemplo();
        var resultado = _comissaoService.CalcularComissoes(dadosExemplo);
        return Ok(resultado);
    }

    /// <summary>
    /// Retorna o payload JSON de vendas de exemplo para uso em testes no endpoint POST.
    /// </summary>
    /// <returns>JSON contendo as vendas de exemplo do time comercial</returns>
    [HttpGet("dados-exemplo")]
    [ProducesResponseType(typeof(VendasRequest), StatusCodes.Status200OK)]
    public ActionResult<VendasRequest> ObterPayloadExemplo()
    {
        return Ok(_comissaoService.ObterDadosExemplo());
    }
}
