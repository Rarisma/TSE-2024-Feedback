using FeedbackTrackerCommon.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Server;

public class TrackerContext(
	string connectionString =
		"Server=www.hallon.rarisma.net;Database=feedbacktracker;User ID=trackeradmin;Password=RaZZmATazz0043_@@!;")
	: DbContext
{
	public string ConnectionString { get; set; } = connectionString;

	public DbSet<User> User { get; set; }
	public DbSet<Feedback> Feedback { get; set; }
	public DbSet<Modules> Modules { get; set; }
	public DbSet<FeedbackFolders> FeedbackFolders { get; set; }
	public DbSet<Users_Modules> UsersModules { get; set; }
	public DbSet<FeedbackComments> FeedbackComments { get; set; }
	public DbSet<FolderLinks> FolderLinks { get; set; }
	
    public DbSet<Notification> Notification { get; set; }
	public DbSet<CodeStorage> CodeStorage { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString));
	}
}