using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.SqliteVec;
using System.Threading.Tasks;

namespace UdemyAICourseNotes.Services.VectorRepo;

internal class SqlLiteVectorRepo<T> : IVectorRepo<T> where T : BaseVector
{
    public VectorStore VectorStore { get; }

    public SqlLiteVectorRepo(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
    {
        var connectionString = $"Data Source={Path.GetTempPath()}\\sample15_SqLite.db";

        VectorStore = new SqliteVectorStore(connectionString, new SqliteVectorStoreOptions()
        {
            EmbeddingGenerator = embeddingGenerator
        });
    }

    public async Task<VectorStoreCollection<Guid, T>> GetCollection(string tableName)
    {
        var vectorStoreCollection = VectorStore.GetCollection<Guid, T>(tableName);

        var collectionExists = await vectorStoreCollection.CollectionExistsAsync();

        if (collectionExists)
            return vectorStoreCollection; 
        
        await vectorStoreCollection.EnsureCollectionExistsAsync();

        return vectorStoreCollection; 
    }

    public async Task DeleteCollection(string tableName)
    {
        var vectorStoreCollection = VectorStore.GetCollection<Guid, T>(tableName);

        var collectionExists = await vectorStoreCollection.CollectionExistsAsync();

        if (!collectionExists)
            return;

        await vectorStoreCollection.EnsureCollectionDeletedAsync(); 
    }

    public async Task<bool> CollectionExists(string tableName)
    {
        var vectorStoreCollection = VectorStore.GetCollection<Guid, T>(tableName);

        return await vectorStoreCollection.CollectionExistsAsync();
    }

    public async Task Insert(VectorStoreCollection<Guid, T> vectorStoreCollection, IEnumerable<T> data)
    {
        foreach (var entry in data)
        {
            await vectorStoreCollection.UpsertAsync(entry); 
        }
    }

    public IAsyncEnumerable<T> GetAll(VectorStoreCollection<Guid, T> vectorStoreCollection)
    {
        return vectorStoreCollection.GetAsync(r => 1 == 1, int.MaxValue); 
    }

    public async Task<T> Get(VectorStoreCollection<Guid, T> vectorStoreCollection, Guid key)
        => await vectorStoreCollection.GetAsync(key);

    public IAsyncEnumerable<VectorSearchResult<T>> Search(
        VectorStoreCollection<Guid, T> vectorStoreCollection, 
        string input)
        => vectorStoreCollection.SearchAsync(input, 10); 
}
