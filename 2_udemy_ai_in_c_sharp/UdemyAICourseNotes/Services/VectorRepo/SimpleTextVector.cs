using Microsoft.Extensions.VectorData;

namespace UdemyAICourseNotes.Services.VectorRepo;

internal class SimpleTextVector : BaseVector
{
    public SimpleTextVector(string text) : base()
    {
        Text = text;
    }

    public SimpleTextVector()
    {
        
    }

    [VectorStoreData]
    public string Text { get; private set; }
}
