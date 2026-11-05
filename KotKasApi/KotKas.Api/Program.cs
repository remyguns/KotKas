using KotKas.Core.Interfaces;
using KotKas.Core.Services;
using KotKas.Infrastructure.Data;
using KotKas.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Data layer
builder.Services.AddDbContext<KotKasContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("KotKasConnection")));

// Repositories (Repository Design Pattern)
builder.Services.AddScoped<IRoommateRepository, RoommateRepository>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();

// Business layer
builder.Services.AddScoped<IBalanceService, BalanceService>();

builder.Services.AddControllers(options =>
{
    // Zodat methodes die eindigen op Async ook zo bereikbaar blijven
    options.SuppressAsyncSuffixInActionNames = false;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Database aanmaken (met seed data) als ze nog niet bestaat
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<KotKasContext>();
    context.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.Run();
