using FeedbackTrackerCommon.Definitions;
using System.Text.Json.Nodes;

namespace Server
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add API Controllers
			builder.Services.AddControllers();

			// Add Swagger
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			var app = builder.Build();

			// Add Swagger Middleware
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.MapControllers();

			app.Run();

		}
	}
}
