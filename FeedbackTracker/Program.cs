using Application.Components;
using Microsoft.AspNetCore.Components;

namespace Application;

public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddRazorComponents()
				.AddInteractiveServerComponents();
			
			builder.Services.AddSingleton<FeedbackApiClient>(sp => 
                new FeedbackApiClient("http://localhost:5189"));
			builder.Services.AddSingleton<UserAPI>(sp =>
				new UserAPI("http://localhost:5189"));
		builder.Services.AddScoped<JAuth>();


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

