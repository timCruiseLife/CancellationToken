using Example.Models;
using NLog.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);


builder.Logging.AddNLog("nlog.config");
builder.Services.AddDbContext<ExamplesContext>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAny",
        builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ExamplesWebAPI", Version = "v1" });
});

var app = builder.Build();

app.UseCors("AllowAny");

app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ExamplesWebAPI v1"));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

app.Logger.LogInformation("init");