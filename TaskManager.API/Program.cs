using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.API;
using TaskManager.Data;
using TaskManager.Identity;
using TaskManager.Proto;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddDbContext<TaskManagerDbContext>(options =>
    options.UseSqlServer(builder.Configuration["ConnectionStrings:TaskManagerDatabase"]));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<TaskManagerDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<TaskManagerClient>();
builder.Services.AddLogging();
builder.Services.AddControllers();

builder.Services.AddGrpcClient<TaskManagerGRPCService.TaskManagerGRPCServiceClient>(o =>
{
    o.Address = new Uri(builder.Configuration["GrpcClient:Url"]!);
    o.ChannelOptionsActions.Add(channelOptions =>
    {
        channelOptions.Credentials = ChannelCredentials.SecureSsl;
    });
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = PathString.Empty;
    options.AccessDeniedPath = PathString.Empty;
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});

var app = builder.Build();

app.UseRouting();

app.UseMiddleware<BasicAuthMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();