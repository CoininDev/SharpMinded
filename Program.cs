using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SharpMinded.Middlewares;
// Supabase connection
Env.Load();
var url = Environment.GetEnvironmentVariable("SUPABASE_URL")
    ?? throw new InvalidOperationException("SUPABASE_URL envvar not configured");
var key = Environment.GetEnvironmentVariable("SUPABASE_KEY")
    ?? throw new InvalidOperationException("SUPABASE_URL envvar not configured");

var options = new Supabase.SupabaseOptions { AutoConnectRealtime = true };

if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(key))
{
    Console.WriteLine("Error: Couldn't load URL or Service Role Key from environment");
    return;
}

var supabase = new Supabase.Client(url, key, options);
await supabase.InitializeAsync();

// App
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(supabase);
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"{url}/auth/v1";
        options.Audience = "authenticated";
        options.RequireHttpsMetadata = true;
    });

builder.Services.AddAuthorization();
builder.Services.AddOpenApi();
builder.Services.AddControllers();

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();