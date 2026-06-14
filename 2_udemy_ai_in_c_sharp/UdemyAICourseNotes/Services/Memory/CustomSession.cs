using Microsoft.Extensions.AI;

namespace UdemyAICourseNotes.Services.Memory;

internal class CustomSession
{
    public CustomSession(List<ChatMessage> messages, string name, string description, DateTime createdAt)
    {
        Messages = messages;
        Name = name;
        Description = description;
        CreatedAt = createdAt;
    }

    public List<ChatMessage> Messages { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
