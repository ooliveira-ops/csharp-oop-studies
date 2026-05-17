# DTOs (Data Transfer Objects)

---

## 🔗 Analogia

Quando você pede uma pizza, o pizzaiolo não te entrega a cozinha inteira — só a pizza no papelão.  
O DTO é esse papelão: um objeto simples que carrega **só o que o outro lado precisa ver**, sem expor a estrutura interna.

---

## 📖 Conceito

DTO é uma classe simples usada para **transportar dados** entre camadas (ex: da API para o cliente e vice-versa).  
Ele protege sua entidade de banco: o cliente não precisa ver campos como `SenhaHash`, `DataCriacao` ou chaves internas.  
Você cria DTOs diferentes para cada operação: um para criar, outro para atualizar, outro para exibir.  
Sem DTO, qualquer mudança na entidade quebra o contrato da API com quem a consome.

---

## 💻 Código

```csharp
// Entidade — representa a tabela no banco (nunca expor direto)
public class Pessoa
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Cpf { get; set; }
    public int Idade { get; set; }
    public string SenhaHash { get; set; }       // não deve sair na API
    public DateTime DataCadastro { get; set; }  // não deve entrar pela API
    public ICollection<Endereco> Enderecos { get; set; }
}

// DTO de criação — o que o cliente envia no POST
public class CriarPessoaDto
{
    public string Nome { get; set; }
    public string Cpf { get; set; }
    public int Idade { get; set; }
}

// DTO de atualização — o que o cliente envia no PUT
public class AtualizarPessoaDto
{
    public string Nome { get; set; }
    public int Idade { get; set; }
    // CPF não pode ser alterado — não entra no DTO
}

// DTO de resposta — o que a API devolve no GET
public class PessoaResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public int Idade { get; set; }
    public DateTime DataCadastro { get; set; }
    // Sem SenhaHash, sem Cpf completo — só o necessário
}

// Mapeamento no Service (manual, sem biblioteca)
public async Task<PessoaResponseDto> CriarAsync(CriarPessoaDto dto)
{
    var pessoa = new Pessoa
    {
        Nome = dto.Nome,
        Cpf = dto.Cpf,
        Idade = dto.Idade,
        DataCadastro = DateTime.UtcNow
    };

    _context.Pessoas.Add(pessoa);
    await _context.SaveChangesAsync();

    return new PessoaResponseDto
    {
        Id = pessoa.Id,
        Nome = pessoa.Nome,
        Idade = pessoa.Idade,
        DataCadastro = pessoa.DataCadastro
    };
}

// Controller usa apenas DTOs — nunca expõe a entidade
[HttpPost]
public async Task<IActionResult> Create(CriarPessoaDto dto)
{
    var resultado = await _pessoaService.CriarAsync(dto);
    return CreatedAtAction(nameof(GetById), new { id = resultado.Id }, resultado);
}

[HttpGet("{id}")]
public async Task<IActionResult> GetById(int id)
{
    var pessoa = await _pessoaService.ObterPorIdAsync(id);
    if (pessoa is null) return NotFound();
    return Ok(pessoa); // pessoa já é PessoaResponseDto
}
```

---

## 🔍 Linha por Linha

1. `public class CriarPessoaDto` — classe dedicada ao POST; só os campos que o cliente deve enviar.
2. `public class AtualizarPessoaDto` — classe dedicada ao PUT; campos editáveis, sem Id (vem pela URL).
3. `public class PessoaResponseDto` — o que a API devolve; sem dados sensíveis ou internos.
4. `var pessoa = new Pessoa { Nome = dto.Nome, ... }` — converte DTO → Entidade para salvar no banco.
5. `DataCadastro = DateTime.UtcNow` — campo que o servidor preenche, não o cliente.
6. `return new PessoaResponseDto { ... }` — converte Entidade → DTO de resposta antes de retornar.
7. `Create(CriarPessoaDto dto)` — o controller recebe DTO, nunca a entidade diretamente.
8. `CreatedAtAction(nameof(GetById), ...)` — retorna 201 com o header `Location` apontando para o novo recurso.

---

## 🏗️ Na Sua API

Estrutura sugerida de DTOs para Pessoas e Endereço:

```
DTOs/
├── Pessoa/
│   ├── CriarPessoaDto.cs       → POST /api/pessoas
│   ├── AtualizarPessoaDto.cs   → PUT  /api/pessoas/{id}
│   └── PessoaResponseDto.cs    → GET  /api/pessoas
│
└── Endereco/
    ├── CriarEnderecoDto.cs      → POST /api/enderecos
    ├── AtualizarEnderecoDto.cs  → PUT  /api/enderecos/{id}
    └── EnderecoResponseDto.cs   → GET  /api/enderecos
```

Regra: **entidade entra e sai só dentro do Service** — controller e cliente só veem DTOs.

---

## 💡 Dica de Ouro

Comece simples: mapeamento manual (DTO → Entidade → DTO) é fácil de entender e depurar.  
Quando o projeto crescer e o mapeamento virar repetição, aí considere uma biblioteca como **AutoMapper**.  
Mas por enquanto, o manual deixa claro o que está acontecendo — e isso é valioso quando você está aprendendo.
