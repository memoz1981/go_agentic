using Microsoft.Agents.AI;
using OpenAI;
using UdemyAICourseNotes.Services.Workflows;
using UdemyAICourseNotes.Workflows.OptimizedAppointment;
using static UdemyAICourseNotes.Helpers.Output;
using static UdemyAICourseNotes.Samples._20._25_Workflows_Intruduction;

namespace UdemyAICourseNotes.Samples._20;

internal class _28_Workflows_Optimized : BaseSample
{
    public override string Description => "Optimized appointment workflow";
    private readonly OpenAIClient _client; 
    private readonly AIAgent _appointmentRecorderAgent;
    private readonly AIAgent _appointmentParserAgent;
    private readonly AIAgent _slotFoundAgent;
    private readonly AIAgent _noSlotFoundAgent;
    private readonly AIAgent _slotSelectionAgent;
    private readonly AppointmentWorkflowFactory _agentFactory;

    private readonly InMemoryAppointmentSchedule _inMemoryAppointmentSchedule;

    private readonly InitialAppointmentExecutor _initialAppointmentExecutor;
    private readonly AppParserExecutor _appointmentParserExecutor; 

    public _28_Workflows_Optimized()
    {
        _agentFactory = new();
        _client = _agentFactory.GetClient();
        _inMemoryAppointmentSchedule = new(); 

        _appointmentRecorderAgent = _agentFactory.GetRecorderAgent(_client);
        _appointmentParserAgent = _agentFactory.GetAppointmentParserAgent(_client);
        _slotFoundAgent = _agentFactory.GetConfirmationAgent(_client);
        _noSlotFoundAgent = _agentFactory.GetNoSlotFoundAgent(_client);
        _slotSelectionAgent = _agentFactory.GetSlotSelectionAgent(_client);

        _initialAppointmentExecutor = new(_appointmentRecorderAgent);
        _appointmentParserExecutor = new(_appointmentParserAgent); 
    }

    public override Task RunAsync()
    {
        Gray($"Running the sample for {Description}");
        Console.WriteLine();
        Console.WriteLine();



    }
}
