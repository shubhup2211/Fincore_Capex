using Fincore.Application.AutoMapper.MasterTable;
using Fincore.Application.Interfaces.IMasterTable;
using Fincore.Application.Interfaces.IPayment;
using Fincore.Application.AutoMapper.MasterTable;
using Fincore.Application.Interfaces.IMasterTable;
using Fincore.Application.Interfaces.IPayment;

using Fincore.Infrastructure.Data;
using Fincore.Infrastructure.Seed;

using Fincore.Infrastructure.Services.MasterTable;
using Fincore.Infrastructure.Services.PaymentModule;

using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("dbconn"),
        b => b.MigrationsAssembly("Fincore.Infrastructure")));


// ---------------------- AutoMapper ----------------------
builder.Services.AddAutoMapper(typeof(AccountsMasterMapper));
builder.Services.AddAutoMapper(typeof(PaymentMapper));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ---------------------- Services ----------------------
builder.Services.AddScoped<IAccountMasterService, AccountMasterService>();

builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IJournalEntryService, JournalEntryService>();

// ---------------------- Rate Limiting ----------------------
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

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Uncomment only if your team still wants automatic seeding
// await DatabaseSeeder.SeedAsync(app.Services);

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

app.Run();