# Injeção de Dependências (Dependency Injection)

---

## 🔗 Analogia

Imagine que você vai a um restaurante. Você não precisa saber **como** o chef prepara o prato — só pede e recebe.  
A cozinha (dependência) é "injetada" no garçom (sua classe) pelo restaurante (o framework).  
Você consome o serviço sem se preocupar em criá-lo.

---

## 📖 Conceito

Injeção de Dependências é um padrão onde uma classe **recebe** seus objetos necessários em vez de criá-los internamente.  
No .NET, o `IServiceCollection` registra os serviços e o framework os entrega automaticamente via construtor.  
Isso desacopla as classes, facilita testes e evita `new` espalhados pelo código.  
É o coração do ASP.NET Core — controllers, repositórios e serviços funcionam assim.

---

## 💻 Código

```csharp
// 1. Interface — define o contrato
public interface IPessoaService
{
    IEnumerable<Pessoa> ObterTodos();
    Pessoa ObterPorId(int id);
    void Criar(CriarPessoaDto dto);
}

// 2. Implementação concreta
public class PessoaService : IPessoaService
{
    private readonly AppDbContext _context;

    // Dependência recebida pelo construtor — não criada aqui
    public PessoaService(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Pessoa> ObterTodos() => _context.Pessoas.ToList();

    public Pessoa ObterPorId(int id) => _context.Pessoas.Find(id);

    public void Criar(CriarPessoaDto dto)
    {
        var pessoa = new Pessoa { Nome = dto.Nome, Cpf = dto.Cpf, Idade = dto.Idade };
        _context.Pessoas.Add(pessoa);
        _context.SaveChanges();
    }
}

// 3. Registro no Program.cs
builder.Services.AddScoped<IPessoaService, PessoaService>();

// 4. Controller recebe o serviço automaticamente
[ApiController]
[Route("api/[controller]")]
public class PessoasController : ControllerBase
{
    private readonly IPessoaService _pessoaService;

    public PessoasController(IPessoaService pessoaService)
    {
        _pessoaService = pessoaService;
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_pessoaService.ObterTodos());

    [HttpGet("{id}")]
    public IActionResult GetById(int id) => Ok(_pessoaService.ObterPorId(id));

    [HttpPost]
    public IActionResult Create(CriarPessoaDto dto)
    {
        _pessoaService.Criar(dto);
        return Created();
    }
}
```

---

## 🔍 Linha por Linha

1. `public interface IPessoaService` — contrato que define **o que** o serviço faz, sem dizer **como**.
2. `public class PessoaService : IPessoaService` — implementação concreta que sabe o "como".
3. `private readonly AppDbContext _context;` — campo privado que guarda a dependência recebida.
4. `public PessoaService(AppDbContext context)` — construtor recebe o contexto; o framework injeta automaticamente.
5. `_context = context;` — armazena a referência para uso nos métodos.
6. `builder.Services.AddScoped<IPessoaService, PessoaService>();` — registra: "quando alguém pedir `IPessoaService`, entregue `PessoaService`".
7. `public PessoasController(IPessoaService pessoaService)` — o ASP.NET Core vê o construtor e injeta o serviço registrado.
8. `_pessoaService.ObterTodos()` — usa a dependência sem saber como ela foi criada.

---

## 🏗️ Na Sua API

Estrutura recomendada para a API de Pessoas e Endereço:

```
Program.cs
│
├── builder.Services.AddScoped<IPessoaService, PessoaService>();
├── builder.Services.AddScoped<IEnderecoService, EnderecoService>();
└── builder.Services.AddDbContext<AppDbContext>(...);

Controllers/
├── PessoasController   → recebe IPessoaService
└── EnderecosController → recebe IEnderecoService

Services/
├── IPessoaService + PessoaService
└── IEnderecoService + EnderecoService
```

Cada camada depende da **interface**, não da implementação — isso é desacoplamento.

---

## 💡 Dica de Ouro

Sempre programe para **interfaces** (`IPessoaService`), não para classes concretas (`PessoaService`).  
Isso permite trocar a implementação no futuro (ex: de banco real para mock em testes) sem alterar nenhum controller.  
E use `AddScoped` para serviços de API — ele cria uma instância por requisição HTTP, que é o comportamento correto para operações de banco de dados.
