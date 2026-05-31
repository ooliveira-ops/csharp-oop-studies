# Relacionamento 1 para 1 (One-to-One) — Entity Framework

---

## 🔗 Analogia

Cada pessoa tem **um** passaporte, e cada passaporte pertence a **uma** pessoa.  
Nunca dois passaportes para a mesma pessoa, nunca um passaporte sem dono.  
Isso é 1:1 — duas tabelas ligadas onde cada linha de um lado corresponde a exatamente uma linha do outro.

---

## 📖 Conceito

No relacionamento 1:1, uma entidade se associa a **no máximo uma** outra entidade.  
No Entity Framework, isso é configurado com uma propriedade de navegação em ambos os lados e uma chave estrangeira em um deles.  
Na sua API, `Pessoa` pode ter um `Endereco` principal — cada pessoa tem um, cada endereço pertence a uma pessoa.  
O EF Core cria automaticamente a `FK` e a `UNIQUE CONSTRAINT` para garantir o vínculo no banco.

---

## 💻 Código

```csharp
// Entidade Pessoa
public class Pessoa
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Cpf { get; set; }
    public int Idade { get; set; }

    // Navegação para o lado "um" do relacionamento
    public Endereco Endereco { get; set; }
}

// Entidade Endereco
public class Endereco
{
    public int Id { get; set; }
    public string Logradouro { get; set; }
    public string Numero { get; set; }
    public string Cep { get; set; }
    public string Cidade { get; set; }

    // Chave estrangeira — Endereco guarda a referência para Pessoa
    public int PessoaId { get; set; }

    // Navegação de volta para Pessoa
    public Pessoa Pessoa { get; set; }
}

// DbContext — configuração via Fluent API
public class AppDbContext : DbContext
{
    public DbSet<Pessoa> Pessoas { get; set; }
    public DbSet<Endereco> Enderecos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pessoa>()
            .HasOne(p => p.Endereco)        // Pessoa tem um Endereco
            .WithOne(e => e.Pessoa)         // Endereco pertence a uma Pessoa
            .HasForeignKey<Endereco>(e => e.PessoaId); // FK fica em Endereco
    }
}

// Service — criar pessoa com endereço
public async Task<PessoaResponseDto> CriarComEnderecoAsync(CriarPessoaComEnderecoDto dto)
{
    var pessoa = new Pessoa
    {
        Nome = dto.Nome,
        Cpf = dto.Cpf,
        Idade = dto.Idade,
        Endereco = new Endereco
        {
            Logradouro = dto.Endereco.Logradouro,
            Numero = dto.Endereco.Numero,
            Cep = dto.Endereco.Cep,
            Cidade = dto.Endereco.Cidade
        }
    };

    _context.Pessoas.Add(pessoa);
    await _context.SaveChangesAsync(); // salva os dois em uma transação

    return new PessoaResponseDto
    {
        Id = pessoa.Id,
        Nome = pessoa.Nome,
        Endereco = new EnderecoResponseDto
        {
            Logradouro = pessoa.Endereco.Logradouro,
            Cidade = pessoa.Endereco.Cidade
        }
    };
}

// Service — buscar pessoa com endereço (Include para carregar a navegação)
public async Task<Pessoa?> ObterComEnderecoAsync(int id)
{
    return await _context.Pessoas
        .Include(p => p.Endereco)
        .FirstOrDefaultAsync(p => p.Id == id);
}
```

---

## 🔍 Linha por Linha

1. `public Endereco Endereco { get; set; }` em `Pessoa` — propriedade de navegação; o EF usa isso para montar o JOIN.
2. `public int PessoaId { get; set; }` em `Endereco` — chave estrangeira; a coluna real no banco.
3. `public Pessoa Pessoa { get; set; }` em `Endereco` — navegação de volta (referência inversa).
4. `.HasOne(p => p.Endereco)` — "Pessoa tem um Endereco".
5. `.WithOne(e => e.Pessoa)` — "e esse Endereco pertence a uma Pessoa".
6. `.HasForeignKey<Endereco>(e => e.PessoaId)` — define em qual tabela a FK fica (sempre no lado dependente).
7. `Endereco = new Endereco { ... }` — cria os dois objetos juntos; o EF resolve a FK automaticamente.
8. `.Include(p => p.Endereco)` — sem isso, `pessoa.Endereco` seria `null` (EF não carrega navegações automaticamente).

---

## 🏗️ Na Sua API

```csharp
// DTOs para criar pessoa com endereço juntos
public class CriarPessoaComEnderecoDto
{
    public string Nome { get; set; }
    public string Cpf { get; set; }
    public int Idade { get; set; }
    public CriarEnderecoDto Endereco { get; set; }
}

public class CriarEnderecoDto
{
    public string Logradouro { get; set; }
    public string Numero { get; set; }
    public string Cep { get; set; }
    public string Cidade { get; set; }
}

// DTO de resposta com endereço aninhado
public class PessoaResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public int Idade { get; set; }
    public EnderecoResponseDto Endereco { get; set; }
}

// Controller
[HttpPost]
public async Task<IActionResult> Create(CriarPessoaComEnderecoDto dto)
{
    var resultado = await _pessoaService.CriarComEnderecoAsync(dto);
    return CreatedAtAction(nameof(GetById), new { id = resultado.Id },
        ApiResponse<PessoaResponseDto>.Ok(resultado));
}

[HttpGet("{id}")]
public async Task<IActionResult> GetById(int id)
{
    var pessoa = await _pessoaService.ObterComEnderecoAsync(id);
    if (pessoa is null)
        return NotFound(ApiResponse<PessoaResponseDto>.Erro("Pessoa não encontrada."));

    return Ok(ApiResponse<PessoaResponseDto>.Ok(MapearParaDto(pessoa)));
}
```

---

## 💡 Dica de Ouro

**Sempre use `.Include()`** quando precisar dos dados da navegação — sem ele, a propriedade vem `null` e você leva um `NullReferenceException` em runtime.  
A FK (`PessoaId`) fica no lado **dependente** — o que não faz sentido existir sozinho. Endereco sem Pessoa não tem propósito, então a FK fica em `Endereco`.  
Se o endereço for opcional (nem toda pessoa tem), declare `public Endereco? Endereco { get; set; }` e `public int? PessoaId { get; set; }` com `?` para aceitar `null`.
