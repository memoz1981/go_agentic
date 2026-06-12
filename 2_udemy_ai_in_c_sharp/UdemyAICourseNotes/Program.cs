using UdemyAICourseNotes.Samples;
using UdemyAICourseNotes.Samples._10;
using static UdemyAICourseNotes.Helpers.Output; 

var sample1 = new _1_Basic_Chat_Client();
var sample2 = new _2_Agent_Sessions();
var sample3 = new _3_Normal_Vs_Streaming();
var sample4 = new _4_Token_Usage();
var sample5 = new _5_Creating_Tools();
var sample6 = new _6_Mcp_Tools();
var sample7 = new _7_Tools_Middleware();
var sample8 = new _8_Agents_As_Tools();
var sample9 = new _9_Agents_As_Tools_2();

var sample10 = new _10_Web_Search_Tool();
var sample11 = new _11_Code_Interpreter_Tool();
var sample12 = new _12_Structured_Output();
var sample13 = new _13_LLM_Call_Lifecycle();
var sample14 = new _14_RAG_Generation();
var sample15 = new _15_RAG_VectorStore();
var sample16 = new _16_RAG_As_A_Tool();
var sample17 = new _17_Reasoning(); 

BaseSample[] samples = 
    [ sample1, sample2, sample3, sample4, sample5, sample6, sample7, sample8, sample9,
      sample10, sample11, sample12, sample13, sample14, sample15, sample16, sample17];

Red("Type the index of the sample to continue:");
Console.WriteLine();
Console.WriteLine();

for (int i = 0; i < samples.Length; i++)
{
    Blue($"({i+1}) - {samples[i].Description}");

    if ((i + 2) % 10 == 0)
        Separator();
    else
        Console.WriteLine(); 
}
Separator();
Console.WriteLine();
Gray("> "); 
var result = Console.ReadLine();

if (!int.TryParse(result, out var index) || index < 1 || index > samples.Length)
{
    Red($"Selected option {result} is not valid.");
    return;
}

var sample = samples[index-1];

Separator(); 

await sample.RunAsync(); 
