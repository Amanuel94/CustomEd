using System.Collections.Concurrent;
using CustomEd.RTNotification.Service.Hubs;
using CustomEd.RTNotification.Service.Model;
using CustomEd.Shared.Data;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using CustomEd.Shared.RabbitMQ;
using CustomEd.Shared.Settings;
using MassTransit;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMongo()
                .AddPersistence<Classroom>("Classroom")
                .AddPersistence<Notification>("Notification")
                .AddPersistence<User>("User");
builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ConcurrentDictionary<Guid, string>>();
builder.Services.AddMassTransitWithRabbitMq("NotificationServiceQueue");
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddAuth();
                
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

app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

app.Run();
