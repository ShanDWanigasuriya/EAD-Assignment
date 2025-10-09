using EVWebApp.Services; // ? Add this at the top
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// ==========================
// 1?? Add MVC + Session
// ==========================
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);  // session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ==========================
// 2?? Add HttpClient + ApiClient service
// ==========================
builder.Services.AddHttpContextAccessor();

// Base URL of your Web API
var apiBaseUrl = builder.Configuration["Api:BaseUrl"] ?? "https://localhost:44338";

builder.Services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

// ==========================
// 3?? Build application
// ==========================
var app = builder.Build();

// ==========================
// 4?? Middleware setup
// ==========================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();             // ? must come before authorization
app.UseAuthorization();

// ==========================
// 5?? MVC route config
// ==========================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
