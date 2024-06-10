using CustomEd.Assessment.Service.AnalyticsSevice;
using CustomEd.Assessment.Service.Model;
using CustomEd.Assessment.Service.Policies;
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

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMongo()
                .AddPersistence<Answer>("Answers")
                .AddPersistence<Assessment>("Assessment")
                .AddPersistence<Classroom>("Classrooms")
                .AddPersistence<Question>("Questions")
                .AddPersistence<Submission>("Submissions")
                .AddPersistence<Analytics>("Analytics");
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddAuth();
builder.Services.AddScoped<AnalysisService>();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CreatorOnly", policy =>
        policy.Requirements.Add(new CreatorOnlyRequirement()));
    options.AddPolicy("StudentOnly", policy =>
        policy.Requirements.Add(new StudentOnlyRequirement()));
    options.AddPolicy("MemberOnly", policy =>
        policy.Requirements.Add(new MemberOnlyRequirement()));
});
builder.Services.AddScoped<IAuthorizationHandler, StudentOnlyPolicy>();
builder.Services.AddScoped<IAuthorizationHandler, CreatorOnlyPolicy>();
builder.Services.AddScoped<IAuthorizationHandler, MemberOnlyPolicy>();
builder.Services.AddMassTransitWithRabbitMq("AssessmentServiceQueue");

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
