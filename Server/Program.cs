using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Helper;
using ServerLibrary.Reository.Contract;
using ServerLibrary.Reository.Implementation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DemoConn") ?? throw new InvalidOperationException("Sorr your connection string is not found"));
});
builder.Services.Configure<JwtSection>(builder.Configuration.GetSection("JwtSection"));
builder.Services.AddScoped<IUserAccount, UserAccountReository>();
builder.Services.AddCors(option =>
{
    option.AddPolicy("AllowBlazorWasm",
        builder => builder
        .WithOrigins("http://localhost:7281", "https://localhost:7282")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazorWasm");
app.UseAuthorization();

app.MapControllers();

app.Run();
