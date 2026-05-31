# Herança e Interface

## 🔗 Analogia

**Herança** é como um filho que herda características do pai — um `Funcionario` é uma `Pessoa`, então já tem nome e CPF automaticamente. **Interface** é como um contrato de emprego — não importa quem você é, se assinou o contrato `IRepositorio`, você **obrigatoriamente** implementa os métodos combinados.

---

## 📖 Conceito

**Herança** (`class Filho : Pai`) permite que uma classe reutilize propriedades e métodos de outra, evitando repetição. **Interface** (`interface IContrato`) define apenas *o que* deve existir, sem implementar nada — quem implementa decide o *como*. Em APIs modernas, interfaces são preferidas por deixar o código mais flexível e testável.

---

## 💻 Código

```csharp
// HERANÇA: Funcionario herda de Pessoa
public class Pessoa
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Cpf { get; set; }
}

public class Funcionario : Pessoa  // herda tudo de Pessoa
{
    public string Cargo { get; set; }
    public decimal Salario { get; set; }

    public string ObterCracha()
    {
        return $"{Nome} | {Cargo}";  // Nome veio da herança!
    }
}

// INTERFACE: contrato que define o que o repositório deve fazer
public interface IPessoaRepositorio
{
    Pessoa BuscarPorId(int id);
    List<Pessoa> ListarTodos();
    void Salvar(Pessoa pessoa);
    void Deletar(int id);
}

// Classe que cumpre o contrato
public class PessoaRepositorio : IPessoaRepositorio
{
    public Pessoa BuscarPorId(int id)
    {
        // implementação real com banco de dados
        return new Pessoa { Id = id, Nome = "Ana Silva" };
    }

    public List<Pessoa> ListarTodos()
    {
        return new List<Pessoa>();
    }

    public void Salvar(Pessoa pessoa) { /* salva no banco */ }
    public void Deletar(int id) { /* deleta do banco */ }
}

// Usando no controller via interface (não a classe direta!)
public class PessoaController
{
    private readonly IPessoaRepositorio _repositorio;

    public PessoaController(IPessoaRepositorio repositorio)
    {
        _repositorio = repositorio;
    }
}
```

---

## 🔍 Linha por Linha

**1 — `public class Funcionario : Pessoa`**
O `:` significa "herda de". `Funcionario` ganha `Id`, `Nome` e `Cpf` automaticamente, sem redeclarar.

**2 — `return $"{Nome} | {Cargo}"`**
`Nome` não foi declarado em `Funcionario` — veio da herança de `Pessoa`. Funciona direto.

**3 — `public interface IPessoaRepositorio`**
Convenção: interfaces começam com `I`. Define apenas assinaturas — sem corpo, sem lógica.

**4 — `public class PessoaRepositorio : IPessoaRepositorio`**
Implementa a interface. O compilador vai **exigir** que todos os métodos do contrato existam — se faltar um, erro na hora de compilar.

**5 — `private readonly IPessoaRepositorio _repositorio`**
O controller conhece apenas a **interface**, não a classe concreta. Isso é **injeção de dependência** — você pode trocar a implementação sem mudar o controller.

---

## 🏗️ Na Sua API

| Onde usar | O quê |
|---|---|
| `Models/Pessoa.cs` e `Models/Funcionario.cs` | Herança se `Funcionario` é uma `Pessoa` |
| `Repositories/IPessoaRepositorio.cs` | Interface do repositório |
| `Repositories/PessoaRepositorio.cs` | Implementação concreta |
| `Controllers/PessoaController.cs` | Recebe a interface via construtor |

No `Program.cs` você registra:
```csharp
builder.Services.AddScoped<IPessoaRepositorio, PessoaRepositorio>();
```
Aí o .NET sabe qual classe usar quando alguém pedir a interface.

---

## 💡 Dica de Ouro

Prefira **interfaces a herança** para repositórios e services — herança cria acoplamento forte (filho depende do pai), interface cria acoplamento fraco (depende só do contrato). Uma regra prática: use herança quando a relação for **"é um"** (`Funcionario` é uma `Pessoa`), use interface quando for **"faz algo"** (`PessoaRepositorio` faz operações de banco).
