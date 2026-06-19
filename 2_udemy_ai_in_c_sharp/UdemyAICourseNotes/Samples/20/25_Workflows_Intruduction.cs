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
             name: "appointmentParserAgent",
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


        //services

        //executors
        var appointmentRecorderExecutor = new UserFacingExecutor(appointmentRecorderAgent);
        var appointmentParserExecutor = new AppointmentParserExecutor(appointmentParserAgent); 

        var workFlowBuilder = new WorkflowBuilder(appointmentRecorderExecutor);
        workFlowBuilder.AddEdge(appointmentRecorderExecutor, appointmentParserExecutor); 
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

            Appointments = new List<Appointment>(endHour - startHour);
            for (int i = 0; i < endHour - startHour; i++)
                Appointments[i] = new(null, startHour + i, false, null, null); //empty
        }
        public int StartHour { get; set; }
        public int EndHour { get; set; }
        public List<Appointment> Appointments { get; set; }

    }

    //for start assuming all are hourly
    public record Appointment(string Name, int StartHour, bool Booked, string Description, string Phone); 

    public record AppointmentDto(string Name, DateTime Date, int[] PossibleStartHours, string Description, string Phone); 
}
