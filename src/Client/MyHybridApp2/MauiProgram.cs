using Microsoft.Extensions.Logging;
using MyHybridApp2.UI;

#if MAUI_DEVFLOW
using Microsoft.Maui.DevFlow.Agent;
using Microsoft.Maui.DevFlow.Blazor;
#endif

namespace MyHybridApp2;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		AppDomain.CurrentDomain.FirstChanceException += (s, e) =>
		Console.WriteLine($"[FirstChance] {e.Exception}\n{e.Exception.StackTrace}");

		AppDomain.CurrentDomain.UnhandledException += (s, e) =>
			Console.WriteLine($"[Unhandled] {e.ExceptionObject}");

		TaskScheduler.UnobservedTaskException += (s, e) =>
			Console.WriteLine($"[UnobservedTask] {e.Exception}");

		#if MACCATALYST
		ObjCRuntime.Runtime.MarshalManagedException += (s, e) =>
			Console.WriteLine($"[Managed->ObjC] {e.Exception}");

		ObjCRuntime.Runtime.MarshalObjectiveCException += (s, e) =>
			Console.WriteLine($"[ObjC] {e.Exception.Name}: {e.Exception.Reason}\n" +
				string.Join("\n", e.Exception.CallStackSymbols.Select(x => x.ToString())));
		#endif

		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
		#if MAUI_DEVFLOW
				builder.AddMauiDevFlowAgent();
				builder.AddMauiBlazorDevFlowTools();
		#endif
				builder.Services.AddMyHybridApp2UI(GetApiBaseAddress());

		#if DEBUG
			builder.Services.AddBlazorWebViewDeveloperTools();
			builder.Logging.AddDebug();
			builder.Logging.SetMinimumLevel(LogLevel.Debug);
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
