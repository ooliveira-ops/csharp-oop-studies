# Métodos Assíncronos (async / await)

---

## 🔗 Analogia

Você pede um café no balcão e, enquanto o barista prepara, você já senta e abre o notebook.  
Você não ficou parado esperando — fez outras coisas e só pegou o café quando ficou pronto.  
`async/await` é exatamente isso: o programa não trava esperando uma operação lenta (banco, rede).

---

## 📖 Conceito

`async` marca um método como assíncrono — ele pode ter pontos de espera sem bloquear a thread.  
`await` pausa a execução **daquele método** até a tarefa terminar, mas libera a thread para outros trabalhos.  
O retorno de um método async é sempre `Task` (sem valor) ou `Task<T>` (com valor).  
Em APIs, isso é essencial: sem async, cada requisição lenta travaria uma thread do servidor.

---

## 💻 Código

```csharp
// Interface com métodos assíncronos
public interface IPessoaService
{
    Task<IEnumerable<Pessoa>> ObterTodosAsync();
    Task<Pessoa?> ObterPorIdAsync(int id);
    Task CriarAsync(CriarPessoaDto dto);
}

// Implementação usando Entity Framework (já é async nativamente)
public class PessoaService : IPessoaService
{
    private readonly AppDbContext _context;

    public PessoaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Pessoa>> ObterTodosAsync()
    {
        return await _context.Pessoas.ToListAsync();
    }

    public async Task<Pessoa?> ObterPorIdAsync(int id)
    {
        return await _context.Pessoas.FindAsync(id);
    }

    public async Task CriarAsync(CriarPessoaDto dto)
    {
        var pessoa = new Pessoa { Nome = dto.Nome, Cpf = dto.Cpf, Idade = dto.Idade };
        _context.Pessoas.Add(pessoa);
        await _context.SaveChangesAsync();
    }
}

// Controller também async
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
    public async Task<IActionResult> GetAll()
    {
        var pessoas = await _pessoaService.ObterTodosAsync();
        return Ok(pessoas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var pessoa = await _pessoaService.ObterPorIdAsync(id);
        if (pessoa is null) return NotFound();
        return Ok(pessoa);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CriarPessoaDto dto)
    {
        await _pessoaService.CriarAsync(dto);
        return Created();
    }
}
```

---

## 🔍 Linha por Linha

1. `Task<IEnumerable<Pessoa>>` — retorno async com valor; `Task` é o "envelope" da operação assíncrona.
2. `async Task<IEnumerable<Pessoa>> ObterTodosAsync()` — `async` habilita o uso de `await` dentro do método.
3. `return await _context.Pessoas.ToListAsync();` — `await` aguarda o banco retornar sem travar a thread.
4. `Task CriarAsync(...)` — `Task` sem tipo quando o método não retorna valor (equivalente a `void` async).
5. `await _context.SaveChangesAsync();` — persiste no banco de forma assíncrona.
6. `public async Task<IActionResult> GetAll()` — o controller também precisa ser async para usar `await`.
7. `var pessoas = await _pessoaService.ObterTodosAsync();` — aguarda o serviço, depois monta a resposta.
8. `if (pessoa is null) return NotFound();` — trata o caso de não encontrar o registro antes de retornar.

---

## 🏗️ Na Sua API

Regra prática: **se o método faz acesso a banco ou rede, ele deve ser async**.

```
Controller  →  await service.ObterTodosAsync()
Service     →  await _context.Pessoas.ToListAsync()
DbContext   →  Entity Framework já fornece versões Async de tudo
```

Convenção de nomenclatura: sempre termine com `Async` — `ObterPorIdAsync`, `CriarAsync`, `DeletarAsync`.  
Isso sinaliza para quem usa o método que ele deve ser aguardado com `await`.

---

## 💡 Dica de Ouro

**Nunca misture sync e async** — não chame `.Result` ou `.Wait()` num método async, isso causa deadlock.  
Se um método usa `await`, todo o caminho acima dele também deve ser `async`.  
E prefira `async Task` a `async void` — `void` engole exceções silenciosamente e é impossível de aguardar.
