using EMSApi.Models;
using EMSApi.Services;
using Microsoft.EntityFrameworkCore;

namespace EMSApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddScoped<IEntityService<Employee>, EmployeeService>();
            builder.Services.AddScoped<EmployeeService>();
            builder.Services.AddScoped<IAuditService, AuditService>();
            builder.Services.AddDbContext<EMSdbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddControllers().AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                x.JsonSerializerOptions.WriteIndented = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();


            app.MapControllers();

            app.Run();
        }
    }
}
