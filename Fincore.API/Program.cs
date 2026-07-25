using Fincore.Application.AutoMapper.Payment;
using Fincore.Application.Interfaces.IPayment;
using Fincore.Application.Interfaces.Payment;
using Fincore.Application.Mapper;
using Fincore.Infrastructure.Data;
using Fincore.Infrastructure.Seed;
using Fincore.Infrastructure.Services.Payment;
using Fincore.Infrastructure.Services.PaymentModule;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("dbconn"),
b => b.MigrationsAssembly("Fincore.Infrastructure")));

builder.Services.AddAutoMapper(typeof(MapperConfigPayment));
builder.Services.AddAutoMapper(typeof(APInvoiceProfile));


builder.Services.AddScoped<IRevenueService, RevenueService>();
builder.Services.AddScoped<IAPInvoiceService, APInvoiceService>();
//builder.Services.AddScoped<IARInvoiceService, ARInvoiceService>();

//Rate limitng
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("FixedPolicy", policy =>
    {
        policy.PermitLimit = 10;
        policy.Window = TimeSpan.FromMinutes(1);

        policy.QueueLimit = 0;

        policy.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await DatabaseSeeder.SeedAsync(app.Services);

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

app.Run();
