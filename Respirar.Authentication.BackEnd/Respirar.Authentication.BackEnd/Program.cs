using Respirar.Authentication.BackEnd.Application.CommandHandlers;
using Respirar.Authentication.BackEnd.Application.Commands;
using Respirar.Authentication.BackEnd.Application.Configuration;
using Respirar.Authentication.BackEnd.Application.DependencyInjection;
using Respirar.Authentication.BackEnd.Application.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string host = builder.Configuration["Keyrock:Host"];
byte[] credentialsBytes = Encoding.UTF8.GetBytes($"{builder.Configuration["Keyrock:ClientId"]}:{builder.Configuration["Keyrock:ClientSecret"]}");
string clientCredentials = Convert.ToBase64String(credentialsBytes);

builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = builder.Configuration["RedisCacheUrl"]; });

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

builder.Services.AddTransient<IMailService, MailService>();

builder.Services.AddCors(policyBuilder =>
    policyBuilder.AddDefaultPolicy(policy =>
        policy.WithOrigins("*").AllowAnyHeader().AllowAnyHeader())
);


builder.Services.AddKeyrockApiClient(httpClient =>
{
    httpClient.BaseAddress = new(host);
    httpClient.AddKeyrockSubjectHeaders(host, clientCredentials);
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
