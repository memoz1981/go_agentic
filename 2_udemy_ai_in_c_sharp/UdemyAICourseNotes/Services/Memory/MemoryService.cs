namespace UdemyAICourseNotes.Services.Memory; 

internal class MemoryService
{
    private readonly string _filePath;

    public MemoryService(string filePath)
        => _filePath = filePath;

    public async Task<List<string>> GetMemory()
        => File.Exists(_filePath) ? (await File.ReadAllLinesAsync(_filePath)).ToList() : [];

    public async Task SetMemory(List<string> data)
        => await File.WriteAllLinesAsync(_filePath, data);
}
