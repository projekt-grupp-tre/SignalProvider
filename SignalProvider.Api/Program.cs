using Microsoft.AspNetCore.Cors.Infrastructure;
using SignalProvider.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();

builder.Services.AddCors(options => 
{
	options.AddPolicy("AllowMe", 
		policy => 
		{
			policy
				.WithOrigins("http://127.0.0.1:5500/")
				.AllowAnyHeader()
				.AllowAnyMethod()
				.AllowCredentials()
				.SetIsOriginAllowed(_ => true);
		});
});

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("AllowMe");
app.MapHub<ChatHub>("/chatHub");
app.MapHub<NotificationHub>("/notificationHub"); 

app.MapGet("/", () => "Hello world");

app.Run();