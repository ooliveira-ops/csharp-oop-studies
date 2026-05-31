# LINQ (Language Integrated Query)

---

## 🔗 Analogia

Pense numa planilha do Excel com filtros, ordenação e fórmulas.  
LINQ é exatamente isso, mas dentro do C# — você filtra, ordena e transforma listas com uma linguagem fluente.  
Sem precisar escrever loops manuais para tudo.

---

## 📖 Conceito

LINQ é um conjunto de métodos que permite consultar e manipular coleções (`List`, `IEnumerable`, `IQueryable`) de forma declarativa.  
Funciona em memória (LINQ to Objects) ou traduzido para SQL pelo Entity Framework (LINQ to Entities).  
Os métodos mais usados: `Where`, `Select`, `FirstOrDefault`, `OrderBy`, `ToList`, `Any`, `Count`.  
Em vez de dizer **como** percorrer a lista, você diz **o que** quer — o LINQ resolve o como.

---

## 💻 Código

```csharp
// Lista de exemplo em memória
var pessoas = new List<Pessoa>
{
    new Pessoa { Id = 1, Nome = "Ana",   Idade = 28, Cidade = "SP" },
    new Pessoa { Id = 2, Nome = "Bruno", Idade = 35, Cidade = "RJ" },
    new Pessoa { Id = 3, Nome = "Carla", Idade = 22, Cidade = "SP" },
    new Pessoa { Id = 4, Nome = "Diego", Idade = 35, Cidade = "MG" },
};

// Where — filtra
var moradoresSP = pessoas.Where(p => p.Cidade == "SP").ToList();
// [ Ana, Carla ]

// Select — transforma/projeta
var nomes = pessoas.Select(p => p.Nome).ToList();
// [ "Ana", "Bruno", "Carla", "Diego" ]

// FirstOrDefault — primeiro que satisfaz, ou null
var pessoa = pessoas.FirstOrDefault(p => p.Id == 2);
// Bruno

// OrderBy / OrderByDescending — ordena
var porIdade = pessoas.OrderBy(p => p.Idade).ToList();
// [ Carla(22), Ana(28), Bruno(35), Diego(35) ]

// Any — existe algum que satisfaz?
bool temMenorDeIdade = pessoas.Any(p => p.Idade < 18); // false

// Count — quantos satisfazem?
int qtd35Anos = pessoas.Count(p => p.Idade == 35); // 2

// Combinando — encadeamento de métodos
var resultado = pessoas
    .Where(p => p.Cidade == "SP")
    .OrderBy(p => p.Nome)
    .Select(p => new { p.Nome, p.Idade })
    .ToList();
// [ { Ana, 28 }, { Carla, 22 } ]


// ---- Com Entity Framework (vira SQL automaticamente) ----
public async Task<IEnumerable<Pessoa>> ObterPorCidadeAsync(string cidade)
{
    return await _context.Pessoas
        .Where(p => p.Cidade == cidade)
        .OrderBy(p => p.Nome)
        .ToListAsync();
}
```

---

## 🔍 Linha por Linha

1. `pessoas.Where(p => p.Cidade == "SP")` — filtra a coleção; `p =>` é uma lambda (cada elemento se chama `p`).
2. `.ToList()` — executa a consulta e materializa o resultado numa `List<T>`.
3. `pessoas.Select(p => p.Nome)` — projeta cada elemento; retorna só o campo `Nome`.
4. `pessoas.FirstOrDefault(p => p.Id == 2)` — retorna o primeiro que bate, ou `null` se não encontrar.
5. `pessoas.OrderBy(p => p.Idade)` — ordena crescente; `OrderByDescending` para decrescente.
6. `pessoas.Any(p => p.Idade < 18)` — retorna `bool`; útil para verificações rápidas sem trazer dados.
7. `pessoas.Count(p => p.Idade == 35)` — conta os que satisfazem a condição.
8. Encadeamento: cada método retorna `IEnumerable<T>`, permitindo chamar outro método na sequência.
9. No EF Core, `.Where().OrderBy().ToListAsync()` é traduzido para `SELECT ... WHERE ... ORDER BY` no banco.

---

## 🏗️ Na Sua API

```csharp
// Buscar endereços de uma pessoa
public async Task<IEnumerable<Endereco>> ObterEnderecosDaPessoaAsync(int pessoaId)
{
    return await _context.Enderecos
        .Where(e => e.PessoaId == pessoaId)
        .OrderBy(e => e.Logradouro)
        .ToListAsync();
}

// Verificar se CPF já existe antes de cadastrar
public async Task<bool> CpfJaCadastradoAsync(string cpf)
{
    return await _context.Pessoas.AnyAsync(p => p.Cpf == cpf);
}

// Retornar só os dados necessários (projetar direto no DTO)
public async Task<IEnumerable<PessoaResumoDto>> ObterResumoAsync()
{
    return await _context.Pessoas
        .Select(p => new PessoaResumoDto { Id = p.Id, Nome = p.Nome })
        .ToListAsync();
}
```

---

## 💡 Dica de Ouro

Use `Select` para projetar **direto no DTO** quando possível — evita trazer colunas desnecessárias do banco.  
Prefira `FirstOrDefaultAsync` a `SingleOrDefaultAsync` em buscas por ID: é mais rápido pois para na primeira linha encontrada.  
E lembre: LINQ no EF Core só vai ao banco quando você chama `.ToList()`, `.FirstOrDefault()`, `.Any()` — antes disso é só uma "receita" ainda não executada.
