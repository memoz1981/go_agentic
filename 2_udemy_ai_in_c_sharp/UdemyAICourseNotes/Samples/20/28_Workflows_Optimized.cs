using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using OpenAI;
using UdemyAICourseNotes.Model.Appointment;
using UdemyAICourseNotes.Services.Appointment;
using UdemyAICourseNotes.Services.Workflows;
using UdemyAICourseNotes.Workflows.OptimizedAppointment;
using static UdemyAICourseNotes.Helpers.Output;

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

    private readonly AppointmentService _appointmentService; 

    private readonly InitialAppointmentExecutor _initialAppointmentExecutor;
    private readonly InitialCancellationExecutor _cancellationExecutor; 
    private readonly AppParserExecutor _appointmentParserExecutor;
    private readonly FreeSlotFinderExecutor _freeSlotFinderExecutor; 

    public _28_Workflows_Optimized()
    {
        _agentFactory = new();
        _client = _agentFactory.GetClient();
        _appointmentService = new(); 

        _appointmentRecorderAgent = _agentFactory.GetRecorderAgent(_client);
        _appointmentParserAgent = _agentFactory.GetAppointmentParserAgent(_client);
        _slotFoundAgent = _agentFactory.GetConfirmationAgent(_client);
        _noSlotFoundAgent = _agentFactory.GetNoSlotFoundAgent(_client);
        _slotSelectionAgent = _agentFactory.GetSlotSelectionAgent(_client);

        _initialAppointmentExecutor = new(_appointmentRecorderAgent);
        _appointmentParserExecutor = new(_appointmentParserAgent);
        _cancellationExecutor = new();
        _freeSlotFinderExecutor = new(_appointmentService); 
    }

    public override Task RunAsync()
    {
        Gray($"Running the sample for {Description}");
        Console.WriteLine();
        Console.WriteLine();

        var workFlowBuilder = new WorkflowBuilder(_initialAppointmentExecutor);
        workFlowBuilder.AddSwitch(_initialAppointmentExecutor, switchBuilder =>
        {
            switchBuilder.AddCase<InitialAppointmentSlim>(
                app => app.InitialAppointmentStatus == InitialAppointmentStatus.RequestCancelled, _cancellationExecutor);
            switchBuilder.AddCase<InitialAppointmentSlim>(
                app => app.InitialAppointmentStatus == InitialAppointmentStatus.CouldNotFinalize, _cancellationExecutor);
            switchBuilder.AddCase<InitialAppointmentSlim>(
                app => app.InitialAppointmentStatus == InitialAppointmentStatus.RequestFinalized, _freeSlotFinderExecutor);
            
            throw new InvalidOperationException("Invalid value for initial apppointment status...");
        });

    }
}
