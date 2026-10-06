using Comissoes.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuração dos serviços
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API de Cálculo de Comissões",
        Version = "v1",
        Description = "API para cálculo de comissões de vendedores com base no valor das vendas:\n" +
                      "- Abaixo de R$ 100,00: 0% de comissão\n" +
                      "- Abaixo de R$ 500,00: 1% de comissão\n" +
                      "- A partir de R$ 500,00: 5% de comissão"
    });
});

// Injeção de dependência das regras de negócio
builder.Services.AddScoped<IComissaoService, ComissaoService>();

var app = builder.Build();

// Habilita o Swagger para facilitar os testes da API
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Comissões v1");
    c.RoutePrefix = "swagger";
});

// Redireciona a raiz para o Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
