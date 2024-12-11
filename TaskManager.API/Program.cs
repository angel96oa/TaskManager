using TaskManager.Proto;
using TaskManager.API;
using Grpc.Core;

var builder = WebApplication.CreateBuilder(args);

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
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
