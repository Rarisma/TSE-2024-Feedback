using Application.API;
using Application.Components;
using Microsoft.AspNetCore.Components;
using Serilog;

namespace Application;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddRazorComponents()
			.AddInteractiveServerComponents();
			
		builder.Services.AddSingleton<FeedbackAPI>(sp => 
            new FeedbackAPI("http://localhost:5189"));
		builder.Services.AddSingleton<UserAPI>(sp =>
			new UserAPI("http://localhost:5189"));
		builder.Services.AddScoped<JAuth>();

		builder.Services.AddSingleton<UserAPI>(sp =>
		new UserAPI("http://localhost:5189"));

		//Configure Serilog
		Log.Logger = new LoggerConfiguration()
			.WriteTo.Console()
			.WriteTo.File("logs/log-app.txt", rollingInterval: RollingInterval.Day)
			.CreateLogger();

		//Enable Serilog.
		builder.Host.UseSerilog();


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

		//JAuth init stuff
		using (var scope = app.Services.CreateScope())
		{
			var navigationManager = scope.ServiceProvider.GetRequiredService<NavigationManager>();
		}

		app.Run();
	}
}

