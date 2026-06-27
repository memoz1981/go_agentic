using Anthropic;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using UdemyAICourseNotes.Services.Appointment;
using UdemyAICourseNotes.Tools;


namespace UdemyAICourseNotes.Services.Workflows;

internal class AppointmentWorkflowFactory_Claude
{
    private static readonly string APPOINTMENT_RECORDER_INSTRUCTIONS =
        ReadInstructions("AppointmentRecorderAgent.md");
    private static readonly string APPOINTMENT_PARSER_INSTRUCTIONS =
        ReadInstructions("AppointmentParserAgent.md");
    private static readonly string SLOT_SELECTION_INSTRUCTIONS =
        ReadInstructions("SlotSelectionAgent.md");
    private static readonly string FINAL_CONFIRMATION_AGENT_INSTRUCTIONS =
        ReadInstructions("FinalConfirmationAgent.md");
    private readonly Memory.InMemoryChatHistoryProvider _chatHistoryProvider;

    public AppointmentWorkflowFactory_Claude(Memory.InMemoryChatHistoryProvider chatHistoryProvider)
    {
        _chatHistoryProvider = chatHistoryProvider;
    }

    public AnthropicClient GetClient()
        => AgentClientFactory.GetClaudeClient();

    public AIAgent GetRecorderAgent(AnthropicClient client, AppointmentService appointmentService)
    {
        return AgentClientFactory
            .GetClaudeAgent(
            claudeClient: client,
            model: Models.Claude.SONNET_4_6,
            name: "appointmentRecorderAgent",
            instructions: APPOINTMENT_RECORDER_INSTRUCTIONS,
            tools:
                [
                    AIFunctionFactory.Create(DateTimeTools.GetTodaysDate, "get_todays_date",
                        "use to answer date related questions"),
                    AIFunctionFactory.Create((DateTime date) => appointmentService.GetEmptySlots(date), "get_empty_slots",
                        "use to get empty slots for a day")
                ],
            chatHistoryProvider: _chatHistoryProvider);
    }

    public AIAgent GetAppointmentParserAgent(AnthropicClient client)
    {
        return AgentClientFactory
            .GetClaudeAgent(
            claudeClient: client,
            model: Models.Claude.SONNET_4_6,
            name: "appointmentParserAgent",
            instructions: APPOINTMENT_PARSER_INSTRUCTIONS);
    }

    public AIAgent GetSlotSelectionAgent(AnthropicClient client)
    {
        return AgentClientFactory
            .GetClaudeAgent(
            claudeClient: client,
            model: Models.Claude.SONNET_4_6,
            name: "slotSelectionAgent",
            instructions: SLOT_SELECTION_INSTRUCTIONS,
            tools:
                [
                    AIFunctionFactory.Create(DateTimeTools.GetTodaysDate, "get_todays_date",
                        "use to answer date related questions")
                ],
            chatHistoryProvider: _chatHistoryProvider);
    }

    public AIAgent GetFinalConfirmationAgent(AnthropicClient client)
    {
        return AgentClientFactory
            .GetClaudeAgent(
            claudeClient: client,
            model: Models.Claude.SONNET_4_6,
            name: "finalConfirmationAgent",
            instructions: FINAL_CONFIRMATION_AGENT_INSTRUCTIONS,
            tools:
                [
                    AIFunctionFactory.Create(DateTimeTools.GetTodaysDate, "get_todays_date",
                        "use to answer date related questions")
                ],
            chatHistoryProvider: _chatHistoryProvider);
    }

    private static string ReadInstructions(string fileName)
    {
        var assembly = typeof(AppointmentWorkflowFactory).Assembly;
        // Resource id = <RootNamespace>.<FolderPath-with-dots>.<FileName>
        var resourceName = $"UdemyAICourseNotes.Instructions.Appointment.{fileName}";

        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException(
                $"Embedded resource '{resourceName}' not found. " +
                $"Available: {string.Join(", ", assembly.GetManifestResourceNames())}");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
