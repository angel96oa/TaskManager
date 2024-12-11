using TaskManager.Service;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddLogging();
builder.Services.AddSingleton<RabbitMQService>();

builder.Services.AddDbContext<TaskManagerDbContext>(options =>
    options.UseSqlServer(builder.Configuration["ConnectionStrings:TaskManagerDatabase"]));

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
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.MapGrpcService<TaskManagerService>();
app.MapGet("/", () => "gRPC server is running");

app.Run();