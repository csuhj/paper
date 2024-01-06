using paper.Hubs;
using paper.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddSingleton<GameHubMediator>();
builder.Services.AddSingleton<GameEngineService>();

var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("", context =>
{
    context.Response.Redirect("./index.html", permanent: false);
    return Task.FromResult(0);
});

app.MapHub<GameHub>("/hub");

app.Services.GetRequiredService<GameEngineService>().Start();
app.Run();
