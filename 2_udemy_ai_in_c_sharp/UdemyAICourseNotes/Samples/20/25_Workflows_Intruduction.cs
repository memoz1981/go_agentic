using Microsoft.Agents.AI.Workflows;
using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using UdemyAICourseNotes.Workflows;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Samples._20;

internal class _25_Workflows_Intruduction : BaseSample
{
    public override string Description => "Workflows - introduction";
    
    public override async Task RunAsync()
    {
        Console.WriteLine();
        Console.WriteLine();

        //single client
        var client = AgentClientFactory
             .GetClient(Enums.Clients.OpenAI);

        //agents
        var appointmentRecorderAgent = AgentClientFactory.GetAgent(
            openAIClient: client,
             model: Models.OpenAI.GPT_5_4,
             name: "appointmentRecorderAgent",
             instructions: "You need to clarify with user - name, phone number, appointment timing preferences and some description",
             withMiddleware: true,
             clientType: ClientType.Chat);

        var appointmentParserAgent = AgentClientFactory.GetAgent(
            openAIClient: client,
             model: Models.OpenAI.GPT_5_4,
             name: "appointmentParserAgent",
             instructions: "You are an appointmentParserAgent",
             withMiddleware: true,
             clientType: ClientType.Chat);

        var noSlotFoundAgent = AgentClientFactory.GetAgent(
            openAIClient: client,
             model: Models.OpenAI.GPT_5_4,
             name: "feedbackAgent",
             instructions: "You are a feedback agent - provide polite feedback to user that no slots could be found for the request...",
             withMiddleware: true,
             clientType: ClientType.Chat);

        var slotSelectionAgent = AgentClientFactory.GetAgent(
            openAIClient: client,
             model: Models.OpenAI.GPT_5_4,
             name: "slotSelectionAgent",
             instructions: "There are multiple slots available - ask user which slot does it want to select - ensure " +
             "the length of the returned PossibleStartHours is exactly one... help user with the decision" +
             "by considering traffic, tea service etc.",
             withMiddleware: true,
             clientType: ClientType.Chat);

        var confirmationAgent = AgentClientFactory.GetAgent(
            openAIClient: client,
             model: Models.OpenAI.GPT_5_4,
             name: "feedbackAgent",
             instructions: "You are a feedback agent - provide polite feedback to user that appointment done...",
             withMiddleware: true,
             clientType: ClientType.Chat);

        //services
        var appointmentSchedule = new InMemoryAppointmentSchedule(); 

        //executors
        var appointmentRecorderExecutor = new UserFacingExecutor(appointmentRecorderAgent, "appointmentRecorderExecutor");
        var appointmentParserExecutor = new AppointmentParserExecutor(appointmentParserAgent);
        var appointmentCheckerExecutor = new AppointmentCheckerExecutor(appointmentSchedule);
        var noSlotFoundExecutor = new UserFacingExecutor(noSlotFoundAgent, "noSlotFoundExecutor");
        var slotSelectionExecutor = new UserFacingExecutor(slotSelectionAgent, "slotSelectionExecutor");
        var appointmentMakerExecutor = new AppointmentMakerExecutor(appointmentSchedule);
        var confirmationExecutor = new UserFacingExecutor(confirmationAgent, "confirmationExecutor"); 

        var workFlowBuilder = new WorkflowBuilder(appointmentRecorderExecutor);
        workFlowBuilder.AddEdge(appointmentRecorderExecutor, appointmentParserExecutor);
        workFlowBuilder.AddEdge(appointmentParserExecutor, appointmentCheckerExecutor);
        workFlowBuilder.AddSwitch(appointmentCheckerExecutor,
            switchBuilder =>
            {
                switchBuilder.AddCase<AppointmentDto>(app => (app.PossibleStartHours?.Length ?? 0) == 0, noSlotFoundExecutor);
                switchBuilder.AddCase<AppointmentDto>(app => app.PossibleStartHours.Length == 1, appointmentMakerExecutor);
                switchBuilder.AddCase<AppointmentDto>(app => app.PossibleStartHours.Length > 1, slotSelectionExecutor);
            });

        workFlowBuilder.AddSwitch(appointmentMakerExecutor, switchBuilder =>
        {
            switchBuilder.AddCase<bool>(val => val, confirmationExecutor);
            switchBuilder.AddCase<bool>(val => !val, noSlotFoundExecutor);
        });

        workFlowBuilder.AddEdge(slotSelectionExecutor, appointmentMakerExecutor);

        var workFlow = workFlowBuilder.Build();

        var run = await InProcessExecution.RunStreamingAsync(workflow: workFlow, 
            input: "I want to make an appointment");

        await foreach (WorkflowEvent evt in run.WatchStreamAsync())
        {
            if (evt is ExecutorCompletedEvent executorComplete)
            {
                GreenLine($"{executorComplete.ExecutorId} Completed");
            }
        }
    }

    public class Day
    {
        public Day(int startHour = 9, int endHour = 18)
        {
            if (startHour < 0 || endHour < 0 || startHour > 23 || endHour > 23)
                throw new ArgumentException("Hours should be between 0 and 23");

            //normalize end hour
            if (endHour == 0)
                endHour = 24;

            if (startHour > endHour)
                throw new ArgumentException("Start hour cannot be after the end hour"); 
            
            StartHour = startHour;
            EndHour = endHour;

            Appointments = new Appointment[endHour - startHour];
            for (int i = 0; i < endHour - startHour; i++)
                Appointments[i] = new(null, startHour + i, false, null, null); //empty
        }
        public int StartHour { get; }
        public int EndHour { get; }
        public Appointment[] Appointments { get; }

    }

    //for start assuming all are hourly
    public record Appointment(string Name, int StartHour, bool Booked, string Description, string Phone); 

    public record AppointmentDto(string Name, DateTime Date, int[] PossibleStartHours, string Description, string Phone);

    public record AddAppointmentDto(string Name, DateTime Date, int Hour, string Description, string Phone); 

    public class InMemoryAppointmentSchedule
    {
        private Dictionary<DateTime, Day> _appointments = new();
        public int StartHour { get; set; }
        public int EndHour { get; set; }

        public InMemoryAppointmentSchedule(int startHour = 9, int endHour = 18)
        {
            if (startHour < 0 || endHour < 0 || startHour > 23 || endHour > 23)
                throw new ArgumentException("Hours should be between 0 and 23");

            //normalize end hour
            if (endHour == 0)
                endHour = 24;

            if (startHour > endHour)
                throw new ArgumentException("Start hour cannot be after the end hour");

            StartHour = startHour;
            EndHour = endHour;
        }

        public int[] ReturnAvailableSlots(AppointmentDto appointmentRequest)
        {
            if (!_appointments.TryGetValue(appointmentRequest.Date, out var day))
            {
                return appointmentRequest
                    .PossibleStartHours
                    .Where(h => h >= StartHour && h < EndHour)
                    .ToArray();
            }

            return appointmentRequest
                    .PossibleStartHours
                    .Where(h => h >= day.StartHour && h < EndHour && !day.Appointments[h - day.StartHour].Booked)
                    .ToArray();
        }

        public void AddAppointment(AddAppointmentDto appointmentRequest)
        {
            if (!_appointments.TryGetValue(appointmentRequest.Date, out var day))
            {
                _appointments[appointmentRequest.Date] = new Day(StartHour, EndHour); 
            }

            var appointment = new Appointment(appointmentRequest.Name, appointmentRequest.Hour, true, 
                appointmentRequest.Description, appointmentRequest.Phone);

            var index = appointmentRequest.Hour - day.StartHour;
            day.Appointments[index] = appointment;
        }
    }
}
