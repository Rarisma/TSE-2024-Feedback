using Blazored.LocalStorage;
using Application.API;
using Application.Components;
using Serilog;
using Radzen;
using FeedbackTracker.Services;

namespace Application;

public class Program
{
	public static string? JWTSecretKey;
	public static string? JWTIssuer;
	public static string? JWTAudience;
	
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);


		//Add Blazored Local Storage
		builder.Services.AddBlazoredLocalStorage();

		builder.Services.AddRazorComponents()
			.AddInteractiveServerComponents();
			
		builder.Services.AddSingleton<FeedbackAPI>(_ => new FeedbackAPI());
		builder.Services.AddSingleton<UserAPI>(_ => new UserAPI());
        builder.Services.AddSingleton<ModuleAPI>(_ => new ModuleAPI());
        builder.Services.AddScoped<JAuth>();
        builder.Services.AddRadzenComponents();
        builder.Services.AddSingleton<EmailAPI>(_ => new EmailAPI());
        builder.Services.AddSingleton<SchoolAPI>(_ => new SchoolAPI());
        builder.Services.AddSingleton<BulkAPI>(_ => new BulkAPI());
        builder.Services.AddScoped<AppTheme>();

        //Configure Serilog
        Log.Logger = new LoggerConfiguration()
			.WriteTo.Console()
			.WriteTo.File("logs/log-app.txt", rollingInterval: RollingInterval.Day)
			.CreateLogger();

		//Enable Serilog.
		builder.Host.UseSerilog();

		JWTSecretKey = builder.Configuration["JwtSettings:SecretKey"];
		JWTAudience = builder.Configuration["JwtSettings:Issuer"];
		JWTIssuer = builder.Configuration["JwtSettings:Audience"];
		
		var app = builder.Build();

		// Configure the HTTP request pipeline.
		if (!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/Error");
			app.UseHsts();
		}

		app.UseStatusCodePagesWithReExecute("/StatusCode/{0}");

		app.UseHttpsRedirection();
		app.UseStaticFiles();
		app.UseAntiforgery();

		app.MapRazorComponents<App>()
			.AddInteractiveServerRenderMode();

		app.Run();
	}
}

