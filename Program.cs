using Microsoft.Net.Http.Headers;
using Ofqual.Common.RegisterFrontend.Cache;
using Ofqual.Common.RegisterFrontend.RegisterAPI;
using Ofqual.Common.RegisterFrontend.UseCases.Qualifications;
using Refit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add register API 
builder.Services.AddRefitClient<IRegisterAPIClient>().ConfigureHttpClient(httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.Configuration["RegisterAPIUrl"]!);

});

// Add Ref Data API
builder.Services.AddRefitClient<IRefDataAPIClient>().ConfigureHttpClient(httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.Configuration["RefDataAPIUrl"]!);

});

builder.Services.AddSingleton<IRefDataCache, RefDataCache>();

//usecases
builder.Services.AddScoped<IQualificationsUseCases, QualificationsUseCases>();

builder.Services.AddWebOptimizer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error/500");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    builder.Services.AddWebOptimizer(minifyJavaScript: false, minifyCss: false);
}

app.Use(async (ctx, next) =>
{
    await next();

    if (ctx.Response.StatusCode == 404 && !ctx.Response.HasStarted)
    {
        //Re-execute the request so the user gets the error page
        string originalPath = ctx.Request.Path.Value;
        ctx.Items["originalPath"] = originalPath;
        ctx.Request.Path = "/error/404";
        await next();
    }

    if (ctx.Response.StatusCode == 400 && !ctx.Response.HasStarted)
    {
        //Re-execute the request so the user gets the error page
        string originalPath = ctx.Request.Path.Value;
        ctx.Items["originalPath"] = originalPath;
        ctx.Request.Path = "/error/500";
        await next();
    }
});

app.UseStatusCodePagesWithRedirects("/error/{0}");

app.UseHttpsRedirection();

app.UseWebOptimizer();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
