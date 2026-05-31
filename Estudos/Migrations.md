# Migrations — Entity Framework Core

---

## 🔗 Analogia

Imagine que o banco de dados é um apartamento e suas classes C# são a planta baixa.  
Cada vez que você muda a planta (adiciona um cômodo, remove uma parede), você registra essa mudança numa **ordem de obra**.  
A Migration é essa ordem de obra — um documento versionado que diz exatamente o que mudar no apartamento, na sequência certa.

---

## 📖 Conceito

Migration é um arquivo gerado automaticamente pelo EF Core que traduz mudanças nas suas classes C# em comandos SQL.  
Ela tem dois métodos: `Up` (aplica a mudança) e `Down` (desfaz a mudança).  
O EF mantém um histórico de quais migrations já foram aplicadas numa tabela `__EFMigrationsHistory` no próprio banco.  
Com isso, qualquer desenvolvedor do time pode sincronizar o banco local com apenas um comando.

---

## 💻 Código

```csharp
// ── Passo 1: você tem suas entidades ──────────────────────
public class Pessoa
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Cpf { get; set; }
    public int Idade { get; set; }
}

// ── Passo 2: roda o comando no terminal ───────────────────
// dotnet ef migrations add CriacaoInicial

// ── Passo 3: EF gera este arquivo automaticamente ─────────
// Migrations/20260515120000_CriacaoInicial.cs
public partial class CriacaoInicial : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // SQL que CRIA a tabela
        migrationBuilder.CreateTable(
            name: "Pessoas",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Nome = table.Column<string>(maxLength: 100, nullable: false),
                Cpf = table.Column<string>(maxLength: 11, nullable: false),
                Idade = table.Column<int>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Pessoas", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // SQL que DESFAZ — dropa a tabela
        migrationBuilder.DropTable(name: "Pessoas");
    }
}

// ── Passo 4: aplica ao banco ──────────────────────────────
// dotnet ef database update


// ── Exemplo de Migration de alteração ─────────────────────
// Você adicionou o campo Ativo na entidade:
public class Pessoa
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Cpf { get; set; }
    public int Idade { get; set; }
    public bool Ativo { get; set; }          // << novo campo
    public DateTime DataCadastro { get; set; } // << novo campo
}

// dotnet ef migrations add AdicionarCamposAtivoCadastro

// EF gera:
public partial class AdicionarCamposAtivoCadastro : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "Ativo",
            table: "Pessoas",
            nullable: false,
            defaultValue: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "DataCadastro",
            table: "Pessoas",
            nullable: false,
            defaultValue: DateTime.UtcNow);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "Ativo", table: "Pessoas");
        migrationBuilder.DropColumn(name: "DataCadastro", table: "Pessoas");
    }
}
```

---

## 🔍 Linha por Linha

1. `dotnet ef migrations add NomeDaMigration` — compara o estado atual das entidades com o último snapshot e gera o arquivo de migration.
2. `protected override void Up(...)` — contém o SQL que **avança** o banco (cria tabela, adiciona coluna, etc.).
3. `protected override void Down(...)` — contém o SQL que **reverte** a migration; permite rollback.
4. `migrationBuilder.CreateTable(...)` — instrução para criar a tabela com suas colunas e constraints.
5. `.Annotation("SqlServer:Identity", "1, 1")` — configura auto-incremento da chave primária.
6. `table.PrimaryKey("PK_Pessoas", x => x.Id)` — define a PK da tabela.
7. `dotnet ef database update` — lê a tabela `__EFMigrationsHistory`, identifica migrations não aplicadas e executa o `Up` de cada uma em ordem.
8. `migrationBuilder.AddColumn<bool>(...)` — adiciona coluna sem recriar a tabela; seguro em produção.
9. `migrationBuilder.DropColumn(...)` — remove a coluna no rollback.

---

## 🏗️ Na Sua API

**Fluxo completo do zero:**

```bash
# 1. Instalar ferramentas (uma vez por máquina)
dotnet tool install --global dotnet-ef

# 2. Adicionar pacotes ao projeto
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design

# 3. Criar a primeira migration
dotnet ef migrations add CriacaoInicial

# 4. Aplicar ao banco
dotnet ef database update

# ── A cada mudança nas entidades ──────────────────────────

# 5. Gerar nova migration
dotnet ef migrations add DescricaoClara_DoQueVoceMudou

# 6. Aplicar
dotnet ef database update

# ── Comandos úteis ────────────────────────────────────────

# Ver lista de migrations e quais já foram aplicadas
dotnet ef migrations list

# Reverter para uma migration anterior
dotnet ef database update NomeDaMigrationAnterior

# Remover a última migration (se ainda não foi aplicada ao banco)
dotnet ef migrations remove
```

**Tabela `__EFMigrationsHistory`** — criada automaticamente pelo EF no banco:
```
MigrationId                              ProductVersion
────────────────────────────────────── ──────────────
20260515120000_CriacaoInicial            8.0.0
20260515130000_AdicionarCamposAtivo      8.0.0
```

---

## 💡 Dica de Ouro

**Nomeie migrations de forma descritiva** — `AdicionarCampoAtivoEmPessoa` é infinitamente melhor que `Migration3`.  
Daqui a seis meses você (ou alguém do time) vai olhar o histórico e precisar entender o que cada uma fez sem abrir o arquivo.  
E nunca edite um arquivo de migration que já foi aplicado em produção — se precisar corrigir, crie uma nova migration que desfaz e refaz o correto.
