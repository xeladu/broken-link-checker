using System.Reflection;

using BrokenLinkChecker.Utils.Models;
using BrokenLinkChecker.Utils;

using Microsoft.OpenApi.Models;

namespace BrokenLinkChecker.Api;

/// <summary>
/// The main program
/// </summary>
public static class Program
{
    /// <summary>
    /// The main function
    /// </summary>
    /// <param name="args">Passed arguments</param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Broken Link Checker",
                Description = "Checks a given website for broken links",
                Contact = new OpenApiContact
                {
                    Name = "xeladu",
                    Url = new Uri("https://quickcoder.org/me")
                }
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        builder.Services.AddTransient<LinkCollector>();
        builder.Services.AddTransient<LinkChecker>();

        var app = builder.Build();

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
