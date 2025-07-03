using System.Security.Claims;
using System.Text;
using Eshop.RazorPage.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterApiServices();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("Account", builder =>
    {
        builder.RequireAuthenticatedUser();
    });
    option.AddPolicy("SellerPanel", builder =>
    {
       
        builder.RequireAuthenticatedUser();
        builder.RequireAssertion(f => f.User.Claims
            .Any(c => c.Type == ClaimTypes.Role && c.Value.Contains("Seller")));
        var pr = ClaimTypes.Role;
    });

    option.AddPolicy("AdminPanel", builder =>
    {
        builder.RequireAuthenticatedUser();
        builder.RequireAssertion(f => f.User.Claims
            .Any(c => c.Type == ClaimTypes.Role && c.Value.Contains("Admin")));
        var pr = ClaimTypes.Role;
    });
});

builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation()
    .AddRazorPagesOptions(options =>
    {
        options.Conventions.AuthorizeFolder("/Profile", "Account");
        options.Conventions.AuthorizeFolder("/SellerPanel", "SellerPanel");
        options.Conventions.AuthorizeFolder("/Admin", "AdminPanel");
    });

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidAudience = builder.Configuration["JwtConfig:Audience"],
        ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
        IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:SignInKey"])),
        ValidateLifetime = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateIssuerSigningKey = true
    };
});

var app = builder.Build();


// Configure the HTTP request pipeline.


app.Use(async (context, next) =>
{
    var token = context.Request.Cookies["token"]?.ToString();
    if (string.IsNullOrWhiteSpace(token) == false)
    {
        context.Request.Headers.Append("Authorization", $"Bearer {token}");
    }
    await next();
});


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.Use(async (context, next) =>
{
    await next();
    var status = context.Response.StatusCode;
    if (status == 401)
    {
        var path = context.Request.Path;
        context.Response.Redirect($"/auth/login?redirectTo={path}");
    }
    if (status == 404)
    {
        context.Response.Redirect("/Error?code=404");
    }
});

if (app.Environment.IsEnvironment("Production"))
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.Run();