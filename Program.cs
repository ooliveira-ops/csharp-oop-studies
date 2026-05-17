using System;
using static Program;
public class Program
{
    public static void Main(string[] args)
    {
        // tipos de textos:

        string nome = "Ana";        //texto, pode ser null sem ?
        string? apelido = null;     //string nullable - aceita null

        char Inicial = 'A';     un único caractere(Aspas simples);

        //Tipos numéricos inteiros:

        int idade = 28;     //inteiro ; o mais comum
        long populacao = 8_000_000_000;     // numeros muito grandes
        short nivel = 3;        //uso raro

        //TIpos numéricos decimais
        double altura = 1.75;       //ponto flutuante 
        decimal salario = 4500.50m;     //precisão financeira
        float proporcao = 0.5f;         // float literal

        //Tipo lógico:

        bool ativo = true;          //só true ou false
        bool? confirmado = null;      //bool nullable

        //Datas:

        DateTime dataNascimento = new DateTime(1995, 6 ,15);
        DateTime agota = DateTime.UtcNow;       //sempre use UTC em APIs
        DateOnly dataOnly = DateOnly.FromDateTime(agora);    // só data,sem hora
        TimeOnly horaOnly = new TimeOnly(14, 30);       //só hora, sem data

        // var - interferência de texto:

        var texto = "olá";      //compilador interefere string
        var numero = 42;            //compilador interefe int
        var pessoas = new List<Pessoa>();   //compilador interefere List<Pessoa>
        //var não é tipo dinâmico - o tipo é fixado na compilação

        // void - sem retorno
        //Método qeu executa algo mas não retorna valor
        public void LogarAcesso(string usuario)
    {
        Console.WriteLine($"Acesso : {usuario} às {DateTime.UtcNow}");
    }

        //Em async: Task no lugar de void (Task = "void assíncrono")
    public async Task EnviarEmailAsync(string destinatario)
    {
        await _emailService.EnviarAsync(destinatario);
    } 


        //Aplicados na API de Pessoas:

        public class Pessoa
    {
        public int Id {get; set;}           //int - chave primario
        
        public string Nome {get; set;}      //string - texto obrigatório

        public string? Apelido {get; set;}  //string? - texto opcional

        public string CPF {get; set;}       //string - CPF como texto

        public int Idade {get; set;}        //Idade - num inteiro

        public decimal Renda {get; set}     //decimal - valor monetário

        public bool Ativo {get; set;}       //bool - ou verdadeiro ou falto(status)

        public DateTime DataCadastro {get; set;}    //DateTime  - data e hora
    }

    public class Endereco
    {
        public int Id {get; set;}       // int - num inteiro
        public string Logadouro {get; set;}     // string - texto
        public string Numero {get; set;}       //string - pode ser "12A", "S/N"
        public string? Complemento {get; set;}  //string? - opcional(aceita nulo = vazio)
        public string Cep {get; set;}      //string - mantém zeros à esquerda 
        public string Cidade {get; set;}     //string - texto
        public string Estado {get; set;}    //string - texto
        public int PessoaId {get; set;}     //int - num. inteiros 
    }

        // Classe = o molde
        public class Pessoa
    {
        public int Id {get; set;}
        public string Nome {get; set;}
        public string Cpf {get; set;}
        public Endereco Endereco{get; set;}

        public string ObterResumo()
        {
            return $"{Nome} - CPF: {Cpf}";
        }
    }

    public class Endereco
    {
        public string Rua {get; set;}
        public string Cidade {get; set;}
        public string Cep {get; set;}
    }


    //Criando Objetos (instâncias):
    var endereco = new Endereco
    {
        Rua = "Rua das flores",
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
// Saída: Ana Silva - Cpf: 123.456.789-00



// HERANÇA: 

    public class Pessoa
    {
        public int Id {get; set;}
        public string Nome {get; set;}
        public string Cpf {get; set;}
    }

//Funcionario herda de Pessoa:
    public class Funcionario : Pessoa  //herda tudo de "Pessoa"
    {
        public string Cargo {get; set;}
        public decimal Salario {get; set;}

        public string ObterCracha()
        {
            return $"{Nome} | {Cargo}"; //Nome veio da herança!
        }
    }

// INTERFACE: contrato que define o que o repositório deve fazer

    public interface IPessoaRepositorio
    {
     Pessoa BuscarPorId(int id);
     List<Pessoa> ListarTodos();
     void Salvar (Pessoa pessoa);
     void Deletar(int id);   
    }

//Classe que cumpre o contarto(da interafe):

    public class PessoaRepositorio : IPessoaRepositorio
    {
        public Pessoa BuscarPorId(int id)
        {
            //implementação real, com banco de dados
            return new Pessoa {Id = id, Nome = "Ana Silva"};
        }

        public List<Pessoa> ListarTodos()
        {
            return new List<Pessoa>();
        }

        public void Salvar(Pessoa pessoa) {/* Salva no banco */}
        
        public void Deletar(int id) {/* deleta do banco */}


     //Usando no controller via interface (não a classe direta)
        public class PessoaController
        {
            private readonly IPessoaRepositorio _repositorio;

            public PessoaController(IPessoaRepositorio repositorio);
            {
                _repositorio = repositorio;
            }
        }
    }

//1. MODEL - representa a entidade no banco
    public class Pessoa
    {
        public int Id{get; set;}
        public string Nome{get; set;}
        public Endereco Endereco{get; set;}
    }

//2. DTO - o que o cliente envia/recebe (sem expor o Model direto)
    public class PessoaDto
    {
        public string Nome{get; set;}
        public string Rua{get; set;}
        public string Cidade{get; set;}
    }

//3. REPOSITORY - acessa o banco via Entity Framework
    public class PessoaRepository
    {
        private readonly AppDbContext _context;
        public PessoaRepository(AppDbDontext context) => _context = context;

        public async Task<List<Pessoa>> GetAllAsync() 
        => await _context.Pessoas.Include(p => p.Endereco)
        .ToListAsycn();
    }


//4. SERVICE - lógica de negócio:
    public class PessoaService
    {
        private readonly PessoaRepository _repo;
        public PessoaService(PessoaRepository repo)
        => _repo = repo;

        public async Task<List<Pessoa>> ListarTodas()
        => await _repo.GetAllAsync();
    }



//5. CONTROLLER - ponto de entrada HTTP
    [ApiController]
    [Route("api/[controller]")]
    public class PessoasController : ControllerBase
    {
        private readonly PessoaService _service;
        public PessoasController(PessoaService service)
        => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var pessoas = await _service.ListarTodas();
            return Ok(pessoas);
        }
}


//PROPRIEDAS : get e set:
    public class Pessoa
    {
        private string _nome;
        private int _idade;

        public string Nome
        {
            get {return _nome; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentExecption("Nome não pode ser vazio.");
                    _nome = value;
            }
       }


    public int Idade
    {
        get {return _idade;}
        set
        {
            if (value < 0)
            throw new ArgumentException("Idade não pode ser negativa.");
                _idade = value;
        }
    }

    //auto-property: quando não precisa de validação
    public string Cpf {get; set:}

    //somente leitura externa (set privado)
    public DateTime DataCadastri {get; private set;}
    = DateTime.UtcNow;

//ENTIDADE:
public class Endereco
    {
        public int Id{get; set;}
        public string Logradouro{get; set;}
        public string Cep{get; set;}
        public int PessoaId{get; set;}
    }

//DTP de criação - sem Id(quem gera é o banco)
public class CriarPessoaDto
    {
        public string Nome{get; set;}
        public int Idade{get; set;}
        public string Cpf{get; set;}
    }
}


//INJEÇÃO DE DEPENDENCIA:
//1. Interface - (define o contrato):
public interface IPessoaService
{
    IEnumerable<Pessoa> ObterTodos();
    Pessoa ObterPorId(int id);
    void Criar(CriarPessoaDto dto);   
}                                                                                                                                                                                                

//2. Implementação concreta:
public class PessoaService : IPessoaService
{
    private readonly AppDbContext _context;

//Dependência recebida pelo construtor - ñ criada aqui 
    public PessoaService(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Pessoa> ObterTodos()
    => _context.Pessoas.ToList();

    public void Criar(CriarPessoaDto dto)
    {
        var pessoa = new Pessoa {Nome = dto.Nome, 
        Cpf = dto.Cpf, Idade = dto.Idade};
        _context.SaveChanges();
    }
}

//3. Registro no Program.cs
builder.Services.AddScoped<IPessoaService, PessoaService>();


//4. Controller recebe o serviço automaticamente
[ApiController]
[Route("api/[controller]")]
public class PessoasController : ControllerBase
{
    private readonly IPessoaService _pessoaService;

    public PessoasController(IPessoaService pessoaervice)
    {
        _pessoaService = pessoaService;
    }
}


    }
}



