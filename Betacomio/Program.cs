

//using Betacomio.Authentication;
//using Betacomio.Models;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.EntityFrameworkCore;
//using System.Text.Json.Serialization;

//namespace Betacomio
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);

//            // Add services to the container.

//            builder.Services.AddControllers();
//            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//            builder.Services.AddEndpointsApiExplorer();
//            builder.Services.AddSwaggerGen();

//            builder.Services.AddDbContext<AdventureWorksLt2019Context>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("MainDB")));
//            builder.Services.AddDbContext<AdventureLoginContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("LoginDB")));
//            builder.Services.AddControllers().AddJsonOptions(jsOpt => jsOpt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);


//            //builder.Services.AddAuthentication()
//            //   .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", opt => { });

//            builder.Services.AddAuthentication("BasicAuthentication")
//    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

//            builder.Services.AddAuthorization(opt =>
//                opt.AddPolicy("BasicAuthentication", new AuthorizationPolicyBuilder("BasicAuthentication").RequireAuthenticatedUser().Build()));



//            builder.Services.AddCors(opt =>
//            {
//                opt.AddDefaultPolicy(
//                    build =>
//                    {
//                        //build.WithOrigins("")
//                        build.SetIsOriginAllowed(origin => true)
//                        .AllowAnyHeader()
//                        .AllowAnyMethod()
//                        .AllowCredentials();
//                    });
//            });


//            var app = builder.Build();

//            // Configure the HTTP request pipeline.
//            if (app.Environment.IsDevelopment())
//            {
//                app.UseSwagger();
//                app.UseSwaggerUI();
//            }

//            app.UseHttpsRedirection();

//            app.UseCors();

//            app.UseAuthentication();
//            app.UseAuthorization();  //questa c'era prima



//            app.MapControllers();

//            app.Run();
//        }
//    }
//}


using Betacomio.Authentication;
using Betacomio.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Betacomio
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            

            builder.Services.AddDbContext<AdventureWorksLt2019Context>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("MainDB")));
            builder.Services.AddDbContext<AdventureLoginContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("LoginDB")));

            // Add JSON options with ReferenceHandler.IgnoreCycles
            builder.Services.AddControllers().AddJsonOptions(jsOpt => jsOpt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            builder.Services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
            });

            builder.Services.AddAuthorization(opt =>
                opt.AddPolicy("BasicAuthentication", new AuthorizationPolicyBuilder("BasicAuthentication").RequireAuthenticatedUser().Build()));

            builder.Services.AddCors(opt =>
            {
                opt.AddDefaultPolicy(
                    build =>
                    {
                        build.SetIsOriginAllowed(origin => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                    });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            

            app.UseHttpsRedirection();

            app.UseCors();

            app.UseAuthentication();  // Prima dell'UseAuthorization()
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
