using MVC.Service;

namespace Demo.SPA.Vue
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddMvc(options => options.EnableEndpointRouting = false);
            builder.Services.AddSingleton<DataSetService>();
            builder.Services.AddCors();
            builder.Services.AddFastReport();
            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors(options => options
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin());

            app.UseMvc();

            app.MapControllers();

            app.UseFastReport();

            app.Run();
        }
    }
}