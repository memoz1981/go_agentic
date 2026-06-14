using Microsoft.Extensions.AI;
using System.Globalization;
using System.Text.Json;

namespace UdemyAICourseNotes.Services.Memory;

internal class SessionService
{
    private readonly string _directoryPath;
    public SessionService()
    {
        _directoryPath = Path.Combine(Path.GetTempPath(), "sessions"); 

        if(!Directory.Exists(_directoryPath))
            Directory.CreateDirectory(_directoryPath);
    }

    public CustomSession CreateEmptySession()
    {
        var dateTime = DateTime.UtcNow;

        return new CustomSession([], dateTime.ToString("yyyy-MM-ddTHH-mm-ss"), string.Empty, dateTime);
    }

    public async Task<CustomSession> GetSession(string sessionName)
    {
        var filePath = Path.Combine(_directoryPath, $"{sessionName}.txt");
        var dateTime = DateTime.SpecifyKind(
            DateTime.ParseExact(sessionName, "yyyy-MM-ddTHH-mm-ss", 
            CultureInfo.InvariantCulture), DateTimeKind.Utc);

        if (!File.Exists(filePath))
        {
            return new CustomSession([], sessionName, string.Empty, dateTime); 
        }

        var fileContent = await File.ReadAllTextAsync(filePath);

        var messages = JsonSerializer.Deserialize<List<ChatMessage>>(fileContent);

        var firstMessage = messages.OrderBy(message => message.CreatedAt)
            .FirstOrDefault()?
            .Text ?? "Empty";

        var description = firstMessage.Substring(0, Math.Min(20, firstMessage.Length));
        
        return new CustomSession(messages, sessionName, description, dateTime); 
    }

    public async Task<List<CustomSession>> GetAllSessions()
    {
        var files = Directory
            .GetFiles(_directoryPath)
            .Select(file => Path.GetFileNameWithoutExtension(file))
            .Select(GetSession);

        return (await Task.WhenAll(files)).ToList(); 
    }

    public async Task SaveSession(CustomSession session)
    {
        if (session.Messages is null or [])
            return; 
        
        var fullPath = Path.Combine(_directoryPath, $"{session.Name}.txt");

        await File.WriteAllTextAsync(fullPath, JsonSerializer.Serialize(session.Messages ?? [])); 
    }
}
