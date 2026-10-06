using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Movimentacoes.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuração dos Controllers e serialização JSON (aceitando strings para os Enums)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API de Movimentações de Estoque",
        Version = "v1",
        Description = "API para controle de entradas e saídas de mercadorias no depósito:\n" +
                      "- Identificador único para cada movimentação\n" +
                      "- Descrição para identificar o tipo da movimentação realizada\n" +
                      "- Retorno da quantidade final em estoque do produto movimentado\n" +
                      "- Validação de saldo suficiente para saídas"
    });
});

// Injeção de dependência do serviço de estoque em memória
builder.Services.AddSingleton<IEstoqueService, EstoqueService>();

var app = builder.Build();

// Habilita o Swagger para facilitar os testes da API
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Movimentações v1");
    c.RoutePrefix = "swagger";
});

// Redireciona a raiz para o Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
