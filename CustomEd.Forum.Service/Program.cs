using CustomEd.Forum.Service.Model;
using CustomEd.Forum.Service.Policies;
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

// Add services to the container.
builder.Services.AddMongo()
    .AddPersistence<User>("User")
    .AddPersistence<Message>("Message")
    .AddPersistence<Classroom>("Classroom");
builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddAuth();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MemberOnlyPolicy", policy =>
        policy.Requirements.Add(new MemberOnlyRequirement()));
});

builder.Services.AddScoped<IAuthorizationHandler, MemberOnlyPolicy>();
builder.Services.AddMassTransitWithRabbitMq();
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

app.MapHub<ForumHub>("/forumHub");

app.Run();
