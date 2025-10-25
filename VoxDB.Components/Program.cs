using Microsoft.EntityFrameworkCore;
using VoxDB.Components.Common.DTOs;
using VoxDB.Components.Common.Services;
using VoxDB.Components.Common.Services.Interfaces;
using VoxDB.Components.Components;
using VoxDB.Entities.DbContext;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var cs = builder.Configuration.GetConnectionString("Sqlite") ?? "Data Source=vox.db";
builder.Services.AddDbContext<VoxDbContext>(o => o.UseSqlite(cs));

builder.Services.AddScoped<CommandInterpreter>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IVoiceService, BrowserVoiceService>();
builder.Services.AddScoped<ILanguageService, LanguageService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<VoxDbContext>();
    db.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

