namespace UdemyAICourseNotes.Samples; 

internal abstract class BaseSample
{
    public abstract string Description { get; }
    public abstract Task RunAsync();
    protected const string EXIT = "exit";
    public static BaseSample EMPTY = new EmptySample(); 

    private class EmptySample : BaseSample
    {
        public override string Description => "EMPTY";

        public override Task RunAsync()
        {
            throw new NotImplementedException();
        }
    }
}
