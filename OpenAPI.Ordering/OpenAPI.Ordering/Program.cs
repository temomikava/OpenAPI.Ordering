using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using OpenAPI.Identity.Data;
using OpenAPI.Ordering.Configurations;
using OpenAPI.Ordering.Data;
using OpenAPI.Ordering.IntegrationEventHandlers;
using OpenAPI.Ordering.Services;
using SharedKernel;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });
builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddMassTransit(configuration =>
{
    configuration.AddConsumers(typeof(CompanyRegisteredIntegrationEventHandler).Assembly);
    var rabbitmqConfig = builder.Configuration.GetSection(nameof(RabbitmqConfig)).Get<RabbitmqConfig>();
    configuration.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitmqConfig.Host, "/", h =>
        {
            h.Username(rabbitmqConfig.UserName);
            h.Password(rabbitmqConfig.Password);
        });
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddScoped<IIntegrationEventService,IntegrationEventService>();
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<OrderComputationService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
await using (var scope = app.Services.CreateAsyncScope())
{
    await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.MigrateAsync();
}
app.Run();
