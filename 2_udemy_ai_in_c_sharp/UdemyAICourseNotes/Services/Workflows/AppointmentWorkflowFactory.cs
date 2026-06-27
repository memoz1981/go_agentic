using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using UdemyAICourseNotes.Services.Appointment;
using UdemyAICourseNotes.Tools;

namespace UdemyAICourseNotes.Services.Workflows;

internal class AppointmentWorkflowFactory
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

    public AppointmentWorkflowFactory(Memory.InMemoryChatHistoryProvider chatHistoryProvider)
    {
        _chatHistoryProvider = chatHistoryProvider;
    }

    public OpenAIClient GetClient()
        => AgentClientFactory.GetClient(Enums.Clients.OpenAI);

    public AIAgent GetRecorderAgent(OpenAIClient client, AppointmentService appointmentService)
    {
        var chatClientAgentOptions = new ChatClientAgentOptions()
        {
            Name = "appointmentRecorderAgent",
            ChatOptions = new()
            {
                Instructions = APPOINTMENT_RECORDER_INSTRUCTIONS,
                Tools = 
                [
                    AIFunctionFactory.Create(DateTimeTools.GetTodaysDate, "get_todays_date", 
                        "use to answer date related questions"),
                    AIFunctionFactory.Create((DateTime date) => appointmentService.GetEmptySlots(date), "get_empty_slots",
                        "use to get empty slots for a day")
                ],
            },
            ChatHistoryProvider = _chatHistoryProvider,
        };

        return AgentClientFactory
            .GetAgent(
            openAIClient: client,
            model: Models.OpenAI.GPT_5_4,
            chatClientAgentOptions: chatClientAgentOptions
            );
    }

    public AIAgent GetAppointmentParserAgent(OpenAIClient client)
    {
        //this agent will parse the request to structured output
        return AgentClientFactory.GetAgent(
            openAIClient: client,
             model: Models.OpenAI.GPT_5_4_NANO,
             name: "appointmentParserAgent",
             instructions: APPOINTMENT_PARSER_INSTRUCTIONS,
             withMiddleware: true,
             clientType: ClientType.Chat);
    }

    public AIAgent GetSlotSelectionAgent(OpenAIClient client)
    {
        var chatClientAgentOptions = new ChatClientAgentOptions()
        {
            Name = "slotSelectionAgent",
            ChatOptions = new()
            {
                Instructions = SLOT_SELECTION_INSTRUCTIONS,
                Tools = [AIFunctionFactory.Create(DateTimeTools.GetTodaysDate,
                    "get_todays_date", "use to answer date related questions")],
            },
            ChatHistoryProvider = _chatHistoryProvider,
        };

        return AgentClientFactory
            .GetAgent(
            openAIClient: client,
            model: Models.OpenAI.GPT_5_4_MINI,
            chatClientAgentOptions: chatClientAgentOptions
            );
    }

    public AIAgent GetFinalConfirmationAgent(OpenAIClient client)
    {
        var chatClientAgentOptions = new ChatClientAgentOptions()
        {
            Name = "finalConfirmationAgent",
            ChatOptions = new()
            {
                Instructions = FINAL_CONFIRMATION_AGENT_INSTRUCTIONS,
                Tools = [AIFunctionFactory.Create(DateTimeTools.GetTodaysDate,
                    "get_todays_date", "use to answer date related questions")],
            },
            ChatHistoryProvider = _chatHistoryProvider,
        };

        return AgentClientFactory
            .GetAgent(
            openAIClient: client,
            model: Models.OpenAI.GPT_5_4_MINI,
            chatClientAgentOptions: chatClientAgentOptions
            );
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
