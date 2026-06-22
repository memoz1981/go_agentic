using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using System.Text.Json;
using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using UdemyAICourseNotes.Tools;
using static UdemyAICourseNotes.Helpers.Output;
using static UdemyAICourseNotes.Samples._20._25_Workflows_Intruduction;

namespace UdemyAICourseNotes.Samples._20; 

internal class _27_Agent_As_Workflow_Orchestrator : BaseSample
{
    public override string Description => "Agent as Workflow Orchestrator";
    // agent to take the initial request
    private const string APPOINTMENT_RECORDER_INSTRUCTIONS =
            @"
            Welcome the user and ask how can you help - you are only appointment agent and cannot answer any other questions
            other than appointment scheduling and date related questions. 
            Work with the user to gather following appoinment information:
            Name: 
            Phone number
            Date of appointment (single date) - user may change the date
            Possible hours (like 10 to 18 or after noon, evening etc.) 
            Description - what is the appointment for
            All data should be provided, keep your questions short, precise, pretty - 
            In the end - summarize and ask for confirmation
            Use the tool 'get_todays_date' to get todays date and calculate tomorrow and any other date related questions
            Return null if the request is cancelled, true if the request is fully taken/confirmed, false if still clarifications required
            Return data as follows:
            IsFinal - return true if the appointment is finalized and confirmed with the user
            IsCancelled - return if the user changed his/her mind and cancelled the request 
            Return IsCancelled true if the user could not answer 10 questions to finalize the request - when returning this 
            return FurtherQuestionsToUser as polite apology saying that you need to try later
            FinalizedRequest - return only if IsFinal is true - summarizing the request details - otherwise return null
            FurtherQuestionToUser - return only if there are further questions to user (use polite short tone) or as above
            the IsCancelled is true - if it's cancelled by the user just tell politely we would love to see you soon";

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

    private const string MAIN_AGENT_INSTRUCTIONS =
        @"Use 'get_todays_date' for any date related questions 
        start with 'record_appointment' to record an appointment - following this pass the string data to 
        '_appointmentParserAgent' to parse data to structured output. 
        Following that check the schedule for any available slots calling 'return_slots'
        If no slots call '_noSlotFoundAgent' to confirm to user that no slots found - the flow should end here then
        If single slot - call 'make_appointment' - if the response is true call '_slotFoundAgent' to provide confirmation to user and stop there,
        but if not successful - call '_noSlotFoundAgent' similar to above and end 
        If multiple slots found - call 'select_slots' and then return based on the response 
        a) IsFinal - call 'make_appointment' same as above sequence
        b) IsCancelled - cancel and call '_noSlotFoundAgent'";


    private readonly OpenAIClient _client; 
    private readonly AIAgent _appointmentRecorderAgent;
    private readonly AIAgent _appointmentParserAgent;
    private readonly AIAgent _noSlotFoundAgent;
    private readonly AIAgent _slotFoundAgent;
    private readonly AIAgent _slotSelectionAgent;
    private readonly AIAgent _mainAgent;
    private readonly InMemoryAppointmentSchedule _schedule; 

    public _27_Agent_As_Workflow_Orchestrator()
    {
        _client = AgentClientFactory.GetClient(Enums.Clients.OpenAI);
        _appointmentRecorderAgent = GetAgent(_client, "appointmentRecorderAgent", APPOINTMENT_RECORDER_INSTRUCTIONS, 
            [AIFunctionFactory.Create(DateTimeTools.GetTodaysDate, "get_todays_date", "use to answer date related questions")]);
        _appointmentParserAgent = GetAgent(_client, "appointmentParserAgent",
            "You are an appointmentParserAgent - parse the request to structured output provided", []);
        _noSlotFoundAgent = GetAgent(_client, "noSlotFoundAgent",
            "You are a feedback agent - say sorry and provide feedback that no slots could be found for the request - summarize the request", []);
        _slotFoundAgent = GetAgent(_client, "slotFoundAgent",
            "You are a feedback agent - provide polite feedback to user that appointment done... Summarize the appointment details provided", []);
        _slotSelectionAgent = GetAgent(_client, "slotSelectionAgent", SLOT_SELECTION_INSTRUCTIONS, []);
        _schedule = new();
        _mainAgent = GetAgent(_client, "mainAgent", MAIN_AGENT_INSTRUCTIONS, 
            [
                AIFunctionFactory.Create(DateTimeTools.GetTodaysDate, "get_todays_date", "use to answer date related questions"),
                AIFunctionFactory.Create(() => TakeAppointment(_appointmentRecorderAgent), "record_appointment", "use this to get the initial appointment"),
                _appointmentParserAgent.AsAIFunction(),
                _noSlotFoundAgent.AsAIFunction(),
                _slotFoundAgent.AsAIFunction(),
                AIFunctionFactory.Create(_schedule.ReturnAvailableSlots, "return_slots", "use this to filter available hour slots from calendar"),
                AIFunctionFactory.Create(TryMakeAppointment, "make_appointment", "use this function to make a new appointment"),
                AIFunctionFactory.Create(SelectSlots, "select_slots", "use this function to help user to select single slot only")
            ]); 
    }

    private static void Output(string text) => GreenLine(text); 

    public override async Task RunAsync()
    {
        Console.WriteLine();
        Console.WriteLine();

        var session = await _mainAgent.CreateSessionAsync(); 
        var initialResponse = await _mainAgent.RunAsync("Help user to make an appointment", session);
        GreenLine($"> {initialResponse.ToString()}");
        Console.Write("> ");
        var input = Console.ReadLine(); 

        while (true)
        {
            var check = await _mainAgent.RunAsync<(bool isComplete, string message)>(input, session);

            if (check.Result.isComplete)
            {
                Console.WriteLine(check.Result.message);
            }

            var questions = await _mainAgent.RunAsync<string>("Clarify any pending questions with the user if any", session);

            GreenLine($"> {questions}");
            
            Console.Write("> ");
            input = Console.ReadLine(); 
        }
    }

    private static AIAgent GetAgent(OpenAIClient client, string name, string instructions, IList<AITool> tools)
        => AgentClientFactory.GetAgent(
            openAIClient: client,
             model: Models.OpenAI.GPT_5_4,
             name: name,
             instructions: instructions,
             tools: tools,
             withMiddleware: true,
             clientType: ClientType.Chat);

    private static async Task<InitialAppointmentDto> TakeAppointment(AIAgent agent)
    {
        var session = await agent.CreateSessionAsync();
        var result = (await agent.RunAsync<InitialAppointmentDto>("Welcome and ask if user wants to book an appointment",
            session)).Result;

        if (result.IsCancelled)
        {
            RedLine($"> {result.FurtherQuestionToUser}");
            return result;
        }

        GreenLine($"> {result.FurtherQuestionToUser}");
        var userInput = Console.ReadLine();

        while (true)
        {
            result = (await agent.RunAsync<InitialAppointmentDto>(userInput, session)).Result;

            if (result.IsFinal || result.IsCancelled)
            {
                return result;
            }

            GreenLine($"> {result.FurtherQuestionToUser}");
            Console.Write("> ");
            userInput = Console.ReadLine();
        }
    }

    private static AppointmentResultDto TryMakeAppointment(AppointmentDto appointment, InMemoryAppointmentSchedule schedule)
    {
        HashSet<string> blackList = ["dave1", "dave2"];

        if (appointment is null
            || appointment.PossibleStartHours is null or []
            || appointment.PossibleStartHours.Length != 1
            || string.IsNullOrWhiteSpace(appointment.Name)
            || string.IsNullOrWhiteSpace(appointment.Phone)
            || string.IsNullOrWhiteSpace(appointment.Description))
        {
            return new AppointmentResultDto(false, appointment);
        }

        if (blackList.Contains(appointment.Name.ToLower().Trim()))
        {
            return new AppointmentResultDto(false, appointment);
        }

        var addAppointmentDto = new AddAppointmentDto(appointment.Name, appointment.Date, appointment.PossibleStartHours[0],
            appointment.Description, appointment.Phone);

        schedule.AddAppointment(addAppointmentDto);

        return new AppointmentResultDto(true, appointment);
    }

    private async Task ProvideFeedback(AppointmentDto appointment, bool success)
    {
        var appointmentString = JsonSerializer.Serialize(appointment);
        if (success)
        {
            var successText = await _slotFoundAgent.RunAsync<string>(appointmentString);

            GreenLine($"> {successText.Result}");
            return;
        }

        var failureText = await _noSlotFoundAgent.RunAsync<string>(appointmentString);
        RedLine($"> {failureText.Result}");
    }

    private async Task<SlotSelectionDto> SelectSlots(AppointmentDto appointment)
    {
        var session = await _slotSelectionAgent.CreateSessionAsync();
        var result = (await _slotSelectionAgent.RunAsync<SlotSelectionDto>(JsonSerializer.Serialize(appointment), session)).Result;

        if (result.IsCancelled || result.IsFinal)
        {
            return result;
        }

        GreenLine($"> {result.FurtherQuestionToUser}");
        var userInput = Console.ReadLine();

        while (true)
        {
            result = (await _slotSelectionAgent.RunAsync<SlotSelectionDto>(userInput, session)).Result;

            if (result.IsFinal || result.IsCancelled)
            {
                return result;
            }

            GreenLine($"> {result.FurtherQuestionToUser}");
            Console.Write("> ");
            userInput = Console.ReadLine();
        }
    }
}
