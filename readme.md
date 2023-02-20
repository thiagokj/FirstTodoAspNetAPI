# First Todo ASP.NET API

Projeto para revisão de conceitos e aprendizado.

Continuação do projeto [FirstAspNetAPI](https://github.com/thiagokj/FirstAspNetAPI), com introdução ao padrão de arquiterura MVC.

## Requisitos

- Dotnet versão 7
- VS Code Thunder Client
- Entity Framework -> dotnet add package Microsoft.EntityFrameworkCore.Design
- Sqlite -> dotnet add package Microsoft.EntityFrameworkCore.Sqlite

## Primeiros passos

1. Crie a Model do Todo.
1. Crie o AppDbContext com o DbSet do tipo TodoModel.
1. Crie o HomeController com herança da classe ControllerBase. O ControllerBase é melhor para trabalhar com APIs, retornando só JSON.

```Csharp
namespace Todo.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        // Os métodos do controller são chamados de Actions.
        [HttpGet]
        [Route("/")]
        public string Get()
        {
            return "Hello World Controller";
        }
    }
}
```

## Configurando Controllers e DbContext

1. Utilizamos o builder.Services para adicionar o suporte aos serviços disponibilizados pelo AspNet. Abaixo um exemplo do uso de Controllers pela aplicação.

```Csharp
var builder = WebApplication.CreateBuilder(args);

// Adiciona o suporte ao uso de Controllers na aplicação.
builder.Services.AddControllers();

var app = builder.Build();

// Mapeia os Controllers.
app.MapControllers();

app.Run();
```

1. Para adicionar o suporte a conexão com banco de dados, podemos adicionar o **Services.AddDbContext**.

```Csharp
...
builder.Services.AddControllers();

// Adiciona o suporte ao uso do DbContext, fazendo toda parte de gerenciamento
// do ciclo de vida da conexão.
builder.Services.AddDbContext<AppDbContext>();
```

Dessa forma, podemos criar um método para retornar uma lista de tarefas.

```Csharp
    ...
    // HomeController é uma convenção de nome para um controlador inicial.
    public class HomeController : ControllerBase
    {
        [HttpGet("/")]
        // Utiliza o DbContext dos serviços do AspNet via injeção de dependência.
        public List<TodoModel> Get([FromServices] AppDbContext context)
        {
            return context.Todos.ToList();
        }
    }
```

## CRUD básico no HomeController

Um exemplo simples e que ainda pode ser aperfeiçoado, com as operações basicas: Create (Criação), Read (Leitura), Update (Atualização) e Delete (Exclusão).

Passando o CRUD para verbos Http, temos: Post (Criação), Get (Leitura), Put (Atualização) e Delete (Exclusão).

```Csharp
    [ApiController]
    public class HomeController : ControllerBase
    {
        // Os métodos do controller são chamados de Actions.
        // Faz a requisição via URL.
        [HttpGet("/")]
        public List<TodoModel> Get([FromServices] AppDbContext context)
        {
            // Retorna uma lista de itens.
            return context.Todos.ToList();
        }

        // Faz a requisição via URL, com a passagem de parâmetros.
        [HttpGet("/{id:int}")]
        public TodoModel GetById(
            // A anotação FromRoute/FromBody é opcional, o Http já consegue entender.
            // Mas é uma boa prática declarar, melhorando a legibilidade do código.
            [FromRoute] int id,
            [FromServices] AppDbContext context
        )
        {
            // Retorna apenas um item.
            return context.Todos.FirstOrDefault(x => x.Id == id);
        }

        // Faz a requisição via Body.
        [HttpPost("/")]
        public TodoModel Post(
            [FromBody] TodoModel todo,
            [FromServices] AppDbContext context)
        {
            // Insere um item.
            context.Todos.Add(todo);
            context.SaveChanges();

            // O Asp.Net faz a conversão do retorno em JSON.
            return todo;
        }

        // Faz a requisição via Body, com base nos parâmetros da URL.
        [HttpPut("/{id:int}")]
        public TodoModel Put(
            [FromRoute] int id,
            [FromBody] TodoModel todo,
            [FromServices] AppDbContext context
        )
        {
            // Atualiza um item.
            var model = context.Todos.FirstOrDefault(x => x.Id == id);
            if (model == null) return todo;

            model.Title = todo.Title;
            model.Done = todo.Done;

            context.Todos.Update(model);
            context.SaveChanges();
            return model;
        }

        // Faz a requisição via URL, com a passagem de parâmetros.
        [HttpDelete("/{id:int}")]
        public TodoModel Delete(
            [FromRoute] int id,
            [FromServices] AppDbContext context
        )
        {
            // Remove um item.
            var model = context.Todos.FirstOrDefault(x => x.Id == id);
            context.Todos.Remove(model);
            context.SaveChanges();
            return model;
        }
    }
```

## Exemplo de HomeController com Ações padronizadas

Aqui o exemplo aprimorado do Controller, utilizando a interface do Asp.Net IActionResult.

A interface possui métodos de retorno padrão, com os códigos específicos do Http.

```Csharp
[ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet("/")]
        public IActionResult Get([FromServices] AppDbContext context)
        => Ok(context.Todos.ToList());

        [HttpGet("/{id:int}")]
        public IActionResult GetById(
            [FromRoute] int id,
            [FromServices] AppDbContext context
        )
        {
            var todos = context.Todos.FirstOrDefault(x => x.Id == id);
            if (todos == null) return NotFound();

            return Ok(todos);
        }

        [HttpPost("/")]
        public IActionResult Post(
            TodoModel todo,
            [FromServices] AppDbContext context)
        {
            context.Todos.Add(todo);
            context.SaveChanges();

            return Created($"/{todo.Id}", todo);
        }

        [HttpPut("/{id:int}")]
        public IActionResult Put(
            [FromRoute] int id,
            [FromBody] TodoModel todo,
            [FromServices] AppDbContext context
        )
        {
            var model = context.Todos.FirstOrDefault(x => x.Id == id);
            if (model == null) return NotFound();

            model.Title = todo.Title;
            model.Done = todo.Done;

            context.Todos.Update(model);
            context.SaveChanges();
            return Ok(model);
        }

        [HttpDelete("/{id:int}")]
        public IActionResult Delete(
            [FromRoute] int id,
            [FromServices] AppDbContext context
        )
        {
            var model = context.Todos.FirstOrDefault(x => x.Id == id);
            if (model == null) return NotFound();

            context.Todos.Remove(model);
            context.SaveChanges();

            return Ok(model);
        }
    }
```
