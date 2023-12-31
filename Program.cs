using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using TaskCircle.GroupManagerApi.Infrastructure.Repositories;
using TaskCircle.GroupManagerApi.Infrastructure.Repositories.Interfaces;
using TaskCircle.GroupManagerApi.Infrastructure.Services;
using TaskCircle.GroupManagerApi.Infrastructure.Services.Interfaces;
using TaskCircle.UserManagerApi.Infrastructure.Setting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Group",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GroupApi",
        Version = "v1",
        Description = "Group manager Api",
        Contact = new OpenApiContact
        {
            Name = "Henrique Ara�jo Neto 2023/24",
            Email = "anhenrique93@gmail.com",
            Url = new Uri("http://anhenrique.netlify.app/")
        },
    });

    var filePath = System.IO.Path.Combine(System.AppContext.BaseDirectory, "GroupManagerApi.xml");
    options.IncludeXmlComments(filePath);

    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer Scheme (\"bearer  {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

//Auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration.GetSection("Jwt:Issuer").Value,
            ValidAudience = builder.Configuration.GetSection("Jwt:Audience").Value,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("Jwt:Key").Value))
        };
    });

// Connection DB
builder.Services.Configure<ConnectionSetting>(builder.Configuration.GetSection("ConnectionSetting"));

// AutoMapper for DTOs
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Repositories and Services
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IGroupService, GroupService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "GroupApi v1");
    });
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "GroupApi v1");
    });
}

app.UseHttpsRedirection();

app.UseCors("Group");

app.UseAuthorization();

app.MapControllers();

app.Run();
