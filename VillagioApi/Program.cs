using VillagioApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var strConn = builder.Environment.IsDevelopment()
    ? builder.Configuration.GetConnectionString("strConnExterna")
    : builder.Configuration.GetConnectionString("strConnExterna");

builder.Services.AddDbContext<VillagioApi.Data.AppDbContext>(options =>
options.UseSqlServer(strConn));

// Adiciona serviços do Swagger
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