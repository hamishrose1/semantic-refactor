using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.Core;
using SemanticRefactor;
using Kernel = Microsoft.SemanticKernel.Kernel;

#pragma warning disable SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

public static class Program
{
    public static async Task Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<Worker>();

        builder.Services.AddOptions<AzureOpenAI>()
                        .Bind(builder.Configuration.GetSection(nameof(AzureOpenAI)))
                        .ValidateDataAnnotations()
                        .ValidateOnStart();

        builder.Services.AddSingleton<IChatCompletionService>(sp =>
        {
            AzureOpenAI options = sp.GetRequiredService<IOptions<AzureOpenAI>>().Value;
            return new AzureOpenAIChatCompletionService(options.ChatDeploymentName, options.Endpoint, options.ApiKey);
        });

        // Locally deployed ollama model
        //builder.Services.AddSingleton<IChatCompletionService>(sp =>
        //{
        //    return new CustomChatCompletionService()
        //    {
        //        ModelName = "dolphin-mistral",
        //        ModelUrl = "http://20.70.153.239:11434"
        //    };
        //});


        builder.Services.AddSingleton<FileIOPlugin>();
        builder.Services.AddSingleton<PowershellPlugin>();
        builder.Services.AddSingleton<DirectoryIOPlugin>();

        builder.Services.AddKeyedTransient<Kernel>("AngularConversionKernel", (sp, key) =>
        {
            KernelPluginCollection pluginCollection = new();
            pluginCollection.AddFromObject(sp.GetRequiredService<FileIOPlugin>());
            pluginCollection.AddFromObject(sp.GetService<DirectoryIOPlugin>());
            pluginCollection.AddFromObject(sp.GetRequiredService<PowershellPlugin>());

            return new Kernel(sp, pluginCollection);
        });

        using IHost host = builder.Build();

        await host.RunAsync();
    }
}
