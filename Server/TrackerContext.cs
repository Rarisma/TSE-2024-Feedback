using System;
using FeedbackTrackerCommon.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Server
{
    public class TrackerContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Feedback> feedback { get; set; }
        public DbSet<Modules> modules { get; set; }
        public DbSet<FeedbackFolders> feedback_folders { get; set; }
        public DbSet<UsersModules> users_modules { get; set; }
        public DbSet<FeedbackComments> feedback_comments { get; set; }
        public DbSet<FolderLinks> folder_links { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
	        string ConnectionString =
		        "Server=www.hallon.rarisma.net;Database=feedbacktracker;User ID=trackeradmin;Password=RaZZmATazz0043_@@!;";

			optionsBuilder.UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString));

        }
    }
}