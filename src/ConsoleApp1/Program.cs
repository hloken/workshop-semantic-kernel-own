// See https://aka.ms/new-console-template for more information

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
// ReSharper disable UseConfigureAwaitFalse

var kernelBuilder = Kernel.CreateBuilder();
var config = new
{
    modelId = "gpt-4",
    azureEndpoint= "",
    apiKey = ""
};
kernelBuilder.AddAzureOpenAIChatCompletion(config.modelId, config.azureEndpoint, config.apiKey);
var kernel = kernelBuilder.Build();

var chatService = kernel.GetRequiredService<IChatCompletionService>();

string line = string.Empty;
ChatHistory chatHistory = [];

while(true)
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write("User: ");
    
    Console.ForegroundColor = ConsoleColor.Green;
    line = Console.ReadLine()!;
    chatHistory.AddUserMessage(line);
    
    if (line == "q!")
        break;
    
    Console.ForegroundColor = ConsoleColor.Red;
    var message = "";
    await foreach (var chunk in chatService.GetStreamingChatMessageContentsAsync(
                       chatHistory:chatHistory, 
                       kernel: kernel))
    {
        message += chunk;
        chatHistory.AddAssistantMessage(message);
        Console.Write(chunk);
        
        await Task.Delay(TimeSpan.FromMilliseconds(10));
    }

    Console.Write("\r\n");
}
