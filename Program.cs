using Microsoft.Extensions.FileProviders;

using Entity.AppDbContext;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(Configuration["pgConnection"]));

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);

var app = builder.Build();

var cacheMaxAgeOneWeek = (60 * 60 * 24 * 7).ToString();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "StaticFiles")
    ),
    RequestPath = "/StaticFiles",
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append(
             "Cache-Control", $"public, max-age={cacheMaxAgeOneWeek}");
    }
});

app.MapControllers();

app.Run();
