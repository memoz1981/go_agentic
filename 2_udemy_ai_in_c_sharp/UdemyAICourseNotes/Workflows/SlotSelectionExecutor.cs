using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using System.Text.Json;
using static UdemyAICourseNotes.Helpers.Output;
using static UdemyAICourseNotes.Samples._20._25_Workflows_Intruduction;

namespace UdemyAICourseNotes.Workflows;

internal class SlotSelectionExecutor(AIAgent agent) : Executor<AppointmentDto, AppointmentDto>("SlotSelectionExecutor")
{
    public override async ValueTask<AppointmentDto> HandleAsync(AppointmentDto message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        GrayLine("SlotSelectionExecutor agent executing...");

        var session = await agent.CreateSessionAsync(); 
        while (true)
        {
            var response = await agent.RunAsync("Work with the user to select only one of PossibleStartHours of the AppointmentDto as " +
                $"following: {JsonSerializer.Serialize(message)}", session);

            BlueLine($">{response.ToString()}");
            Blue(">");
            var input = Console.ReadLine();

            var oneSlotSelectedResponse = await agent.RunAsync<bool>($"Confirm based on the provided input if one time slot is selected " +
                $"and if it falls into one of the message hours provided: {input}", session);

            if (oneSlotSelectedResponse.Result)
            {
                var selectionResponse = await agent.RunAsync<AppointmentDto>($"Convert the provided input to AppointmentDto " +
                    $"with single hour slot {input}", session);

                if (selectionResponse.Result.PossibleStartHours.Length != 1)
                    throw new InvalidOperationException("Some mismatch..."); 

                GrayLine("SlotSelectionExecutor agent executed...");
                return message with { PossibleStartHours = selectionResponse.Result.PossibleStartHours };
            }
        }
    }
}
