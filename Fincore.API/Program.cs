using Fincore.Application.AutoMapper;
using Fincore.Application.Interfaces.Opex;
using Fincore.Infrastructure.Data;
using Fincore.Infrastructure.Seed;
using Fincore.Infrastructure.Services.Opex;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;
using Fincore.Application.Interfaces.ExpenseClaim;
using Fincore.Infrastructure.Services.ExpenseClaim;
using Fincore.Application.Interfaces.WorkOrder;
using Fincore.Infrastructure.Services.WorkOrder;
using Fincore.Application.Interfaces.BudgetCategory;
using Fincore.Infrastructure.Services.BudgetCategory;
using Fincore.Application.Interfaces.Budget;
using Fincore.Infrastructure.Services.Budget;
using Fincore.Application.Interfaces.BudgetLine;
using Fincore.Infrastructure.Services.BudgetLine;

var builder = WebApplication.CreateBuilder(args);

// -------------------------
// Add Services
// -------------------------

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("localdb"),
        b => b.MigrationsAssembly("Fincore.Infrastructure")));

// Memory Cache
builder.Services.AddMemoryCache();

// Dependency Injection
builder.Services.AddScoped<IOpexRequestService, OpexRequestService>();

// Rate Limiting
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
builder.Services.AddScoped<IExpenseClaimService, ExpenseClaimService>();
builder.Services.AddScoped<IWorkOrderService, WorkOrderService>();
builder.Services.AddScoped<IBudgetCategoryService, BudgetCategoryService>();
builder.Services.AddScoped<IBudgetService, BudgetService>();
builder.Services.AddScoped<IBudgetLineService, BudgetLineService>();
var app = builder.Build();

// -------------------------
// Configure HTTP Pipeline
// -------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Seed Data
await DatabaseSeeder.SeedAsync(app.Services);

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.Run();