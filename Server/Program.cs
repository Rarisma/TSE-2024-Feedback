using FeedbackTrackerCommon.Definitions;
using System.Text.Json.Nodes;

namespace Server
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			//Add API Controllers
			builder.Services.AddControllers();
			var app = builder.Build();

			app.MapControllers();

			app.Run();
		}
	}
}
