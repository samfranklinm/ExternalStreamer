using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ExternalStreamer.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policyBuilder =>
        policyBuilder.WithOrigins("http://localhost:7025") // webapi URL
                     .AllowAnyMethod()
                     .AllowAnyHeader()
                     .AllowCredentials());
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<SentenceHub>("/sentenceHub");
});

app.Run();
