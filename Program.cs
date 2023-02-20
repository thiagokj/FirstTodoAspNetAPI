using Todo.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Adiciona o suporte ao uso do DbContext, fazendo toda parte de gerenciamento
// do ciclo de vida da conexão.
builder.Services.AddDbContext<AppDbContext>();

var app = builder.Build();

app.MapControllers();
app.UseHttpsRedirection();
app.Run();




