using VillagioApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Define a string de conexão
var strConn = builder.Environment.IsDevelopment()
    ? builder.Configuration.GetConnectionString("strConnExterna")
    : builder.Configuration.GetConnectionString("strConnExterna");

// Registra o DBContext com a conexão SQL Server
builder.Services.AddDbContext<DBContext>(options =>
    options.UseSqlServer(strConn));

// Adiciona Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Adiciona controllers
builder.Services.AddControllers();

var app = builder.Build();

// Middleware do Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();