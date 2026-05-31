# Entity Framework Core (EF Core)

---

## 🔗 Analogia

Imagine um tradutor simultâneo numa reunião entre brasileiros e japoneses.  
Você fala em C# ("me dê todas as pessoas ativas"), o tradutor (EF Core) converte para SQL (`SELECT * FROM Pessoas WHERE Ativo = 1`) e traz o resultado de volta já em objetos C#.  
Você nunca precisou escrever SQL — o EF fez a ponte.

---

## 📖 Conceito

Entity Framework Core é um ORM (Object-Relational Mapper) — ele mapeia classes C# para tabelas do banco e vice-versa.  
O `DbContext` é a porta de entrada: representa a conexão e expõe `DbSet<T>` para cada tabela.  
Com **Migrations**, o EF gera e aplica automaticamente o SQL para criar/alterar tabelas a partir das suas classes.  
Você escreve C# puro; o EF traduz para SQL no banco configurado (SQL Server, PostgreSQL, SQLite, etc.).

---

## 💻 Código

```csharp
// 1. Entidades — classes que viram tabelas
public class Pessoa
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Cpf { get; set; }
    public int Idade { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
    public Endereco Endereco { get; set; } // navegação 1:1
}

public class Endereco
{
    public int Id { get; set; }
    public string Logradouro { get; set; }
    public string Numero { get; set; }
    public string? Complemento { get; set; }
    public string Cep { get; set; }
    public string Cidade { get; set; }
    public int PessoaId { get; set; }
    public Pessoa Pessoa { get; set; }
}

// 2. DbContext — representa o banco de dados
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Pessoa> Pessoas { get; set; }
    public DbSet<Endereco> Enderecos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configurações extras via Fluent API
        modelBuilder.Entity<Pessoa>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Nome).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Cpf).IsRequired().HasMaxLength(11);
            entity.HasIndex(p => p.Cpf).IsUnique(); // CPF único no banco
        });

        modelBuilder.Entity<Endereco>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Logradouro).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Cep).IsRequired().HasMaxLength(8);

            entity.HasOne(e => e.Pessoa)
                  .WithOne(p => p.Endereco)
                  .HasForeignKey<Endereco>(e => e.PessoaId);
        });
    }
}

// 3. Registro no Program.cs
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// appsettings.json
// "ConnectionStrings": {
//   "DefaultConnection": "Server=localhost;Database=PessoasDb;Trusted_Connection=True;"
// }

// 4. Operações CRUD no Service
public class PessoaService : IPessoaService
{
    private readonly AppDbContext _context;

    public PessoaService(AppDbContext context) => _context = context;

    // READ — buscar todos
    public async Task<IEnumerable<Pessoa>> ObterTodosAsync() =>
        await _context.Pessoas.Include(p => p.Endereco).ToListAsync();

    // READ — buscar por id
    public async Task<Pessoa?> ObterPorIdAsync(int id) =>
        await _context.Pessoas
            .Include(p => p.Endereco)
            .FirstOrDefaultAsync(p => p.Id == id);

    // CREATE
    public async Task<Pessoa> CriarAsync(CriarPessoaDto dto)
    {
        var pessoa = new Pessoa
        {
            Nome = dto.Nome,
            Cpf = dto.Cpf,
            Idade = dto.Idade,
            Ativo = true,
            DataCadastro = DateTime.UtcNow
        };
        _context.Pessoas.Add(pessoa);
        await _context.SaveChangesAsync();
        return pessoa;
    }

    // UPDATE
    public async Task AtualizarAsync(int id, AtualizarPessoaDto dto)
    {
        var pessoa = await _context.Pessoas.FindAsync(id)
            ?? throw new KeyNotFoundException("Pessoa não encontrada.");

        pessoa.Nome = dto.Nome;
        pessoa.Idade = dto.Idade;
        await _context.SaveChangesAsync();
    }

    // DELETE
    public async Task DeletarAsync(int id)
    {
        var pessoa = await _context.Pessoas.FindAsync(id)
            ?? throw new KeyNotFoundException("Pessoa não encontrada.");

        _context.Pessoas.Remove(pessoa);
        await _context.SaveChangesAsync();
    }
}
```

---

## 🔍 Linha por Linha

1. `public DbSet<Pessoa> Pessoas { get; set; }` — representa a tabela `Pessoas`; é por aqui que todas as queries passam.
2. `AppDbContext(DbContextOptions<AppDbContext> options) : base(options)` — repassa as configurações de conexão para a classe base.
3. `entity.Property(p => p.Nome).IsRequired().HasMaxLength(100)` — Fluent API: coluna `NOT NULL` com limite de 100 caracteres.
4. `entity.HasIndex(p => p.Cpf).IsUnique()` — cria índice único no banco; impede CPF duplicado a nível de banco.
5. `options.UseSqlServer(...)` — define o banco de dados; trocar por `UseNpgsql(...)` para PostgreSQL, `UseSqlite(...)` para SQLite.
6. `.Include(p => p.Endereco)` — carrega a navegação junto; sem isso, `Endereco` vem `null`.
7. `_context.Pessoas.Add(pessoa)` — rastreia o objeto como "novo"; só vai ao banco no `SaveChangesAsync`.
8. `await _context.SaveChangesAsync()` — executa o `INSERT`/`UPDATE`/`DELETE` no banco em uma transação.
9. `_context.Pessoas.Remove(pessoa)` — marca o objeto para deleção; efetivado no próximo `SaveChangesAsync`.

---

## 🏗️ Na Sua API

**Migrations** — fluxo para criar e atualizar o banco:

```bash
# Criar uma migration (gera o script SQL a partir das classes)
dotnet ef migrations add CriacaoInicial

# Aplicar ao banco (executa o SQL)
dotnet ef database update

# Se mudar uma entidade (adicionar campo, por exemplo):
dotnet ef migrations add AdicionarCampoAtivo
dotnet ef database update
```

Estrutura gerada na pasta `Migrations/`:
```
Migrations/
├── 20260515_CriacaoInicial.cs       ← SQL de criação das tabelas
├── 20260515_CriacaoInicial.Designer.cs
└── AppDbContextModelSnapshot.cs     ← estado atual do modelo
```

Nunca edite os arquivos de Migration manualmente — sempre gere um novo com `migrations add`.

---

## 💡 Dica de Ouro

**`SaveChangesAsync` é uma transação**: se você fizer `Add` de uma Pessoa e `Add` de um Endereco antes de chamar `SaveChanges`, os dois são salvos juntos — se um falhar, o outro não é salvo.  
Use `FindAsync(id)` para buscar por chave primária — ele verifica o cache do contexto antes de ir ao banco, sendo mais eficiente que `FirstOrDefaultAsync(p => p.Id == id)`.  
E sempre rode `dotnet ef database update` após puxar código novo do repositório — se alguém adicionou uma Migration, seu banco local precisa ser atualizado.
