using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using TalkGPT.Utils;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Maui.Storage;
//using TalkGPT.Platforms;

namespace TalkGPT;

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

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

        builder.Services.AddAntDesign();
        builder.Services.AddBootstrapBlazor();

        Debug.WriteLine("");
		Debug.WriteLine(FileSystem.AppDataDirectory);
        Debug.WriteLine("");

        using var stream = FileSystem.OpenAppPackageFileAsync("TalkGPT.appsettings.json").Result;
        var config = new ConfigurationBuilder().AddJsonStream(stream).Build();
        builder.Configuration.AddConfiguration(config);

        builder.Services.AddSingleton<SDKSettingsService>();
        builder.Services.AddSingleton<GPTSDK>();
		builder.Services.AddSingleton<AzureSpeechSDK>();
        //builder.Services.AddSingleton<ISpeechToText, SpeechToTextImplementation>();

        return builder.Build();
	}
}
