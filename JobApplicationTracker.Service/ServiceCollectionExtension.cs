﻿using JobApplicationTracke.Data.Database;
using JobApplicationTracker.Data.Config;
using JobApplicationTracker.Data.Interface;
using JobApplicationTracker.Data.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JobApplicationTracker.Service
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddServiceLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatabaseConfig>(configuration.GetSection("DatabaseConfig"));

            services.AddScoped<ICompaniesRepository, CompaniesRepository>();
            //services.AddScoped<INotificationsRepository, NotificationsRepository>();
            //services.AddScoped<IAdminActionRepository, AdminActionService>();
            services.AddScoped<IJobApplicationRepository, ApplicationsRepository>();
            services.AddScoped<IJobSeekerExperienceRepository, JobSeekerExperienceRepository>();
            services.AddScoped<IJobSeekersRepository, JobSeekerRepository>();
            services.AddScoped<IJobSeekersSkillsRepository, JobSeekerSkillsRepository>();
            services.AddScoped<ISkillsRepository, SkillsRepository>();
            services.AddScoped<IUserRepository, UsersRepository>();

            services.AddScoped<IDatabaseConnectionService, DatabaseConnectionService>();

            return services;
        }
    }
}
