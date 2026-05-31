# Estrutura de uma API

## 🔗 Analogia

Pensa numa API como um **restaurante**: o cliente (frontend/consumidor) faz um pedido ao garçom (rota/endpoint), que leva ao cozinheiro (serviço/lógica), que busca os ingredientes na despensa (banco de dados) e retorna o prato pronto.

---

## 📖 Conceito

Uma API REST em .NET é organizada em camadas com responsabilidades separadas: **Controller** (recebe a requisição HTTP), **Service** (aplica a lógica de negócio), **Repository** (acessa o banco) e **Model/DTO** (representa os dados). Cada camada fala apenas com a próxima, nunca pulando etapas.

---

## 💻 Código

```csharp
// 1. MODEL — representa a entidade no banco
public class Pessoa
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public Endereco Endereco { get; set; }
}

// 2. DTO — o que o cliente envia/recebe (sem expor o Model direto)
public class PessoaDto
{
    public string Nome { get; set; }
    public string Rua { get; set; }
    public string Cidade { get; set; }
}

// 3. REPOSITORY — acessa o banco via Entity Framework
public class PessoaRepository
{
    private readonly AppDbContext _context;
    public PessoaRepository(AppDbContext context) => _context = context;

    public async Task<List<Pessoa>> GetAllAsync()
        => await _context.Pessoas.Include(p => p.Endereco).ToListAsync();
}

// 4. SERVICE — lógica de negócio
public class PessoaService
{
    private readonly PessoaRepository _repo;
    public PessoaService(PessoaRepository repo) => _repo = repo;

    public async Task<List<Pessoa>> ListarTodas()
        => await _repo.GetAllAsync();
}

// 5. CONTROLLER — ponto de entrada HTTP
[ApiController]
[Route("api/[controller]")]
public class PessoasController : ControllerBase
{
    private readonly PessoaService _service;
    public PessoasController(PessoaService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var pessoas = await _service.ListarTodas();
        return Ok(pessoas);
    }
}
```

---

## 🔍 Linha por Linha

**1. Model (`Pessoa`)**
- Define como o dado existe no banco de dados — com `Id`, `Nome` e a relação com `Endereco`.

**2. DTO (`PessoaDto`)**
- É o "formulário" que o cliente preenche. Você controla o que entra e sai, sem expor campos internos do Model.

**3. Repository (`PessoaRepository`)**
- Única classe que fala com o banco. O `.Include(p => p.Endereco)` carrega o endereço junto com a pessoa (JOIN automático).

**4. Service (`PessoaService`)**
- Onde ficaria validação, regras de negócio (ex: "não pode cadastrar CPF duplicado"). Chama o Repository, nunca o banco diretamente.

**5. Controller (`PessoasController`)**
- `[HttpGet]` mapeia `GET /api/pessoas`. Chama o Service e devolve `200 OK` com os dados.

---

## 🏗️ Na Sua API

O fluxo completo para **listar pessoas com endereço**:

```
GET /api/pessoas
    → PessoasController.GetAll()
    → PessoaService.ListarTodas()
    → PessoaRepository.GetAllAsync()
    → banco de dados
    ← retorna List<Pessoa>
    ← Ok(pessoas) → JSON para o cliente
```

Crie uma pasta para cada camada: `Models/`, `DTOs/`, `Repositories/`, `Services/`, `Controllers/`.

---

## 💡 Dica de Ouro

**Nunca deixe o Controller tocar no banco diretamente.** Se você está injetando o `DbContext` no Controller, alguma coisa está errada na arquitetura — mova essa lógica para um Service ou Repository. Isso facilita testes e manutenção futura.
