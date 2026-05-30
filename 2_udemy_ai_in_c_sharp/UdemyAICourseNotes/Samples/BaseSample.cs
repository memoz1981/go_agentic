namespace UdemyAICourseNotes.Samples; 

internal abstract class BaseSample
{
    public abstract string Description { get; }
    public abstract Task RunAsync(); 
}
