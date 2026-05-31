# Tipos Mais Usados no C#

---

## 🔗 Analogia

Pense em formulários físicos: cada campo tem um tipo de dado aceito.  
"Nome" aceita texto, "Idade" aceita número inteiro, "Ativo" aceita sim/não, "Salário" aceita número com decimal.  
Os tipos do C# são exatamente isso — você declara o que cabe em cada "campo" do seu código.

---

## 📖 Conceito

C# é uma linguagem **fortemente tipada**: toda variável tem um tipo definido e o compilador verifica se você usa corretamente.  
`var` não é um tipo sem tipo — é o compilador inferindo o tipo automaticamente pela atribuição.  
Tipos com `?` (nullable) aceitam `null` além do valor normal; sem `?`, `null` gera erro de compilação.  
Conhecer os tipos certos evita conversões desnecessárias e bugs silenciosos.

---

## 💻 Código

```csharp
// ─── Tipos de texto ───────────────────────────────────────
string nome = "Ana";           // texto, pode ser null sem ?
string? apelido = null;        // string nullable — aceita null explicitamente
char inicial = 'A';            // um único caractere (aspas simples)

// ─── Tipos numéricos inteiros ─────────────────────────────
int idade = 28;                // inteiro 32 bits (-2bi a +2bi) — o mais comum
long populacao = 8_000_000_000; // inteiro 64 bits — números muito grandes
short nivel = 3;               // inteiro 16 bits — uso raro

// ─── Tipos numéricos decimais ─────────────────────────────
double altura = 1.75;          // ponto flutuante — cálculos científicos
decimal salario = 4500.50m;    // precisão financeira (m = decimal literal)
float proporcao = 0.5f;        // menos preciso que double (f = float literal)

// ─── Tipo lógico ──────────────────────────────────────────
bool ativo = true;             // só true ou false
bool? confirmado = null;       // bool nullable — útil quando "ainda não respondeu"

// ─── Datas ────────────────────────────────────────────────
DateTime dataNascimento = new DateTime(1995, 6, 15);
DateTime agora = DateTime.UtcNow;          // sempre use UTC em APIs
DateOnly dataOnly = DateOnly.FromDateTime(agora); // só data, sem hora (.NET 6+)
TimeOnly horaOnly = new TimeOnly(14, 30);         // só hora, sem data

// ─── var — inferência de tipo ─────────────────────────────
var texto = "olá";             // compilador infere string
var numero = 42;               // compilador infere int
var pessoas = new List<Pessoa>(); // compilador infere List<Pessoa>
// var não é tipo dinâmico — o tipo é fixado na compilação

// ─── void — sem retorno ───────────────────────────────────
// Método que executa algo mas não retorna valor
public void LogarAcesso(string usuario)
{
    Console.WriteLine($"Acesso: {usuario} às {DateTime.UtcNow}");
}

// Em async: Task no lugar de void (Task = "void assíncrono")
public async Task EnviarEmailAsync(string destinatario)
{
    await _emailService.EnviarAsync(destinatario);
}

// ─── Aplicados na API de Pessoas ──────────────────────────
public class Pessoa
{
    public int Id { get; set; }               // int  — chave primária
    public string Nome { get; set; }          // string — texto obrigatório
    public string? Apelido { get; set; }      // string? — texto opcional
    public string Cpf { get; set; }           // string — CPF como texto (tem zeros à esquerda)
    public int Idade { get; set; }            // int — número inteiro
    public decimal Renda { get; set; }        // decimal — valor monetário
    public bool Ativo { get; set; }           // bool — flag de status
    public DateTime DataCadastro { get; set; } // DateTime — data e hora
}

public class Endereco
{
    public int Id { get; set; }
    public string Logradouro { get; set; }
    public string Numero { get; set; }        // string — pode ser "12A", "S/N"
    public string? Complemento { get; set; } // string? — nem todo endereço tem
    public string Cep { get; set; }           // string — mantém zeros à esquerda
    public string Cidade { get; set; }
    public string Estado { get; set; }
    public int PessoaId { get; set; }         // int — chave estrangeira
}
```

---

## 🔍 Linha por Linha

1. `string nome = "Ana"` — tipo texto; aspas duplas; `string` é alias para `System.String`.
2. `string? apelido = null` — o `?` torna o tipo nullable; sem ele, atribuir `null` gera warning/erro.
3. `int idade = 28` — inteiro 32 bits; o tipo mais comum para IDs, idades, quantidades.
4. `decimal salario = 4500.50m` — use sempre `decimal` para dinheiro; `double` perde precisão em cálculos financeiros.
5. `bool ativo = true` — só dois valores possíveis: `true` ou `false`; muito usado em flags e condições.
6. `DateTime.UtcNow` — sempre prefira UTC em APIs para evitar problemas de fuso horário.
7. `var texto = "olá"` — o compilador vê `"olá"` e infere `string`; o tipo é estático, não muda em runtime.
8. `void` — método que não retorna nada; em async use `Task` (nunca `async void` fora de eventos).
9. `string Cep` — CEP, CPF e telefone ficam em `string` mesmo sendo "números"; preserva zeros à esquerda e formatação.

---

## 🏗️ Na Sua API

Guia rápido de qual tipo usar em cada situação:

| Situação | Tipo certo | Por quê |
|---|---|---|
| ID de banco | `int` | Chave primária inteira |
| Nome, texto | `string` | Texto de tamanho variável |
| Texto opcional | `string?` | Aceita null sem erro |
| CPF, CEP, telefone | `string` | Zeros à esquerda, formatação |
| Idade, quantidade | `int` | Número inteiro |
| Dinheiro, salário | `decimal` | Precisão financeira |
| Sim/Não, ativo/inativo | `bool` | Flag binária |
| Data e hora de registro | `DateTime` (UTC) | Timestamp completo |
| Método sem retorno | `void` / `Task` | `Task` para async |

---

## 💡 Dica de Ouro

Use `decimal` para qualquer valor monetário — nunca `double` ou `float`.  
`double` tem imprecisão binária: `0.1 + 0.2` em `double` não dá exatamente `0.3`, o que em cálculo financeiro é inaceitável.  
E prefira `var` quando o tipo já está óbvio na atribuição (`var lista = new List<Pessoa>()`) — reduz ruído sem perder clareza.
