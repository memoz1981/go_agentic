using UdemyAICourseNotes.Samples;
using UdemyAICourseNotes.Samples._10;
using UdemyAICourseNotes.Samples._20;
using UdemyAICourseNotes.Samples._30_Open_AI_Topics;
using UdemyAICourseNotes.Samples._40_Gemini_Topics;
using UdemyAICourseNotes.Samples._50_Anthropic_Topics;
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
var sample18 = new _18_Multi_Modal();
var sample19 = new _19_Empty();

var sample20 = new _20_Memory_Agent();
var sample21 = new _21_Memory_Tool();
var sample22 = new _22_Memory_Conversation_Persistence();
var sample23 = new _23_AI_As_Data_Filter();
var sample24 = new _24_Chat_History_Reducers_Empty();
var sample25 = new _25_Workflows_Intruduction();
var sample26 = new _26_Workflow_Without_Workflows();
var sample27 = new _27_Agent_As_Workflow_Orchestrator();
var sample28 = new _28_Workflows_Optimized(); 
var sample29 = new _29_Workflows_Optimized_Claude();

var sample30 = new _30_Service_Tiers();

var sample40 = new _40_Basic_Gemini_Agent();
var sample41 = new _41_List_Models();
var sample42 = new _42_Reasoning();
var sample43 = new _43_Google_Web_Search_Tool();
var sample44 = new _44_Google_Maps();

var sample50 = new _50_Reasoning(); 

BaseSample[] samples = 
    [ sample1, sample2, sample3, sample4, sample5, sample6, sample7, sample8, sample9,
      sample10, sample11, sample12, sample13, sample14, sample15, sample16, sample17, sample18, sample19,
      sample20, sample21, sample22, sample23, sample24, sample25, sample26, sample27, sample28, sample29,
      sample30, BaseSample.EMPTY, BaseSample.EMPTY, BaseSample.EMPTY, BaseSample.EMPTY, BaseSample.EMPTY, BaseSample.EMPTY, BaseSample.EMPTY, BaseSample.EMPTY, BaseSample.EMPTY,
      sample40, sample41, sample42, sample43, sample44, BaseSample.EMPTY, BaseSample.EMPTY, BaseSample.EMPTY, BaseSample.EMPTY, BaseSample.EMPTY,
      sample50, BaseSample.EMPTY, BaseSample.EMPTY, BaseSample.EMPTY, BaseSample.EMPTY, BaseSample.EMPTY, BaseSample.EMPTY, BaseSample.EMPTY, BaseSample.EMPTY, BaseSample.EMPTY, 
    ];

Red("Type the index of the sample to continue:");
Console.WriteLine();
Console.WriteLine();

for (int i = 0; i < samples.Length; i++)
{
    if (samples[i].Description != "EMPTY")
    {
        Blue($"({i + 1}) - {samples[i].Description}");

        Console.WriteLine();
    }

    if ((i + 2) % 10 == 0)
        Separator();
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
