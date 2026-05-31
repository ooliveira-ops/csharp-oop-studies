# Modificadores de Acesso e readonly — public, private, readonly

---

## 🔗 Analogia

Pense num prédio comercial. A recepção é **pública** — qualquer um entra.  
A sala do servidor é **privada** — só funcionários autorizados acessam.  
O endereço gravado na placa da entrada é **readonly** — está fixado, ninguém muda depois que foi instalado.

---

## 📖 Conceito

`public` significa que qualquer código de qualquer lugar pode acessar aquele membro.  
`private` significa que só a própria classe pode acessar — protege o estado interno.  
`readonly` significa que o campo só pode ser atribuído uma vez: na declaração ou no construtor.  
Juntos, eles controlam **quem acessa** e **quem pode modificar** — base de um código seguro e previsível.

---

## 💻 Código

```csharp
public class PessoaService : IPessoaService
{
    // private readonly — padrão ouro para dependências injetadas
    // private: só esta classe usa o contexto
    // readonly: garante que ninguém reatribui _context depois do construtor
    private readonly AppDbContext _context;
    private readonly ILogger<PessoaService> _logger;

    // O construtor é público — o framework precisa instanciar a classe
    public PessoaService(AppDbContext context, ILogger<PessoaService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // public — faz parte do contrato IPessoaService, precisa ser acessível
    public async Task<IEnumerable<Pessoa>> ObterTodosAsync()
    {
        _logger.LogInformation("Buscando todas as pessoas.");
        return await BuscarComFiltroAsync(p => true); // chama método privado interno
    }

    public async Task<Pessoa?> ObterPorIdAsync(int id)
    {
        return await BuscarComFiltroAsync(p => p.Id == id)
            .ContinueWith(t => t.Result.FirstOrDefault());
    }

    public async Task CriarAsync(CriarPessoaDto dto)
    {
        ValidarDto(dto);   // método privado — detalhe de implementação
        var pessoa = MapearEntidade(dto); // método privado
        _context.Pessoas.Add(pessoa);
        await _context.SaveChangesAsync();
    }

    // private — lógica interna, não faz parte da interface pública do serviço
    private void ValidarDto(CriarPessoaDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new ArgumentException("Nome é obrigatório.");
        if (dto.Idade < 0)
            throw new ArgumentException("Idade inválida.");
    }

    private Pessoa MapearEntidade(CriarPessoaDto dto) =>
        new Pessoa { Nome = dto.Nome, Cpf = dto.Cpf, Idade = dto.Idade };

    private async Task<IEnumerable<Pessoa>> BuscarComFiltroAsync(
        System.Linq.Expressions.Expression<Func<Pessoa, bool>> filtro)
    {
        return await _context.Pessoas.Where(filtro).ToListAsync();
    }
}


// Exemplos em entidades
public class Pessoa
{
    public int Id { get; set; }           // public: EF e controllers precisam ler/escrever

    public string Nome { get; set; }

    public string Cpf { get; set; }

    // private set: qualquer um lê, mas só a classe altera
    public DateTime DataCadastro { get; private set; } = DateTime.UtcNow;

    // readonly em campo: valor fixado na criação do objeto
    // (menos comum em entidades, mais comum em services/configs)
}


// readonly em constantes de configuração
public class EmailService
{
    private readonly string _remetente;
    private readonly int _porta;

    public EmailService(IConfiguration config)
    {
        // atribuídos no construtor — readonly garante que não mudam depois
        _remetente = config["Email:Remetente"];
        _porta = int.Parse(config["Email:Porta"]);
    }
}
```

---

## 🔍 Linha por Linha

1. `private readonly AppDbContext _context` — `private`: só `PessoaService` acessa; `readonly`: não pode ser reatribuído após o construtor.
2. `public PessoaService(AppDbContext context)` — construtor `public` para o framework conseguir instanciar via injeção de dependência.
3. `_context = context` — única atribuição permitida pelo `readonly`; qualquer outra tentativa fora do construtor gera erro de compilação.
4. `public async Task<...> ObterTodosAsync()` — `public` porque implementa a interface; qualquer um pode chamar.
5. `private void ValidarDto(...)` — `private` porque é detalhe interno; quem usa `IPessoaService` não precisa saber que existe.
6. `public DateTime DataCadastro { get; private set; }` — `get` público (qualquer um lê) + `set` privado (só a classe escreve).
7. `private readonly string _remetente` — configuração que não muda durante a vida do objeto; `readonly` previne bugs de reatribuição acidental.

---

## 🏗️ Na Sua API

Regras práticas para cada camada:

```
Controllers
├── public  → action methods (GET, POST, PUT, DELETE)
└── private → métodos auxiliares internos (MapearDto, ValidarId...)

Services
├── public  → métodos da interface (ObterTodosAsync, CriarAsync...)
├── private → lógica interna (ValidarDto, MapearEntidade...)
└── private readonly → dependências injetadas (_context, _logger)

Entidades
├── public  → propriedades mapeadas pelo EF
└── private set → campos que só a entidade deve controlar (DataCadastro, etc.)
```

---

## 💡 Dica de Ouro

**`private readonly`** é o padrão para qualquer dependência injetada — use sempre.  
`readonly` transforma erros de runtime em erros de compilação: se você tentar reatribuir `_context` em outro lugar, o compilador avisa na hora, não em produção.  
E quanto mais `private` você usar, mais fácil fica refatorar: o que é privado você muda sem medo de quebrar código externo.
