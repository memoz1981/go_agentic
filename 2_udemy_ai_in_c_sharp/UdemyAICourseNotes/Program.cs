using UdemyAICourseNotes.Samples;
using static UdemyAICourseNotes.Helpers.Output; 

var sample1 = new _1_Basic_Chat_Client();
var sample2 = new _2_Agent_Sessions();
var sample3 = new _3_Normal_Vs_Streaming();
var sample4 = new _4_Token_Usage();
var sample5 = new _5_Creating_Tools();
var sample6 = new _6_Mcp_Tools();
var sample7 = new _7_Tools_Middleware(); 

BaseSample[] samples = [ sample1, sample2, sample3, sample4, sample5, sample6, sample7 ];

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
