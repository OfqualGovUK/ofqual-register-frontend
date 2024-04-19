using Microsoft.Net.Http.Headers;
using Ofqual.Common.RegisterFrontend.RegisterAPI;
using Refit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//builder.Services.AddHttpClient("OfqualRegisterAPI", httpClient =>
//{
//    httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("APIUrl")!);
//});

builder.Services.AddRefitClient<IRegisterAPIClient>().ConfigureHttpClient(httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.Configuration["RegisterAPIUrl"]!);
    
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
