using Microsoft.Extensions.Logging;
using MyHybridApp2.UI;

namespace MyHybridApp2;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
		builder.Services.AddMyHybridApp2UI(GetApiBaseAddress());

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}

	private static Uri GetApiBaseAddress()
	{
#if DEBUG
		// Local API (MyHybridApp2.Api "http" profile). The Android emulator reaches the host machine via 10.0.2.2.
		var host = DeviceInfo.Platform == DevicePlatform.Android ? "10.0.2.2" : "localhost";
		return new Uri($"http://{host}:5206/");
#else
		// TODO: Point at the deployed API.
		return new Uri("https://api.example.com/");
#endif
	}
}
