using Microsoft.Extensions.VectorData;

namespace UdemyAICourseNotes.Services.VectorRepo; 

internal abstract class BaseVector
{
    public BaseVector()
    {
         Id = Guid.NewGuid();
    }

    [VectorStoreKey]
    public Guid Id { get; set; }
    
    //under the hood, magically it creates tables for vectors...
    [VectorStoreVector(3072)]
    public string Vector => "Hi there, I'm a vector..."; //well why is it required at all? 
}
