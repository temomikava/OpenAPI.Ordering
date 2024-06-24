using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using OpenAPI.Identity;
using OpenAPI.Identity.Data;
using OpenAPI.Ordering.IntegrationEventHandlers;
using SharedKernel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
    configuration.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("tmikava");
            h.Password("Npottwyctd12");
        });
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddScoped<IIntegrationEventService,IntegrationEventService>();
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
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
