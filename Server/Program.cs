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

			app.MapGet("/", () => "Hello World!");
			app.MapControllers();

			app.Run();
		}
	}
}
