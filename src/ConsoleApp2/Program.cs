// See https://aka.ms/new-console-template for more information

using ConsoleApp2.Plugins;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

// ReSharper disable UseConfigureAwaitFalse

var kernelBuilder = Kernel.CreateBuilder();
var config = new
{
    modelId = "gpt-4",
    azureEndpoint= "",
    apiKey = ""
};
// Load plugins
kernelBuilder.Plugins.AddFromType<MessageProvider>();
kernelBuilder.Plugins.AddFromType<ManFluPrioritization>();
kernelBuilder.AddAzureOpenAIChatCompletion(config.modelId, config.azureEndpoint, config.apiKey);
var kernel = kernelBuilder.Build();
var executionSettings = new OpenAIPromptExecutionSettings
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

// Load prompts
var prompts = kernel.CreatePluginFromPromptDirectory("./../../../Prompts");

var chatService = kernel.GetRequiredService<IChatCompletionService>();

string userRequest = string.Empty;

while(true)
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write("User: ");
    
    Console.ForegroundColor = ConsoleColor.Green;
    userRequest = Console.ReadLine();
    
    Console.ForegroundColor = ConsoleColor.Red;

    var intent = await kernel.InvokeAsync(prompts["Intent"], 
        new () { { "userRequest", userRequest } });
    
    if (string.Equals(intent.ToString(), "EndConversation")) break;
    
    var response = await chatService.GetChatMessageContentAsync(userRequest!, executionSettings, kernel: kernel);
    Console.WriteLine(response);
}

Console.WriteLine("System > Ok, goodbye!");
Console.ReadKey();

