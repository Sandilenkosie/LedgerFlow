using WebUI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddWebUIDI(builder.Configuration);

// Register a named HttpClient for calling the API project. Default base URL can be overridden
// via configuration key "Api:BaseUrl" (e.g. https://localhost:5001/)
builder.Services.AddHttpClient("api", c =>
{
    c.BaseAddress = new Uri(builder.Configuration["Api:BaseUrl"] ?? "https://localhost:5001/");
});

var jwtKey = builder.Configuration["Jwt:Key"] ?? "PleaseChangeThisSecretKeyToASecureRandomValue!ChangeMe";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "LedgerFlow";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "LedgerFlowUsers";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
