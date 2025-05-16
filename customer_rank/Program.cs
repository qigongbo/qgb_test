
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Collections;

namespace customer_rank
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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.ConfigObject.TryItOutEnabled = false;
                    // 设置文档全部展开
                    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.Full);
                });
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
