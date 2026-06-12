using Microsoft.Extensions.VectorData;

namespace UdemyAICourseNotes.Services.VectorRepo; 

internal interface IVectorRepo<T> : IDisposable where T : BaseVector
{
    VectorStore VectorStore { get; }
    Task<VectorStoreCollection<Guid, T>> GetCollection(string tableName);
    Task DeleteCollection(string tableName);
    Task<bool> CollectionExists(string tableName);
    Task Insert(VectorStoreCollection<Guid, T> vectorStoreCollection, IEnumerable<T> data);
    IAsyncEnumerable<T> GetAll(VectorStoreCollection<Guid, T> vectorStoreCollection);
    Task<T> Get(VectorStoreCollection<Guid, T> vectorStoreCollection, Guid key);
    IAsyncEnumerable<VectorSearchResult<T>> Search(
        VectorStoreCollection<Guid, T> vectorStoreCollection,
        string input);
}
