using System.Security.Claims;
using CusotmEd.Classroom.Service.Clients;
using CustomEd.Classroom.Service.Model;
using CustomEd.Shared.Data;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.RabbitMQ;
using CustomEd.Shared.Settings;
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
builder.Services.AddHttpClient<ClassroomApiClient>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5126");
        });


// Add services to the container.

builder.Services.AddMongo();
builder.Services.AddPersistence<Classroom>("Classroom");
builder.Services.AddPersistence<Teacher>("Teacher");
builder.Services.AddPersistence<Student>("Student");
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddMassTransitWithRabbitMq("ClassroomServiceQueue");
builder.Services.AddAuth();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IdentityProvider>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TeacherOnlyPolicy", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(ClaimTypes.Role, CustomEd.Shared.Model.Role.Teacher.ToString());
        
        });
});

builder.Services.AddControllers();
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

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
