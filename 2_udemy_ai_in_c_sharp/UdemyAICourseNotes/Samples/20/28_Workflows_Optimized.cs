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
    private readonly AIAgent _slotSelectionAgent;
    private readonly AIAgent _finalConfirmationAgent; 
    private readonly AppointmentWorkflowFactory _agentFactory;

    private readonly AppointmentService _appointmentService;
    private readonly Services.Memory.InMemoryChatHistoryProvider _chatHistoryProvider;

    private readonly InitialAppointmentExecutor _initialAppointmentExecutor;
    private readonly InitialCancellationExecutor _cancellationExecutor; 
    private readonly AppParserExecutor _appointmentParserExecutor;
    private readonly CalendarCheckerExecutor _calendarCheckerExecutor;
    private readonly SlotSelectionExecutor _slotSelectionExecutor;
    private readonly SlotSelectionCancellationExecutor _slotSelectionCancellationExecutor;
    private readonly SlotSelectionToTextConversionExecutor _slotSelectionToTextExecutor;
    private readonly AppointmentMakerExecutor _appointmentMakerExecutor;
    private readonly FinalConfirmationExecutor _finalConfirmationExecutor;
    private readonly FinalAppointmentToTextExecutor _finalAppointmentToTextExecutor;
    private readonly EndOfAppointmentExecutor _endOfAppointmentExecutor; 

    public const string INITIAL_INPUT = "START"; 

    public _28_Workflows_Optimized()
    {
        _chatHistoryProvider = new(); 
        _agentFactory = new(_chatHistoryProvider);
        _client = _agentFactory.GetClient();
        _appointmentService = new(); 

        _appointmentRecorderAgent = _agentFactory.GetRecorderAgent(_client, _appointmentService);
        _appointmentParserAgent = _agentFactory.GetAppointmentParserAgent(_client);
        _slotSelectionAgent = _agentFactory.GetSlotSelectionAgent(_client);
        _finalConfirmationAgent = _agentFactory.GetFinalConfirmationAgent(_client);

        _initialAppointmentExecutor = new(_appointmentRecorderAgent);
        _appointmentParserExecutor = new(_appointmentParserAgent);
        _cancellationExecutor = new();
        _calendarCheckerExecutor = new(_appointmentService);
        _slotSelectionExecutor = new(_slotSelectionAgent);
        _slotSelectionCancellationExecutor = new();
        _slotSelectionToTextExecutor = new();
        _appointmentMakerExecutor = new(_appointmentService);
        _finalConfirmationExecutor = new(_finalConfirmationAgent);
        _finalAppointmentToTextExecutor = new();
        _endOfAppointmentExecutor = new(); 
    }

    public override async Task RunAsync()
    {
        Console.Clear(); 
        Gray($"Running the sample for {Description}");
        Console.WriteLine();
        Console.WriteLine();

        while (true)
        {
            var workFlow = ReturnWorkflow();
            Blue("> ");

            var input = Console.ReadLine() ?? string.Empty;

            if (string.Equals(input, EXIT, StringComparison.InvariantCultureIgnoreCase))
                break;

            await InProcessExecution.RunAsync(workFlow, INITIAL_INPUT);

            _appointmentService.PrintCalendar();

            Separator();
        }
    }

    private Workflow ReturnWorkflow()
    {
        //start of the workflow - talk with the customer
        var workFlowBuilder = new WorkflowBuilder(_initialAppointmentExecutor);

        //if the request is finalized - pass to parser, otherwise pass to cancellation
        workFlowBuilder.AddSwitch(_initialAppointmentExecutor, switchBuilder =>
        {
            switchBuilder.AddCase<InitialAppointmentSlim>(
                app => app.InitialAppointmentStatus == InitialAppointmentStatus.RequestCancelled, _cancellationExecutor);
            switchBuilder.AddCase<InitialAppointmentSlim>(
                app => app.InitialAppointmentStatus == InitialAppointmentStatus.CouldNotFinalize, _cancellationExecutor);
            switchBuilder.AddCase<InitialAppointmentSlim>(
                app => app.InitialAppointmentStatus == InitialAppointmentStatus.RequestFinalized, _appointmentParserExecutor);

        });

        //feed the result of the parser to calendar checker
        workFlowBuilder.AddEdge(_appointmentParserExecutor, _calendarCheckerExecutor);

        //return the free slots to user
        workFlowBuilder.AddEdge(_calendarCheckerExecutor, _slotSelectionExecutor);

        //based on the slot selection result, either cancel or return to start or proceed to making the appointment 
        //because of the type mismatch - to go back to start we first need to convert slot selection result to string
        //basically taking one property of it... 
        workFlowBuilder.AddSwitch(_slotSelectionExecutor, switchBuilder =>
        {
            switchBuilder.AddCase<SlotSelectionResult>(
                res => res.SlotSelectionStatus == SlotSelectionStatus.Cancelled, _slotSelectionCancellationExecutor);
            switchBuilder.AddCase<SlotSelectionResult>(
                res => res.SlotSelectionStatus == SlotSelectionStatus.AlternativeDateProposed, _slotSelectionToTextExecutor);
            switchBuilder.AddCase<SlotSelectionResult>(
                res => res.SlotSelectionStatus == SlotSelectionStatus.SlotSelected, _appointmentMakerExecutor);
        });

        //if user wants to go back - route to initial appointment
        workFlowBuilder.AddEdge(_slotSelectionToTextExecutor, _initialAppointmentExecutor);

        //feed the result of the booking to final confirmation - to work with user to decide way forward
        workFlowBuilder.AddEdge(_appointmentMakerExecutor, _finalConfirmationExecutor);

        workFlowBuilder.AddSwitch(_finalConfirmationExecutor, switchBuilder => 
        {
            switchBuilder.AddCase<FinalAppointment>(app => app.Status == FinalAppointmentStatus.CancelledWithFollowUp,
                _finalAppointmentToTextExecutor);
            switchBuilder.AddCase<FinalAppointment>(app => app.Status == FinalAppointmentStatus.CompletedWithFollowUp,
                _finalAppointmentToTextExecutor);
            switchBuilder.AddCase<FinalAppointment>(app => app.Status == FinalAppointmentStatus.CancelledStopHere,
                _endOfAppointmentExecutor);
            switchBuilder.AddCase<FinalAppointment>(app => app.Status == FinalAppointmentStatus.CompletedStopHere,
                _endOfAppointmentExecutor);
        });

        workFlowBuilder.AddEdge(_finalAppointmentToTextExecutor, _initialAppointmentExecutor); 

        return workFlowBuilder.Build(); 
    }
}
