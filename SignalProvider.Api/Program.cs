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
				.SetIsOriginAllowed(_ => true)
				.AllowAnyHeader()
				.AllowAnyMethod()
				.AllowCredentials();
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