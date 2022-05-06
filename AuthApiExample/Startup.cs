using System.Security.Authentication;
using AuthApiExample.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Mkb.Auth.Contracts.DataStore;
using Mkb.Auth.DataMongo;
using Mkb.Auth.DataMongo.Repo;
using Mkb.Auth.DataSql;
using Mkb.Auth.Middleware;
using Mkb.Auth.Middleware.AuthSchemaHandler;
using Mkb.Auth.Middleware.Middleware;
using Mkb.Auth.Services;
using MongoDB.Driver;
using Pluralize.NET;

namespace AuthApiExample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddAuthentication(options =>
            {
                // DOES Routing for bad results
                options.DefaultChallengeScheme = "Default";
                options.DefaultForbidScheme = "Default";
                options.AddScheme<SchemeHandler>("Default", "Default");
            });
            // configure strongly typed settings object
            services.Configure<TokenSettings>(Configuration.GetSection("TokenSettings"));
            services.Configure<AuthSettings>(Configuration.GetSection("AuthSettings"));
            services.Configure<ConnectionsSettings>(Configuration.GetSection("ConnectionStrings"));

            // DB CONTEXTS
            services.AddDbContext<AuthDbContext>((f, options) =>
                {
                    options.UseSqlServer(f.GetService<IOptions<ConnectionsSettings>>().Value.AuthSql);
                    options.UseLazyLoadingProxies();
                }
            );

            services.AddSingleton(t =>
            {
                var settings = MongoClientSettings.FromConnectionString(t.GetService<IOptions<ConnectionsSettings>>().Value.AuthMongo);
                settings.SslSettings = new SslSettings {EnabledSslProtocols = SslProtocols.Tls12};
                return settings;
            });
            
            services.AddScoped<IMongoRepository>(f =>
                new MongoRepository(f.GetService<IOptions<ConnectionsSettings>>().Value.AuthDbNameMongo,
                    f.GetService<IPluralize>(), f.GetService<MongoClient>()));
            services.AddSingleton<IPluralize, Pluralizer>();
            services.AddScoped<MongoClient>();
            
            
            services.AddScoped<IAuthDataStoreContract, AuthDataStoreMongo>();
            // Services
            services.AddScoped<IUserService, UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseMiddleware<JwtMiddleware>();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}