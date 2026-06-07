## Samples 10-19

### Sample 10
Web search tool added - as per the training it searched for bing.com - haven't checked/confirmed this...

````
> what are latest finance news?

Agent > **June 3, 2026**

Here are a few **latest finance-news themes** showing up most recently:
...
````

### Sample 11 - Code Interpreter Tool (Hosted)
Note that this is hosted code interpreter tool - which spins up a container within LLM (not locally) - runs a python code and constructs a ready image representation: 

Notes:
- OpenAI Only? 
- In the training - it returned single annotation - while in the sample it returns 2 - didn't go to details. 
- Need to try to run code interpreter/compiler locally (maybe using .Net)
- Need access to OpenAIClient - so had to do helper class changes to expose the client - the file is accessed by client, container id and file id. 

```
Prompt: give me a pie chart representing countries by population - show the date of the statistics on the top
```

### Sample 12 - Structured Output
Example where we provide a format on which we want the data back, and LLM happy provides data back in that format. 

**Important Note - The weight of the agent instructions vs user input vs Description attribute**
Instructions > User instructions
Instructions > Description attribute
User Instructions ??? Description attribute (in the training example attribute took over but it's unknown 50/50). 

````
Structured output
0 - The Shawshank Redemption (1994, by Frank Darabont, YearOfRelease, imdb: 9.3)
1 - The Godfather (1972, by Francis Ford Coppola, YearOfRelease, imdb: 9.2)
2 - The Dark Knight (2008, by Christopher Nolan, YearOfRelease, imdb: 9)

````

### Sample 13 - LLM call lifecycle
Added a custom http handler to log all request/responses for LLM calls.
In the sample provided - the question is to get weather forecast for today. There are following tools:
- DateTime tool to get today's date
- Weather tool to get weather based on the date

A call to LLM follows following pattern: 

- First call dumps all data including promps, instructions, tool info etc. to LLM
LLM responds by pointing to tool call for gatDate as below
 ```"finish_reason": "tool_calls"```

- Second call to LLM includes the result of getDate function, including all data from original call:
 ```"role": "tool",
    "tool_call_id": "call_Pl9pOZYu1PFoSjrg7Y1i8XUD",
    "content": "\u00222026-06-06\u0022"```
LLM responds by pointing to next tool call getWeather with arguments: 

```
"tool_calls": [
          {
            "id": "call_Y8f7c8rHVDJMBcHE6lgZAESr",
            "type": "function",
            "function": {
              "name": "getWeather",
              "arguments": "{\u0022date\u0022:\u00222026-06-06\u0022}"
            }
```
The finish reason is still tool_calls
```"finish_reason": "tool_calls"```

- Third call to LLM includes the weather info returned from the tool
```
      "role": "tool",
      "tool_call_id": "call_Y8f7c8rHVDJMBcHE6lgZAESr",
      "content": "\u0022The weather for 6/6/2026 - Sunny weather, 30 degC, no wind\u0022"
```
LLM responds by providing the final response based on the tool call: 

```` json
 {"message": {
        "role": "assistant",
        "content": "The weather for today is: Sunny, 30\u00B0C, no wind.",
        "refusal": null,
        "annotations": []
      }}
````

The finish reason is now stop, denoting the call is complete:
```"finish_reason": "stop"```

**Important Notes:**
- All the LLM calls are stateless - all (potentially required) data is passed
- message.content is populated only on last call response. 
- finish_reason - returning tool_calls denotes follow up tool call, and stop denotes request is completed. 

### RAG - Generation 
- Used an Open AI generation model to generate vectors based on agent input/ouput. 

**Important Notes: (when using text-embedding-3-small embedding model)**
- 2 methods resulted in slightly different vectors
- Calling generate multiple times resulted in slightly different vectors
- Overall this is not perfectly repeatable

**Important Note:** When using large model (text-embedding-3-large) - Generated outputs are consistent... 

Output for small model 
```
Embedding data dimension: 1536 - printing top 10 elements
-0.0043640137,-0.046691895,-0.010047913,-0.014221191,-0.026382446,-0.019714355,0.0055656433,0.05883789,-0.0057373047,-0.039611816
Embedding data dimension: 1536 - printing top 10 elements
-0.004398346,-0.046691895,-0.010055542,-0.014198303,-0.02633667,-0.019683838,0.005558014,0.05883789,-0.0057144165,-0.039611816
Generated input data is same? False

Agent > Hello! How can I assist you today?
Embedding data dimension: 1536 - printing top 10 elements
-0.0043754578,-0.046691895,-0.010040283,-0.014205933,-0.026351929,-0.01966858,0.005554199,0.05883789,-0.0056533813,-0.03967285
Embedding data dimension: 1536 - printing top 10 elements
-0.0043754578,-0.046691895,-0.010040283,-0.014205933,-0.026351929,-0.01966858,0.005554199,0.05883789,-0.0056533813,-0.03967285
Generated output data is same? True

Calling generate async for input 10 times and check if all same?
Vectors had different elements at index 0 as below:
-0.0043640137,-0.0043754578,-0.004371643,-0.0043945312

Calling generate async for output 10 times and check if all same?
Vectors had different elements at index 0 as below:
0.007534027,0.0075149536,0.007408142
```

Output for large model: 
```
Embedding data dimension: 3072 - printing top 10 elements
-0.05218506,-0.008628845,-0.0042762756,0.014205933,0.01574707,0.0085372925,0.0099487305,0.064697266,-0.010307312,0.06008911

Embedding data dimension: 3072 - printing top 10 elements
-0.05218506,-0.008628845,-0.0042762756,0.014205933,0.01574707,0.0085372925,0.0099487305,0.064697266,-0.010307312,0.06008911

Generated input data is same? True

Agent > Hi there! How can I help?
Embedding data dimension: 3072 - printing top 10 elements
-0.05218506,-0.008628845,-0.0042762756,0.014205933,0.01574707,0.0085372925,0.0099487305,0.064697266,-0.010307312,0.06008911

Embedding data dimension: 3072 - printing top 10 elements
-0.05218506,-0.008628845,-0.0042762756,0.014205933,0.01574707,0.0085372925,0.0099487305,0.064697266,-0.010307312,0.06008911

Generated output data is same? True
Calling generate async for input 10 times and check if all same?
10 vectors same

Calling generate async for output 10 times and check if all same?
10 vectors same
```