using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using UdemyAICourseNotes.Tools;

namespace UdemyAICourseNotes.Services.Workflows;

internal class AppointmentWorkflowFactory
{
    public AppointmentWorkflowFactory()
    {
        _chatHistoryProvider = new(); 
    }

    //agents
    // agent to take the initial request
    private const string APPOINTMENT_RECORDER_INSTRUCTIONS =
        @"
            - Welcome the user and ask how can you help 
            - you are only appointment agent and cannot answer any other questions other than appointment scheduling and date related questions. 
            - Work with the customer to gather following appoinment information:
            a) Name 
            b) Phone number
            c) Date and Time of the appointment that works for the customer - accept responses like today after noon or tomorrow morning. 
            d) Description - what is the appointment for
            - All data should be provided, keep your questions short, precise, pretty
            - In the end - summarize the request and ask for confirmation
            - Use the tool 'get_todays_date' to get todays date and calculate tomorrow and any other date related questions
            - Return data as follows:
            a) InitialAppointmentStatus
            i) Return RequestFinalized if request is finalized/confirmed with the customer
            ii) Return RequestCancelled if customer cancelled after confirmation
            iii) Return CouldNotFinalize - if the user failed to confirm/cancel after 10 questions - don't ask any confirmation just return then
            iv) Return ClarificationsRequired - if still questions need to be answered by client or confirmation required - FurtherQuestionToUser should be provided
            b) IsCancelled - return if the customer changed his/her mind and cancelled the request 
            c) FinalizedRequest - return only if IsFinal is true - summarizing the request details - otherwise return null
            d) FurtherQuestionToUser - return only if there are further questions to user (use polite short tone) and InitialAppointmentStatus is ClarificationsRequired";

    private const string SLOT_SELECTION_INSTRUCTIONS =
        @"You are a appointment slot selection agent - work with user to select the suitable time slot - one of the 
        hours from PossibleStartHours - all hours are start hours - don't allow the user to select any other hour. 
        Don't allow the user to change any other details like name, phone, date description.
        Confirm the final hour selected with user.
        Return data as follows:
            IsFinal - return true if the appointment is finalized and confirmed with the user with single timeslot
            IsCancelled - return if the user changed his/her mind and cancelled the request 
            Return IsCancelled true if the user could not answer 10 questions to finalize the request - when returning this 
            return FurtherQuestionsToUser as polite apology saying that you need to try later
            Appointment - return only if IsFinal is true - otherwise return null
            FurtherQuestionToUser - return only if there are further questions to user (use polite short tone) or as above
            the IsCancelled is true - if it's cancelled by the user just tell politely we would love to see you soon";

    private readonly Memory.InMemoryChatHistoryProvider _chatHistoryProvider = new();

    public OpenAIClient GetClient()
        => AgentClientFactory.GetClient(Enums.Clients.OpenAI);

    public AIAgent GetRecorderAgent(OpenAIClient client)
    {
        var chatClientAgentOptions = new ChatClientAgentOptions()
        {
            Name = "appointmentRecorderAgent",
            ChatOptions = new()
            {
                Instructions = APPOINTMENT_RECORDER_INSTRUCTIONS,
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

    public AIAgent GetAppointmentParserAgent(OpenAIClient client)
    {
        //this agent will parse the request to structured output
        return AgentClientFactory.GetAgent(
            openAIClient: client,
             model: Models.OpenAI.GPT_5_4_NANO,
             name: "appointmentParserAgent",
             instructions: "You are an appointmentParserAgent - parse the request to structured output provided",
             withMiddleware: true,
             clientType: ClientType.Chat);
    }

    public AIAgent GetNoSlotFoundAgent(OpenAIClient client)
    {
        var instructions = "You are a feedback agent - provide polite feedback to user that no slots could be found for the request. " +
            "In your feedback - provide the day and time/time range provided... Ask if user wants to schedule for another day";

        var chatClientAgentOptions = new ChatClientAgentOptions()
        {
            Name = "noSlotFoundAgent",
            ChatOptions = new()
            {
                Instructions = instructions,
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

    public AIAgent GetConfirmationAgent(OpenAIClient client)
    {
        var instructions = "You are a feedback agent - provide polite feedback to user that appointment done..." +
            "In your feedback - provide the day and time/time range provided...";

        var chatClientAgentOptions = new ChatClientAgentOptions()
        {
            Name = "confirmationAgent",
            ChatOptions = new()
            {
                Instructions = instructions,
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
}
