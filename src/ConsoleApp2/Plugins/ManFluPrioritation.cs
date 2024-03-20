using System.ComponentModel;
using Microsoft.SemanticKernel;
namespace ConsoleApp2.Plugins;

public class ManFluPrioritization
{
    [KernelFunction]
    [Description("Tells whether a message is important")]
    public static bool IsImportant(
        [Description("Message related to cold")] string messageToEvaluate)
        => messageToEvaluate.ToLower().Contains("cold") || messageToEvaluate.ToLower().Contains("manflu");
}