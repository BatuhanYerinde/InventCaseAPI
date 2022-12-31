using InventCaseAPI.Constants.Configurations;
using InventCaseAPI.Data;
using InventCaseAPI.Data.Abstract;
using InventCaseAPI.Data.Concrete;
using Microsoft.Extensions.Configuration;

namespace InventCaseAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton(builder.Configuration.GetSection("ConnectionStrings").Get<ConnectionStrings>());
            builder.Services.AddScoped<DbConnection>();
            builder.Services.AddScoped<ISaleDAL, SaleDAL>();
            
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