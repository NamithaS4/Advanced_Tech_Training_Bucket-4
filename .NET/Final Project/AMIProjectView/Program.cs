using Microsoft.AspNetCore.Authentication.Cookies;
using AMIProjectView.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// HttpContext for claims
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Handler that attaches JWT from cookie claims
builder.Services.AddTransient<TokenDelegatingHandler>();

// HttpClient -> AMIProjectAPI (on https://localhost:7168)
builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri("https://localhost:7168/");
    client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
})
.AddHttpMessageHandler<TokenDelegatingHandler>()
// DEV ONLY: bypass self-signed cert validation if needed
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    if (builder.Environment.IsDevelopment())
    {
        handler.ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true;
    }
    return handler;
});

// Cookie authentication for MVC site
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

// Authorization policies mirroring the API
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserPolicy", p => p.RequireClaim("UserType", "User"));
    options.AddPolicy("ConsumerPolicy", p => p.RequireClaim("UserType", "Consumer"));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
