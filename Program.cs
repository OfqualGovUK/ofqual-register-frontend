using Azure.Identity;
using GovUk.Frontend.AspNetCore;
using Microsoft.Extensions.Azure;
using Ofqual.Common.RegisterFrontend.BlobStorage;
using Ofqual.Common.RegisterFrontend.Cache;
using Ofqual.Common.RegisterFrontend.RegisterAPI;
using Ofqual.Common.RegisterFrontend.UseCases.Qualifications;
using Refit;


var builder = WebApplication.CreateBuilder(args);

//Add GovUK Frontend assets
builder.Services.AddGovUkFrontend(options => 
{ 
    options.Rebrand = true; 
});

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

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(new Uri(builder.Configuration["StorageContainerUrl"]!));
    clientBuilder.UseCredential(new DefaultAzureCredential());
});

builder.Services.AddSingleton<IRefDataCache, RefDataCache>();
builder.Services.AddSingleton<IBlobService, BlobService>();

//usecases
builder.Services.AddScoped<IQualificationsUseCases, QualificationsUseCases>();

builder.Services.AddWebOptimizer();

//healthcheck
builder.Services.AddHealthChecks();

var app = builder.Build();

//Add GovUk Frontend to the middleware pipeline
app.UseGovUkFrontend();

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

    if (ctx.Response.StatusCode != 200 && !ctx.Response.HasStarted)
    {
        //Re-execute the request so the user gets the error page
        string originalPath = ctx.Request.Path.Value;
        ctx.Items["originalPath"] = originalPath;
        ctx.Request.Path = $"/error/{ctx.Response.StatusCode}";
        await next();
    }
});

app.UseStatusCodePagesWithRedirects("/error/{0}");

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/robots.txt"))
    {
        var robotsTxtPath = Path.Combine(app.Environment.ContentRootPath, "robots.txt");
        string output = "User-agent: *  \nDisallow: /";
        if (File.Exists(robotsTxtPath))
        {
            output = await File.ReadAllTextAsync(robotsTxtPath);
        }
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync(output);
    }
    else if (context.Request.Path.StartsWithSegments("/sitemap.xml"))
    {
        var robotsTxtPath = Path.Combine(app.Environment.ContentRootPath, "sitemap.xml");
        string output = "User-agent: *  \nDisallow: /";
        if (File.Exists(robotsTxtPath))
        {
            output = await File.ReadAllTextAsync(robotsTxtPath);
        }
        context.Response.ContentType = "application/xml";
        await context.Response.WriteAsync(output);
    }
    else await next();
});

// Configure the health check endpoint
app.MapHealthChecks("/health");

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
