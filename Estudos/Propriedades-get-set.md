# Propriedades get / set

---

## 🔗 Analogia

Pense num cofre com duas ranhuras: uma para **depositar** (set) e outra para **consultar o saldo** (get).  
Você controla o que entra e o que sai — ninguém mexe direto no dinheiro lá dentro.

---

## 📖 Conceito

Propriedades são a forma do C# expor campos privados de uma classe de maneira controlada.  
`get` define o que acontece quando alguém **lê** o valor.  
`set` define o que acontece quando alguém **escreve** o valor.  
Isso protege o estado interno do objeto e permite validações.

---

## 💻 Código

```csharp
public class Pessoa
{
    private string _nome;
    private int _idade;

    public string Nome
    {
        get { return _nome; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Nome não pode ser vazio.");
            _nome = value;
        }
    }

    public int Idade
    {
        get { return _idade; }
        set
        {
            if (value < 0)
                throw new ArgumentException("Idade não pode ser negativa.");
            _idade = value;
        }
    }

    // Auto-property: quando não precisa de validação
    public string Cpf { get; set; }

    // Somente leitura externa (set privado)
    public DateTime DataCadastro { get; private set; } = DateTime.UtcNow;
}
```

---

## 🔍 Linha por Linha

1. `private string _nome;` — campo privado, ninguém acessa diretamente de fora da classe.
2. `public string Nome { get; set; }` — propriedade pública que expõe `_nome` de forma controlada.
3. `get { return _nome; }` — ao ler `pessoa.Nome`, retorna o valor do campo privado.
4. `set { ... _nome = value; }` — ao escrever `pessoa.Nome = "João"`, o `value` é o que foi passado.
5. `if (string.IsNullOrWhiteSpace(value))` — validação antes de aceitar o valor; garante integridade.
6. `public string Cpf { get; set; }` — auto-property: o compilador cria o campo privado automaticamente.
7. `public DateTime DataCadastro { get; private set; }` — qualquer um pode ler, mas só a própria classe pode alterar.

---

## 🏗️ Na Sua API

Na API de Pessoas e Endereço, use propriedades em todas as suas entidades e DTOs:

```csharp
// Entidade
public class Endereco
{
    public int Id { get; set; }
    public string Logradouro { get; set; }
    public string Cep { get; set; }
    public int PessoaId { get; set; }
}

// DTO de criação — sem Id (quem gera é o banco)
public class CriarPessoaDto
{
    public string Nome { get; set; }
    public int Idade { get; set; }
    public string Cpf { get; set; }
}
```

O Entity Framework lê e escreve via essas propriedades para mapear colunas do banco.

---

## 💡 Dica de Ouro

Use **auto-properties** (`{ get; set; }`) na maioria dos casos — é mais limpo.  
Reserve o `get`/`set` com corpo completo apenas quando precisar de **validação ou lógica** ao atribuir/ler o valor.  
E prefira `{ get; private set; }` em campos que só devem ser alterados internamente, como `DataCadastro` — evita bugs onde alguém seta um valor errado de fora.
