using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Server;




var builder = WebApplication.CreateBuilder(args);

//Add API Controllers
builder.Services.AddControllers();

//Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configure Serilog
Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.WriteTo.File("logs/log-server.txt", rollingInterval: RollingInterval.Day)
	.CreateLogger();

//Configure Entity Framework Core with MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TrackerContext>(options =>
	options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

//Enable Serilog.
builder.Host.UseSerilog();

//Register Services for Dependency Injection
builder.Services.AddScoped<AuthService>();

//Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings.GetValue<string>("SecretKey");
var issuer = jwtSettings.GetValue<string>("Issuer");
var audience = jwtSettings.GetValue<string>("Audience");

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = "JwtBearer";
	options.DefaultChallengeScheme = "JwtBearer";
})
.AddJwtBearer("JwtBearer", options =>
{
	//Check secret key is valid.
	if (secretKey == null) { throw new Exception("Secret key is null"); }

	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = issuer,
		ValidAudience = audience,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
	};
});

builder.Services.AddAuthorization();

// Build the app
var app = builder.Build();


// Enable Swagger in Development
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Apply CORS Policy
app.UseCors("AllowBlazorClient");

// Enable Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map Controllers
app.MapControllers();

// Run the application
app.Run();
