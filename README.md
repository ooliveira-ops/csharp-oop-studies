# C# OOP Studies

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![VS Code](https://img.shields.io/badge/VS%20Code-007ACC?style=for-the-badge&logo=visualstudiocode&logoColor=white)
![AI Assisted](https://img.shields.io/badge/AI%20Assisted-Claude-D97757?style=for-the-badge&logo=anthropic&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

> Caderno de estudos de **Programação Orientada a Objetos em C#**, escrito linha por linha
> para construir base sólida antes de partir para projetos em ASP.NET Core.

---

## 🎯 Contexto

Estou há poucos meses como **estagiário de desenvolvimento backend** e recebi o desafio de
construir uma **API de Pessoas e Endereços**. Em vez de copiar tutoriais, decidi documentar
cada conceito que precisei aprender: escrever o código, explicar o que cada linha faz e
registrar onde aquilo se aplica no projeto real.

Este repositório é esse registro.

**Objetivos:**
- Dominar os fundamentos de POO em vez de decorar sintaxe.
- Entender *por que* cada camada da API existe (Controller → Service → Repository).
- Criar material de consulta que eu mesmo escrevi — e por isso entendo.

---

## 🤖 Como usei IA como tutor

Trabalhei com a IA no **sidebar do VS Code**, mas com uma regra: ela não entrega código pronto,
ela me **ensina**. Para garantir isso, criei um prompt customizado em
[`prompts-de-estudo/analisar-e-executar.md`](prompts-de-estudo/analisar-e-executar.md) que obriga toda
resposta a seguir a mesma estrutura:

| Seção | O que a IA é obrigada a entregar |
|-------|----------------------------------|
| 🔗 **Analogia** | Comparação com algo do cotidiano (máx. 3 linhas) |
| 📖 **Conceito** | Explicação direta e simples (máx. 4 linhas) |
| 💻 **Código** | Exemplo funcional **dentro do domínio Pessoa/Endereço** |
| 🔍 **Linha por linha** | Cada bloco explicado com numeração |
| 🏗️ **Na sua API** | Onde e como aplicar no projeto real |
| 💡 **Dica de ouro** | Um conselho prático para o desafio |
---
**Uso:**

```text
@/analisar-e-executar O que é DTO?
@/analisar-e-executar Como usar Entity Framework?
@/analisar-e-executar Qual a diferença entre var e tipo explícito?
```

**Por que isso importa:** amarrar o exemplo ao meu domínio real (`Pessoa`, `Endereco`) elimina
o "código genérico de tutorial" e me força a entender a aplicação, não só a sintaxe.
Cada arquivo em [`Estudos/`](Estudos/) nasceu de uma dessas sessões - Lendo, escrevendo linha por linha e depois revendo.

---

## 📚 Índice de conteúdos

### Fundamentos de POO
| Tema | Nota |
|------|------|
| Classes e Objetos | [`Classes-e-Objetos.md`](Estudos/Classes-e-Objetos.md) |
| Herança e Interfaces | [`Heranças-e-interface.md`](Estudos/Heranças-e-interface.md) |
| Propriedades (`get` / `set`) | [`Propriedades-get-set.md`](Estudos/Propriedades-get-set.md) |
| Modificadores de acesso | [`pub-priv-read.md`](Estudos/pub-priv-read.md) |
| Tipos comuns | [`Tipos-comuns.md`](Estudos/Tipos-comuns.md) |
| Genéricos `<T>` | [`Genericos.md`](Estudos/Genericos.md) |

### Arquitetura de API
| Tema | Nota |
|------|------|
| Estrutura de uma API | [`Estrutura-de-uma-API.md`](Estudos/Estrutura-de-uma-API.md) |
| Injeção de dependência | [`Injecao-de-dependencias.md`](Estudos/Injecao-de-dependencias.md) |
| DTOs | [`DTOs.md`](Estudos/DTOs.md) |
| Métodos assíncronos | [`Metodos-assincronos.md`](Estudos/Metodos-assincronos.md) |
| Swagger | [`Swagger.md`](Estudos/Swagger.md) |

### Acesso a dados
| Tema | Nota |
|------|------|
| Entity Framework | [`Entity-Framework.md`](Estudos/Entity-Framework.md) |
| Migrations | [`Migrations.md`](Estudos/Migrations.md) |
| LINQ | [`Linq.md`](Estudos/Linq.md) |
| Relacionamento 1:1 | [`Relationship1-1.md`](Estudos/Relationship1-1.md) |

### Qualidade
| Tema | Nota |
|------|------|
| Testes unitários (xUnit) | [`Testes-unitarios.md`](Estudos/Testes-unitarios.md) |

---

## 🗂️ Estrutura do repositório

```text
csharp-oop-studies/
├── Program.cs        # Laboratório: todos os exemplos, comentados linha por linha
├── Estudos/          # Notas em Markdown — um arquivo por conceito
├── ai-prompts/       # Prompts customizados que uso como tutor no VS Code
├── LICENSE
└── README.md
```

O [`Program.cs`](Program.cs) funciona como laboratório: reúne as classes `Pessoa`, `Endereco`,
`PessoaRepository`, `PessoaService`, `PessoasController` e `ApiResponse<T>`, indo dos tipos
básicos até relacionamentos e testes.

---

## 🚀 Como executar

```bash
git clone https://github.com/ooliveira-ops/csharp-oop-studies.git
cd csharp-oop-studies
dotnet run
```

**Requisitos:** [.NET SDK](https://dotnet.microsoft.com/download) 8.0 ou superior.

---

## 📄 Licença

Distribuído sob a licença [MIT](LICENSE).

---

<div align="center">

**Repositório mantido por [ooliveira-ops](https://github.com/ooliveira-ops)**
*Estudando em público — cada commit é um conceito a mais entendido.*

</div>
