using UdemyAICourseNotes.Samples;
using static UdemyAICourseNotes.Helpers.Output; 

var sample1 = new _1_Basic_Chat_Client();
var sample2 = new _2_Agent_Sessions();
var sample3 = new _3_Normal_Vs_Streaming();

BaseSample[] samples = [ sample1, sample2, sample3 ];

Red("Type the index of the sample to continue:");
Console.WriteLine();
Console.WriteLine();

for (int i = 0; i < samples.Length; i++)
{
    Blue($"({i}) - {samples[i].Description}");
    Console.WriteLine(); 
}

Console.WriteLine();
Gray("> "); 
var result = Console.ReadLine();

if (!int.TryParse(result, out var index) || index < 0 || index >= samples.Length)
{
    Red($"Selected option {result} is not valid.");
    return;
}

var sample = samples[index];

Separator(); 

await sample.RunAsync(); 
