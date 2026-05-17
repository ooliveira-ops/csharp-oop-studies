# Swagger (OpenAPI)

---

## 🔗 Analogia

Imagine um cardápio interativo de restaurante onde além de ver os pratos, você pode **fazer o pedido ali mesmo** e ver o resultado na hora.  
Swagger é esse cardápio para a sua API: documenta todos os endpoints e permite testá-los direto no navegador, sem precisar de Postman ou curl.

---

## 📖 Conceito

Swagger é uma interface visual gerada automaticamente a partir dos seus controllers e DTOs.  
No .NET, o pacote `Swashbuckle` lê os atributos (`[HttpGet]`, `[HttpPost]`, etc.) e gera a documentação em formato OpenAPI.  
Você acessa em `/swagger` enquanto a API está rodando e vê todos os endpoints, parâmetros e modelos.  
É a ferramenta principal de teste e documentação durante o desenvolvimento de uma API.

---

## 💻 Código

```csharp
// ── Program.cs — configuração básica (já vem no template) ─
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API de Pessoas e Endereços",
        Version = "v1",
        Description = "API para gerenciamento de pessoas e seus endereços."
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Pessoas API v1");
        options.RoutePrefix = string.Empty; // abre Swagger na raiz "/"
    });
}

app.UseAuthorization();
app.MapControllers();
app.Run();


// ── Anotações no Controller — melhoram a documentação ─────
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PessoasController : ControllerBase
{
    private readonly IPessoaService _pessoaService;

    public PessoasController(IPessoaService pessoaService)
    {
        _pessoaService = pessoaService;
    }

    /// <summary>Lista todas as pessoas cadastradas.</summary>
    /// <response code="200">Retorna a lista de pessoas.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PessoaResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var pessoas = await _pessoaService.ObterTodosAsync();
        return Ok(ApiResponse<IEnumerable<PessoaResponseDto>>.Ok(pessoas));
    }

    /// <summary>Busca uma pessoa pelo ID.</summary>
    /// <param name="id">ID da pessoa.</param>
    /// <response code="200">Pessoa encontrada.</response>
    /// <response code="404">Pessoa não encontrada.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<PessoaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PessoaResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var pessoa = await _pessoaService.ObterPorIdAsync(id);
        if (pessoa is null)
            return NotFound(ApiResponse<PessoaResponseDto>.Erro("Pessoa não encontrada."));
        return Ok(ApiResponse<PessoaResponseDto>.Ok(pessoa));
    }

    /// <summary>Cadastra uma nova pessoa.</summary>
    /// <response code="201">Pessoa criada com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PessoaResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CriarPessoaDto dto)
    {
        var criada = await _pessoaService.CriarAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = criada.Id },
            ApiResponse<PessoaResponseDto>.Ok(criada));
    }

    /// <summary>Atualiza os dados de uma pessoa.</summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, AtualizarPessoaDto dto)
    {
        await _pessoaService.AtualizarAsync(id, dto);
        return NoContent();
    }

    /// <summary>Remove uma pessoa pelo ID.</summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _pessoaService.DeletarAsync(id);
        return NoContent();
    }
}


// ── Anotações no DTO — aparecem como exemplo no Swagger ───
public class CriarPessoaDto
{
    /// <example>Ana Silva</example>
    public string Nome { get; set; }

    /// <example>12345678900</example>
    public string Cpf { get; set; }

    /// <example>28</example>
    public int Idade { get; set; }
}
```

---

## 🔍 Linha por Linha

1. `AddSwaggerGen(...)` — registra o gerador de documentação OpenAPI no container de DI.
2. `new OpenApiInfo { Title, Version, Description }` — aparece no topo da página do Swagger.
3. `app.UseSwagger()` — ativa o endpoint `/swagger/v1/swagger.json` (o JSON da documentação).
4. `app.UseSwaggerUI(...)` — ativa a interface visual que consome esse JSON.
5. `options.RoutePrefix = string.Empty` — move o Swagger para a raiz `/` em vez de `/swagger`.
6. `[Produces("application/json")]` — informa ao Swagger que o controller sempre retorna JSON.
7. `/// <summary>...</summary>` — comentário XML que vira a descrição do endpoint no Swagger.
8. `[ProducesResponseType(typeof(...), StatusCode)]` — documenta quais status codes e tipos o endpoint retorna.
9. `/// <example>Ana Silva</example>` — valor de exemplo que aparece no formulário do Swagger.

---

## 🏗️ Na Sua API

Para ativar os comentários XML (summary) no Swagger, adicione no `.csproj`:

```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

E no `Program.cs`, diga ao Swagger para usar o XML gerado:

```csharp
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Pessoas API", Version = "v1" });

    // Inclui os comentários XML na documentação
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});
```

Resultado no navegador em `https://localhost:{porta}/`:
```
GET    /api/pessoas          → Lista todas as pessoas
GET    /api/pessoas/{id}     → Busca por ID
POST   /api/pessoas          → Cadastra nova pessoa
PUT    /api/pessoas/{id}     → Atualiza pessoa
DELETE /api/pessoas/{id}     → Remove pessoa
```

---

## 💡 Dica de Ouro

Use `[ProducesResponseType]` em todos os endpoints — além de documentar, isso ajuda o Swagger a gerar exemplos corretos para cada status code.  
E configure `options.RoutePrefix = string.Empty` para abrir o Swagger na raiz: quando alguém acessar `https://localhost:5000/` durante o desenvolvimento, cai direto na documentação — muito mais prático do que navegar para `/swagger`.
