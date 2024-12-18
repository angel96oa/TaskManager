using TaskManager.Service;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Messaging;
using System.Reflection;
using TaskManager.Identity;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddGrpc();
builder.Services.AddLogging();



builder.Services.Configure<RabbitMQConfiguration>(
    builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddScoped<IRabbitMQService, RabbitMQService>();

builder.Services.AddDbContext<TaskManagerDbContext>(options =>
    options.UseSqlServer(builder.Configuration["ConnectionStrings:TaskManagerDatabase"]));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<TaskManagerDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(Int32.Parse(builder.Configuration["gRPCPort"]!), listenOptions =>
    {
        listenOptions.UseHttps();
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;

    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TaskManagerDbContext>();
    dbContext.Database.Migrate();

    var roles = scope.ServiceProvider.GetRequiredService<IUserRoleService>();
    await roles.InitializeRoles();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.MapGrpcService<TaskManagerService>();
app.MapGet("/", () => "gRPC server is running");

var rabbitService = app.Services.GetRequiredService<RabbitMQService>();
rabbitService.StartListening(); // Inicia el servicio que escucha los mensajes

app.Run();

app.Lifetime.ApplicationStopping.Register(() =>
{
    rabbitService.StopListening(); // Detener la escucha cuando la aplicación se está apagando
});