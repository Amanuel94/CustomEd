using System.Collections.Concurrent;
using CustomEd.RTNotification.Service.Hubs;
using CustomEd.RTNotification.Service.Model;
using CustomEd.Shared.Data;
using CustomEd.Shared.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;

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
builder.Services.AddMassTransitWithRabbitMq();
builder.Services.AddAutoMapper(typeof(Program));
                
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
app.MapHub<NotificationHub>("/notificationHub");

app.Run();
