

using Betacomio.Models;

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
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AdventureWorksLt2019Context>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("MainDB")));
            builder.Services.AddDbContext<AdventureLoginContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("LoginDB")));
            builder.Services.AddControllers().AddJsonOptions(jsOpt => jsOpt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}