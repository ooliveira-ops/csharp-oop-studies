# Classes e Objetos

## 🔗 Analogia

Pensa numa **ficha cadastral de pessoa**: o modelo da ficha (campos: nome, CPF, endereço) é a **classe**. Cada ficha preenchida com dados reais de uma pessoa específica é o **objeto**.

---

## 📖 Conceito

Uma **classe** é o molde que define quais dados (propriedades) e comportamentos (métodos) algo terá. Um **objeto** é uma instância concreta desse molde, criado com `new`. Em C#, tudo gira em torno de classes — controllers, services, models são todos classes.

---

## 💻 Código

```csharp
// Classe = o molde
public class Pessoa
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Cpf { get; set; }
    public Endereco Endereco { get; set; }

    public string ObterResumo()
    {
        return $"{Nome} - CPF: {Cpf}";
    }
}

public class Endereco
{
    public string Rua { get; set; }
    public string Cidade { get; set; }
    public string Cep { get; set; }
}

// Criando objetos (instâncias)
var endereco = new Endereco
{
    Rua = "Rua das Flores",
    Cidade = "São Paulo",
    Cep = "01310-100"
};

var pessoa = new Pessoa
{
    Id = 1,
    Nome = "Ana Silva",
    Cpf = "123.456.789-00",
    Endereco = endereco
};

Console.WriteLine(pessoa.ObterResumo());
// Saída: Ana Silva - CPF: 123.456.789-00
```

---

## 🔍 Linha por Linha

**1 — `public class Pessoa`**
Declara a classe. `public` = acessível em qualquer lugar do projeto.

**2 — `public string Nome { get; set; }`**
Propriedade auto-implementada. `get` lê o valor, `set` atribui. O `{ get; set; }` é o atalho do C# para isso.

**3 — `public Endereco Endereco { get; set; }`**
Uma classe pode conter outra como propriedade — isso se chama **composição**. `Pessoa` tem um `Endereco`.

**4 — `public string ObterResumo()`**
Método da classe: um comportamento que o objeto sabe executar usando seus próprios dados.

**5 — `var endereco = new Endereco { ... }`**
Cria um objeto usando **object initializer** — atalho para setar propriedades sem precisar de construtor.

**6 — `pessoa.ObterResumo()`**
Acessa o método através do objeto com o operador `.` (ponto).

---

## 🏗️ Na Sua API

Você vai usar isso em três lugares diretos:

| Arquivo | Papel |
|---|---|
| `Models/Pessoa.cs` | Classe que representa a tabela no banco |
| `DTOs/PessoaDto.cs` | Classe usada para trafegar dados na API |
| `Controllers/PessoaController.cs` | Classe que recebe as requisições HTTP |

No controller, você vai criar objetos do tipo `Pessoa` para salvar, e objetos `PessoaDto` para retornar — tudo instâncias de classes.

---

## 💡 Dica de Ouro

Nunca exponha sua classe `Pessoa` (Model) diretamente nos endpoints. Crie um `PessoaDto` separado — assim você controla exatamente o que entra e o que sai da API, sem vazar campos sensíveis como senha ou dados internos.
