using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.Text.Json;
using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using UdemyAICourseNotes.Tools;
using static UdemyAICourseNotes.Helpers.Output;
using static UdemyAICourseNotes.Samples._20._25_Workflows_Intruduction;

namespace UdemyAICourseNotes.Samples._20; 

internal class _26_Workflow_Without_Workflows : BaseSample
{
    public override string Description => "Workflow without workflows (pure agents)";

    public override async Task RunAsync()
    {
        Console.WriteLine();
        Console.WriteLine();

        //single client
        var client = AgentClientFactory
             .GetClient(Enums.Clients.OpenAI);

        //agents
        // agent to take the initial request
        var appointmentRecorderAgentInstruction =
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

        //public record InitialAppointmentDto(bool IsFinal, bool IsCancelled, string FinalizedRequest, string FurtherQuestionToUser); 
        var appointmentRecorderAgent = AgentClientFactory.GetAgent(
            openAIClient: client,
             model: Models.OpenAI.GPT_5_4,
             name: "appointmentRecorderAgent",
             instructions: appointmentRecorderAgentInstruction,
             tools: [AIFunctionFactory.Create(DateTimeTools.GetTodaysDate, "get_todays_date", "use to answer date related questions")],
             withMiddleware: true,
             clientType: ClientType.Chat);

        //this agent will parse the request to structured output
        var appointmentParserAgent = AgentClientFactory.GetAgent(
            openAIClient: client,
             model: Models.OpenAI.GPT_5_4,
             name: "appointmentParserAgent",
             instructions: "You are an appointmentParserAgent - parse the request to structured output provided",
             withMiddleware: true,
             clientType: ClientType.Chat);
        
        //this agent will just feedback to the user that no slot has been found...
        var noSlotFoundAgent = AgentClientFactory.GetAgent(
            openAIClient: client,
             model: Models.OpenAI.GPT_5_4,
             name: "noSlotFoundAgent",
             instructions: "You are a feedback agent - say sorry and provide feedback that no slots could be found for the request - summarize the request",
             withMiddleware: true,
             clientType: ClientType.Chat);

        var slotFoundAgent = AgentClientFactory.GetAgent(
            openAIClient: client,
             model: Models.OpenAI.GPT_5_4,
             name: "slotFoundAgent",
             instructions: "You are a feedback agent - provide polite feedback to user that appointment done... Summarize the appointment details provided",
             withMiddleware: true,
             clientType: ClientType.Chat);

        var slotSelectionAgentInstructions =
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

        var slotSelectionAgent = AgentClientFactory.GetAgent(
            openAIClient: client,
             model: Models.OpenAI.GPT_5_4,
             name: "slotSelectionAgent",
             instructions: slotSelectionAgentInstructions,
             withMiddleware: true,
             clientType: ClientType.Chat);


        //services
        var appointmentSchedule = new InMemoryAppointmentSchedule();

        //workflow using the agents

        //take user input
        var userInput = await TakeAppointment(appointmentRecorderAgent);

        if (userInput.IsCancelled)
            return;

        if (!userInput.IsFinal)
            throw new InvalidOperationException("Cannot be");

        var request = userInput.FinalizedRequest;

        //convert user input to structured output
        var initialAppointmentDto = await appointmentParserAgent.RunAsync<AppointmentDto>(request);

        //check the calendar
        var hours = appointmentSchedule.ReturnAvailableSlots(initialAppointmentDto.Result);

        var filteredAppointment = initialAppointmentDto.Result with { PossibleStartHours = hours };
        //notify the user if no slots found...
        if (hours is null or [])
        {
            await ProvideFeedback(noSlotFoundAgent, slotFoundAgent, filteredAppointment, false);
            return; 
        }

        //if single hour - just make the appointment
        if (filteredAppointment.PossibleStartHours.Length == 1)
        {
            var addAppointmentResult = TryMakeAppointment(filteredAppointment, appointmentSchedule);

            await ProvideFeedback(noSlotFoundAgent, slotFoundAgent, addAppointmentResult.Appointment, addAppointmentResult.Success);
        }

        //if multiple hours - work with user to select other timeslot
        var slotSelectionResult = await SelectSlots(slotSelectionAgent, filteredAppointment);
        var appointmentResult = filteredAppointment with { PossibleStartHours = [slotSelectionResult.Appointment.Hour] };

        if (slotSelectionResult.IsCancelled)
        {
            await ProvideFeedback(noSlotFoundAgent, slotFoundAgent, appointmentResult, false);
            return;
        }

        if (slotSelectionResult.IsFinal)
        {
            await ProvideFeedback(noSlotFoundAgent, slotFoundAgent, appointmentResult, true);
            return;
        }

        throw new ArgumentException("should have been selected...");
    }

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

    private static async Task ProvideFeedback(AIAgent noSlotAgent, AIAgent slotFoundAgent, AppointmentDto appointment, bool success)
    {
        var appointmentString = JsonSerializer.Serialize(appointment);
        if (success)
        {
            var successText = await slotFoundAgent.RunAsync<string>(appointmentString);

            GreenLine($"> {successText.Result}");
            return; 
        }
        
        var failureText = await noSlotAgent.RunAsync<string>(appointmentString);
        RedLine($"> {failureText.Result}");
    }

    private static async Task<SlotSelectionDto> SelectSlots(AIAgent slotSelectionAgent, AppointmentDto appointment)
    {
        var session = await slotSelectionAgent.CreateSessionAsync();
        var result = (await slotSelectionAgent.RunAsync<SlotSelectionDto>(JsonSerializer.Serialize(appointment), session)).Result;

        if (result.IsCancelled || result.IsFinal)
        {
            return result;
        }

        GreenLine($"> {result.FurtherQuestionToUser}");
        var userInput = Console.ReadLine();

        while (true)
        {
            result = (await slotSelectionAgent.RunAsync<SlotSelectionDto>(userInput, session)).Result;

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
