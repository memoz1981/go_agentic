using Microsoft.Agents.AI;
using System.Text;

namespace HelloAgent.Extensions; 

public static class AgentExtensions
{
    public static void PrintAgent(this ChatClientAgent agent)
    {
        var builder = new StringBuilder();

        builder.AppendLine($"Agent: Id: {agent.Id}, Name: {agent.Name}");

        builder.AppendLine($"instructions: {agent.Instructions}");

        builder.AppendLine($"description: {agent.Description}");

        var description = builder.ToString(); 

        Console.WriteLine(description);
    }
}
