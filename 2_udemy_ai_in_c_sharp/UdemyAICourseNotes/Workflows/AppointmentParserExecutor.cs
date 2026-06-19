using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using static UdemyAICourseNotes.Helpers.Output; 
using static UdemyAICourseNotes.Samples._20._25_Workflows_Intruduction;

namespace UdemyAICourseNotes.Workflows;

internal class AppointmentParserExecutor(AIAgent agent) : Executor<string, List<AppointmentDto>>("AppointmentParser")
{
    private const string INSTRUCTIONS = @"You are an appointment agent who helps to take appointments from the users. 
All appointents are hourly - for a day - multiple hours can be marked just populating start hours, 9 for 9 am, 
18 for 18am, 0 for midnight etc. Parse the user data - provide one AppointmentDto object for each day, use DateOnly for dates
if the description contains PhoneNumber - parse it to PhoneNumber property and remove from Description, if not
put all info into description. If there's a name - parse it to name property. input data is start only most of the time. "; 
    
    public override async ValueTask<List<AppointmentDto>> HandleAsync(
        string message, 
        IWorkflowContext context, 
        CancellationToken cancellationToken = default)
    {
        GrayLine("Appointment parser parsing message...");
        var response = await agent.RunAsync<List<AppointmentDto>>($"{INSTRUCTIONS}: {message}");

        GrayLine($"Appointment parser returned {response.Result?.Count ?? 0} of possible options");

        return response.Result;
    }
}
