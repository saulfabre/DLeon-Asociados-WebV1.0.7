using DLeon_Asociados_Web.Components;
using DLeon_Asociados_Web.Context;
using DLeon_Asociados_Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContextFactory<Contexto>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlConStr")));

builder.Services.AddScoped<VehiculosServices>();
builder.Services.AddScoped<SegurosServices>();
builder.Services.AddScoped<ClientesServices>();
builder.Services.AddScoped<GarantiasServices>();
builder.Services.AddScoped<RepuestosServices>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
