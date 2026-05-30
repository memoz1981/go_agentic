using Microsoft.Extensions.AI;

namespace UdemyAICourseNotes.Extensions; 

public static class ResponseExtensions
{
    public static string Counts(this UsageDetails details)
        => details == null ? ""
        : $"Token Usage: input: {details.InputTokenCount}, output: {details.OutputTokenCount}, reasoning: {details.ReasoningTokenCount}";
}
