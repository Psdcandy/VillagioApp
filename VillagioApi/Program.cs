using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.Intrinsics.X86;
using System.Text;
using VillagioApi.Data;

var builder = WebApplication.CreateBuilder(args);

// String de conexão
var strConn = builder.Environment.IsDevelopment()
    ? builder.Configuration.GetConnectionString("strConnExterna")
    : builder.Configuration.GetConnectionString("strConnExterna");


// ✅ Configura DbContext
builder.Services.AddDbContext<DBContext>(options =>
    options.UseSqlServer(strConn));

// ✅ Adiciona Controllers
builder.Services.AddControllers();

// ✅ Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ✅ Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ✅ Sem autenticação, apenas rotas abertas
app.MapControllers();

app.Run();
