using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using UdemyAICourseNotes.Services.VectorRepo;
using static UdemyAICourseNotes.Helpers.Output; 

namespace UdemyAICourseNotes.Tools;

internal record  VectorSearchTool(
    SqlLiteVectorRepo<SimpleTextVector> Repo, 
    VectorStoreCollection<Guid, SimpleTextVector> VectorStoreCollection) : IDisposable
{
    public void Dispose()
    {
        Repo?.Dispose();
    }

    public async Task<List<ChatMessage>> Search(string input)
    {
        var chatMessageList = new List<ChatMessage>();
        chatMessageList.Add(new ChatMessage(ChatRole.User, input));

        //write the top 3 vector search results and add to list to send to LLM
        BlueLine("Getting top 3 elements from the vector store:");
        int index = 1;
        await foreach (var vectorSearchResult in Repo.Search(VectorStoreCollection, input).OrderByDescending(r => r.Score).Take(3))
        {
            BlueLine($"Text: {vectorSearchResult.Record.Text}, Score: {vectorSearchResult.Score}");

            var chatMessage = $"This is the {index}th relevant information: {vectorSearchResult.Record.Text}";
            chatMessageList.Add(new ChatMessage(ChatRole.User, chatMessage));
        }

        return chatMessageList;
    }
}
