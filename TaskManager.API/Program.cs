using TaskManager.Proto;
using TaskManager.API;
using TaskManager.Identity;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskManager.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Leer la configuración para un proyecto específico
var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
var kestrelConfig = builder.Configuration.GetSection($"Kestrel:{assemblyName}");

// Configurar Kestrel con los puertos específicos del proyecto
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(kestrelConfig["Http:Port"]));  // Usar el puerto HTTP específico
    options.ListenAnyIP(int.Parse(kestrelConfig["Https:Port"]), listenOptions =>
    {
        listenOptions.UseHttps(); // Configurar HTTPS
    });
});

builder.Services.AddDbContext<TaskManagerDbContext>(options =>
    options.UseSqlServer(builder.Configuration["ConnectionStrings:TaskManagerDatabase"])); 

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<TaskManagerDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
        };
    });

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
//});



// Add services to the container.
builder.Services.AddGrpcClient<TaskManagerGRPCService.TaskManagerGRPCServiceClient>(o =>
{
    o.Address = new Uri(builder.Configuration["GrpcClient:Url"]!);
    o.ChannelOptionsActions.Add(channelOptions =>
    {
        channelOptions.Credentials = ChannelCredentials.SecureSsl;
    });
});

builder.Services.AddScoped<TaskManagerClient>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddLogging();
builder.Services.AddControllers();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
