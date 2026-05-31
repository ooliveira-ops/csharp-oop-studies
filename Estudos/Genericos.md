# Genéricos — `<T>` (Generics)

---

## 🔗 Analogia

Pense numa caixa de papelão. Ela pode guardar livros, sapatos ou eletrônicos — a caixa é a mesma, o conteúdo muda.  
Genéricos são essa caixa: você escreve o código **uma vez** e ele funciona para qualquer tipo.  
O `<T>` é o rótulo que diz "o tipo será definido na hora de usar".

---

## 📖 Conceito

Genéricos permitem criar classes, métodos e interfaces que funcionam com **qualquer tipo** sem perder segurança de tipagem.  
Você já usa isso o tempo todo: `List<Pessoa>`, `Task<IEnumerable<Pessoa>>`, `IEnumerable<T>` — todos são genéricos.  
O benefício: escrever uma lógica reutilizável sem duplicar código para `int`, `string`, `Pessoa`, etc.  
O compilador substitui `<T>` pelo tipo real na hora da compilação — sem custo de performance.

---

## 💻 Código

```csharp
// Sem genérico — precisaria repetir para cada tipo
public Pessoa BuscarPrimeiro(List<Pessoa> lista) => lista.FirstOrDefault();
public Endereco BuscarPrimeiro(List<Endereco> lista) => lista.FirstOrDefault();

// Com genérico — uma única implementação para qualquer tipo
public T BuscarPrimeiro<T>(List<T> lista) => lista.FirstOrDefault();

// Usando:
var pessoa   = BuscarPrimeiro<Pessoa>(listaDePessoas);
var endereco = BuscarPrimeiro<Endereco>(listaDeEnderecos);


// ----- Classe genérica: ApiResponse<T> -----
// Padrão muito usado em APIs para padronizar o envelope de resposta
public class ApiResponse<T>
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; }
    public T Dados { get; set; }

    public static ApiResponse<T> Ok(T dados) =>
        new ApiResponse<T> { Sucesso = true, Dados = dados };

    public static ApiResponse<T> Erro(string mensagem) =>
        new ApiResponse<T> { Sucesso = false, Mensagem = mensagem };
}

// Usando no controller — o <T> vira o tipo que você passar
[HttpGet("{id}")]
public async Task<IActionResult> GetById(int id)
{
    var pessoa = await _pessoaService.ObterPorIdAsync(id);

    if (pessoa is null)
        return NotFound(ApiResponse<PessoaResponseDto>.Erro("Pessoa não encontrada."));

    return Ok(ApiResponse<PessoaResponseDto>.Ok(pessoa));
}

// Resposta JSON gerada:
// {
//   "sucesso": true,
//   "mensagem": null,
//   "dados": { "id": 1, "nome": "Ana", "idade": 28 }
// }


// ----- Restrição de tipo com where -----
// where T : class  → T deve ser uma classe (não int, bool...)
// where T : new()  → T deve ter construtor sem parâmetros
public T Criar<T>() where T : class, new()
{
    return new T(); // só funciona por causa do where T : new()
}
```

---

## 🔍 Linha por Linha

1. `public T BuscarPrimeiro<T>(List<T> lista)` — `T` é um parâmetro de tipo; na chamada, você diz qual tipo usar.
2. `BuscarPrimeiro<Pessoa>(lista)` — aqui o compilador substitui `T` por `Pessoa`.
3. `public class ApiResponse<T>` — classe genérica; `T` representa o tipo dos dados da resposta.
4. `public T Dados { get; set; }` — propriedade cujo tipo só é definido na hora de instanciar a classe.
5. `ApiResponse<PessoaResponseDto>.Ok(pessoa)` — `T` vira `PessoaResponseDto`; o compilador garante o tipo.
6. `ApiResponse<T>.Erro(mensagem)` — o mesmo envelope funciona para qualquer tipo de dado.
7. `where T : class, new()` — restrições que limitam quais tipos são aceitos como `T`.
8. `return new T()` — só é possível instanciar `T` quando existe a restrição `new()`.

---

## 🏗️ Na Sua API

Use `ApiResponse<T>` para padronizar todas as respostas:

```csharp
// Todos os endpoints retornam o mesmo envelope
[HttpGet]
public async Task<IActionResult> GetAll()
{
    var pessoas = await _pessoaService.ObterTodosAsync();
    return Ok(ApiResponse<IEnumerable<PessoaResponseDto>>.Ok(pessoas));
}

[HttpPost]
public async Task<IActionResult> Create(CriarPessoaDto dto)
{
    var criada = await _pessoaService.CriarAsync(dto);
    return CreatedAtAction(nameof(GetById), new { id = criada.Id },
        ApiResponse<PessoaResponseDto>.Ok(criada));
}

[HttpGet("{id}")]
public async Task<IActionResult> GetById(int id)
{
    var pessoa = await _pessoaService.ObterPorIdAsync(id);
    if (pessoa is null)
        return NotFound(ApiResponse<PessoaResponseDto>.Erro("Pessoa não encontrada."));

    return Ok(ApiResponse<PessoaResponseDto>.Ok(pessoa));
}
```

O cliente da API sempre recebe `{ sucesso, mensagem, dados }` — consistente em todos os endpoints.

---

## 💡 Dica de Ouro

`ApiResponse<T>` é um dos padrões mais valorizados em APIs profissionais — o frontend agradece por não precisar adivinhar o formato de erro vs sucesso.  
Você já usa genéricos sem perceber: `List<T>`, `Task<T>`, `IEnumerable<T>` são todos genéricos do próprio .NET.  
Entender `<T>` é a chave para ler a documentação do C# com fluidez — tudo faz mais sentido depois disso.
