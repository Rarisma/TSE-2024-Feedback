using FeedbackTrackerCommon.Definitions;
using System.Text.Json.Nodes;

namespace Server
{
	public class Program
	{
		private static readonly HttpClient client = new HttpClient();


		// Add feedback request
		private static string CreateFeedback()
		{
			// Insert new feedback object into db

			// test http request
			var responseString = client.GetStringAsync("https://localhost:7184/getFeedback").Result;

			//respond with success or fail
			return $"Created a new feedback\n Get feedback: {responseString}";
		}

		// Complete feedback
		private static string CompleteFeedback()
		{
			// Alter Feedback in db

			//respond with success or fail
			return "Updated Completed feedback";
		}

		// Edit feedback
		private static string UpdateFeedback()
		{
			// Alter Feedback in db

			//respond with success or fail
			return "changed feedback";
		}

		// Get feedback
		private static JsonNode GetFeedback() 
		{
			// return a specific feedback

			return "feedback";
		}

		// Get All feedback for user
		private static string[] GetAllFeedback()
		{
			// return all feedbacks
			return new[] { "feedback 1", "feedback 2" };
		}
		

		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			var app = builder.Build();

			

			// base route
			app.MapGet("/", () => "Base route");

			// Setup endpoints
				// #To-Do: Any request that change data should be post requests

			// create
			app.MapGet("/create", CreateFeedback);

			// complete
			app.MapGet("/complete", CompleteFeedback);

			// update
			app.MapGet("/update",UpdateFeedback);

			// get feedback
			app.MapGet("/getFeedback", GetFeedback);

			// get all feedback
			app.MapGet("/getAllFeedback",GetAllFeedback);


			// healthz
				// check if service running
			app.MapGet("/healthz", () => "200 ok");

			app.Run();
		}
	}
}
