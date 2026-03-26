using MassTransit;
using UserService.Application.Services;
using UserService.Core.Abstractions;
using UserService.RabbitMQ.Consumers;
using UserService.RabbitMQ.Definitions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddScoped<IKeycloakService,  KeycloakService>();

builder.Services.AddMassTransit(busConf =>
{
    busConf.SetKebabCaseEndpointNameFormatter();

    busConf.AddConsumer<UsersConsumer>();

    busConf.UsingRabbitMq((ctx, configurator) =>
    {
        configurator.Host(new Uri(builder.Configuration["MessageBroker:Host"]), h =>
        {
            h.Username(builder.Configuration["MessageBroker:Username"]);
            h.Password(builder.Configuration["MessageBroker:Password"]);
        });

        //configurator.ConfigureEndpoints(ctx);

        // Просто подключаемся к существующей очереди
        configurator.ReceiveEndpoint(builder.Configuration["MessageBroker:Users-queue"], e =>
        {
            // Отключаем автоматическое создание всего
            e.ConfigureConsumeTopology = false;

            // Настраиваем чтение JSON
            e.ClearSerialization();
            e.UseRawJsonSerializer();

            e.ConfigureConsumer<UsersConsumer>(ctx);
        });
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
