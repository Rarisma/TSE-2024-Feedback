using Blazored.LocalStorage;
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


		//Add Blazored Local Storage
		builder.Services.AddBlazoredLocalStorage();

		builder.Services.AddRazorComponents()
			.AddInteractiveServerComponents();
			
		builder.Services.AddSingleton<FeedbackAPI>(_ => new FeedbackAPI());
		builder.Services.AddSingleton<UserAPI>(_ => new UserAPI());
        builder.Services.AddSingleton<ModuleAPI>(_ => new ModuleAPI());
        builder.Services.AddScoped<JAuth>();


		builder.Services.AddSingleton<EmailAPI>(_ => new EmailAPI());

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

		app.Run();
	}
}

