# Testes Unitários

## 🔗 Analogia

Imagine que você monta um carro e, antes de entregar, testa cada peça separadamente: o motor, os freios, as luzes. Testes unitários fazem exatamente isso — testam cada "peça" do seu código de forma isolada, sem depender das outras.

---

## 📖 Conceito

Testes unitários verificam se um método ou classe funciona corretamente de forma isolada. No .NET, usamos o **xUnit** (ou NUnit/MSTest). Você escreve um método de teste que chama seu código e verifica o resultado com `Assert`. Se o resultado bater com o esperado, o teste passa.

---

## 💻 Código

```csharp
// Projeto de teste separado: MinhaApi.Tests
using Xunit;

public class EnderecoServiceTests
{
    [Fact]
    public void CriarEndereco_ComDadosValidos_DeveRetornarEnderecoPreenchido()
    {
        // Arrange
        var service = new EnderecoService();
        var dto = new EnderecoDTO
        {
            Rua = "Rua das Flores",
            Cidade = "São Paulo",
            Cep = "01310-100"
        };

        // Act
        var resultado = service.CriarEndereco(dto);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Rua das Flores", resultado.Rua);
        Assert.Equal("01310-100", resultado.Cep);
    }

    [Fact]
    public void CriarEndereco_ComCepVazio_DeveLancarExcecao()
    {
        // Arrange
        var service = new EnderecoService();
        var dto = new EnderecoDTO { Rua = "Rua X", Cidade = "SP", Cep = "" };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => service.CriarEndereco(dto));
    }
}
```

---

## 🔍 Linha por Linha

**1. `[Fact]`**
Atributo do xUnit que marca o método como um teste. O runner executa tudo que tiver esse atributo.

**2. Padrão `// Arrange / Act / Assert` (AAA)**
- **Arrange** — prepara os dados e objetos necessários
- **Act** — executa o método que você quer testar
- **Assert** — verifica se o resultado é o esperado

**3. `Assert.NotNull(resultado)`**
Garante que o retorno não é nulo. Se for, o teste falha com uma mensagem clara.

**4. `Assert.Equal("Rua das Flores", resultado.Rua)`**
Compara valor esperado com valor real. Ordem: **esperado primeiro**, depois o real.

**5. `Assert.Throws<ArgumentException>(() => ...)`**
Verifica que o método lança uma exceção específica — útil para testar validações.

---

## 🏗️ Na Sua API

Crie um projeto separado chamado `MinhaApi.Tests` dentro da solution. Para a classe `Endereco`, você pode testar:

- Se `EnderecoService.CriarEndereco()` preenche corretamente os campos `Rua`, `Cidade` e `Cep`
- Se o serviço rejeita um CEP inválido ou vazio
- Se `PessoaService.AdicionarEndereco()` associa o endereço à pessoa corretamente

```bash
dotnet new xunit -n logic-programming-studies.Tests
dotnet add logic-programming-studies.Tests/logic-programming-studies.Tests.csproj reference logic-programming-studies.csproj
dotnet test
```

---

## 💡 Dica de Ouro

Siga o padrão **AAA** religiosamente e nomeie seus testes assim:
`Metodo_Cenario_ResultadoEsperado`
Ex: `CriarEndereco_ComCepVazio_DeveLancarExcecao`

Quando o teste falhar em produção, o nome já te diz exatamente o que quebrou — sem precisar abrir o código.