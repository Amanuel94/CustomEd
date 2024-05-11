using System.Collections.Concurrent;
using System.Reflection;
using CustomEd.Forum.Service.Model;
using CustomEd.Forum.Service.Policies;
using CustomEd.Shared.Data;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.RabbitMQ;
using CustomEd.Shared.Settings;
using MassTransit;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
    });

// Add services to the container.
builder.Services.AddMongo()
    .AddPersistence<Student>("Student")
    .AddPersistence<Teacher>("Teacher")
    .AddPersistence<Message>("Message")
    .AddPersistence<Classroom>("Classroom");
builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddAuth();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MemberOnly", policy =>
        policy.Requirements.Add(new MemberOnlyRequirement()));
});

builder.Services.AddScoped<IAuthorizationHandler, MemberOnlyPolicy>();
builder.Services.AddMassTransitWithRabbitMq("ForumServiceQueue");
builder.Services.AddSingleton<ConcurrentDictionary<Guid, string>>();
// builder.Services.AddMassTransit(busConfigurator =>
//         {
//             busConfigurator.SetKebabCaseEndpointNameFormatter();
//             busConfigurator.AddConsumers(Assembly.GetEntryAssembly());
//             busConfigurator.UsingRabbitMq((context, configurator) =>
//             {
//                 // var rabbitMQSettings = context.GetRequiredService<IConfiguration>().GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
//                 // var queueName = Assembly.GetEntryAssembly().GetName().Name;
//                 var queueName = Guid.NewGuid().ToString();
//                 Console.WriteLine($"Queue Name: {queueName}");
//                 configurator.Host("localhost");
//                 // configurator.ConfigureEndpoints(context);
//                 configurator.ReceiveEndpoint(queueName, endpointConfigurator =>
//                 {
//                     endpointConfigurator.ConfigureConsumers(context);
//                 });

//                 configurator.UseMessageRetry(retryConfigurator =>
//                 {
//                     retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
//                 });
//             });
//         });
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();


app.MapControllers();

app.MapHub<ForumHub>("/forumHub");

app.Run();
