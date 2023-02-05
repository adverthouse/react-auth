using Adverthouse.Common.Data;
using Adverthouse.Common.Data.Caching;
using Adverthouse.Common.Interfaces;
using Adverthouse.Core.Configuration;
using Adverthouse.Core.Infrastructure;
using back_end.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace back_end.Infrastructure
{
    public static class DependencyInjection
    {
        private static IServiceCollection _services;

        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            
            var settings = configuration.GetSection("AppSettings").Get<AppSettings>(); 

            JToken jAppSettings = JToken.Parse(
                  File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "appsettings.json"))
            );

            settings.AdditionalSettings = (IDictionary<string, JToken>)jAppSettings["AppSettings"]["AdditionalSettings"];
            
            services.AddSingleton<AppSettings>(settings);
            Singleton<AppSettings>.Instance = settings;
            
            services.AddSingleton<IDapperSql>(new DapperSql(configuration.GetConnectionString("BEConnection"),300));

            services.AddDbContext<DataContext>(x => x.UseInMemoryDatabase("TestDb"));
            services.AddTransient(typeof(IRepository<>), typeof(EntityRepository<>));

            //get instance and seed data
     
            services.AddScoped<DbContext, DataContext>();

            services.AddScoped<IMemberService, MemberService>();  

            services.AddHttpContextAccessor();

            _services = services;

            return services;
        }

        public static IServiceProvider GetProvide()
        {
            IServiceProvider _serviceProvider = _services.BuildServiceProvider();
            return _serviceProvider;
        }
    }
}
