using Application.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;

namespace Application;

public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddRazorComponents()
				.AddInteractiveServerComponents();
			builder.Services.AddHttpContextAccessor();

			builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
			.AddCookie(options =>
			{
				options.LoginPath = "/login"; // redir to login page
				options.AccessDeniedPath = "/Unauthorised"; //redir to unauthorised page if not logged in
				options.Cookie.HttpOnly = true; 
				options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Ensure cookies are only sent over HTTPS
				options.Cookie.SameSite = SameSiteMode.Strict; // Mitigate CSRF
				options.ExpireTimeSpan = TimeSpan.FromMinutes(600); // Cookie expire every 10hrs
			});

			//Add default authentication policy to require users to login
			builder.Services.AddAuthorization(options =>
			{
				options.DefaultPolicy = new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme)
					.RequireAuthenticatedUser()
					.Build();
			}); 
			
			builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}

			app.UseStatusCodePagesWithReExecute("/StatusCode/{0}");

			app.UseHttpsRedirection();
			app.UseAuthentication();
			app.UseAuthorization();
			app.UseStaticFiles();
			app.UseAntiforgery();

			app.MapRazorComponents<App>()
				.AddInteractiveServerRenderMode();

        app.Run();
		}
	}

