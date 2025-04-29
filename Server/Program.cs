using System.Text;
using Core.Definitions;
using Microsoft.AspNetCore.Identity;
using dotenv.net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Radzen;

namespace Server;

internal static class Program
{
	public static IDictionary<string, string>? Secrets;
	public static void Main(string[] args)
	{
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
		builder.Services.AddDbContext<TrackerContext>();

		//Enable Serilog.
		builder.Host.UseSerilog();

		//Register Services for Dependency Injection
		builder.Services.AddScoped<AuthService>();

        builder.Services.AddScoped<INotificationService, NotificationService>();

        DotEnv.Load();
		Secrets = DotEnv.Read();
		
		builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = "JwtBearer";
				options.DefaultChallengeScheme = "JwtBearer";
			})
			.AddJwtBearer("JwtBearer", options =>
			{
				//Check secret key is valid.
				if (!Secrets.ContainsKey("JWTSecret"))
				{
					throw new Exception("Secret key is null");
				}

				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = Secrets["JWTEndpoint"],
					ValidAudience = Secrets["JWTEndpoint"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secrets["JWTSecret"]))
				};
			});

		builder.Services.AddAuthorization();

		builder.Services.AddSingleton<EmailSending>();
		// Build the app
		var app = builder.Build();

		// Delete old users on startup of server
		using (var scope = app.Services.CreateScope())
		{
			scope.ServiceProvider.GetRequiredService<TrackerContext>();
			DeleteOldUsers();
		}


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
	}

	[HttpDelete("DeleteOldUsers")]
	private static void DeleteOldUsers()
	{
        try
        {
            // Stores the date 1 year ago
            using TrackerContext ctx = new();
            var oneYearAgo = DateTime.Now.AddYears(-1);

            // compares last login with one year ago
            var oldUsers = ctx.User
                .Where(user => user.LastLogin.HasValue && user.LastLogin.Value.Year < oneYearAgo.Year).ToList();

            if (oldUsers.Any())
            {
                // gets old user ids 
                var oldUserIds = oldUsers.Select(u => u.UserID).ToList();

                // checks if they have any feedbacks
                var userFeedbacks = ctx.Feedback.ToList()
                    .Where(f => oldUserIds.Contains(f.AssigneeID) ||
                                (f.AssignedUserID.HasValue && oldUserIds.Contains(f.AssignedUserID.Value))).ToList();


                Log.Information("deleting old users.. ");

                // Removes user feedbacks and old users
                ctx.Feedback.RemoveRange(userFeedbacks);
                ctx.User.RemoveRange(oldUsers);

                foreach (var usr in oldUsers)
                {
                    Log.Information("deleted: " + usr.Username);
                }
                ctx.SaveChanges();

            }
            else
            {
                Log.Information("no old users.. ");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to delete old users");
        }
    }
}