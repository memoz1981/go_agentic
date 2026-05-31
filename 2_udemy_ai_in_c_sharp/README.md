## My Notes and Practices from Udemy Course "AI in C# using the Microsoft Agent Framework"

**Course:** AI in C# using the Microsoft Agent Framework
**Instructor:** Rasmus Wulff Jensen

The notes/samples may not always follow the order provided by the udemy course. 

See "Samples" folder for the examples - with the notes backed up here. 

### _1_Basic_Chat_Client
This is just a demonstration of end-to-end working chat client - also to hook up the agent construction. 
Using github models and OpenAI model "gpt-4o-mini". 

#### Construction of the AI agent: 
`new OpenAIClient(new ApiKeyCredential(token), new OpenAIClientOptions { Endpoint = new Uri(OPEN_AI_ENDPPOINT) })` - returns new OpenAIClient object

`openAIClient.GetChatClient(model)` - returns ChatClient object - note there seems to be lots of other clients like `AssistantClient`, `AudioClient`, `ResponseClient` etc. 

`chatClient.AsAIAgent(name, instructions, tools)` - returns ChatClientAgent object which is the agent itself. 

`chatClientAgent(input)` - returns `AgentResponse` object that includes the response text that can be retrieved by converting the object `ToString()`

### _2_Agent_Sessions

Sessions can be added to agents - this ensures that the context is kept between the prompts. 
Two agents are created - one with session and other without: 

```
> my name is Mehdi

Agent without session: > Nice to meet you, Mehdi! How can I assist you today?
Agent with session: > Hello Mehdi! How can I assist you today?
------------------------------------------------------------------------------------------------------------------------
> what is my name?

Agent without session: > I don't have access to personal information about users unless you provide it. Please let me know your name if you want me to address you by it!
Agent with session: > Your name is Mehdi. How can I help you further?
------------------------------------------------------------------------------------------------------------------------
```

The history is stored in StateBag and can be retrieved either by: 
- directly accessing the StateBag
- OR getting the service `InMemoryChatHistoryProvider` for the agent and then getting the history for the session. 

### _3_Normal_Vs_Streaming

Demonstrates the streaming output - raw output is good enough for most of the applications. Some examples where we may use streaming output: 
- Long responses
- Chat UX
- Early termination
- Code generation

### _4_Token_Usage

Comparison of the token usage for the agents with and without the sessions. Without the session it's almost same for similar prompts. For the ones with session, context always adds up resulting in more input tokens each time, output token is not impacted. (but maybe under some under conditions this might happen)

Main types of the tokens: 
- Input
- Output
- Reasoning

### _5_Creating_Tools

Example of tool creation and usage. 

```
> what is todays date time and tomorrows date time and 24 hours from now date time

Agent > Today's date is **2026-05-30** at 15:00.
Tomorrow's date is **2126-06-01** at 16:00.
24 hours from now will also be **2126-06-01** at 16:00.

```

Note: In the above example although we are asking for 24 hours from now - agent interprets that as tomorrow and returns tomorrow's overriden date from the function. 

### _6_Mcp_Tools
MCP - Model Context Protocol - the tools that are available online or offline that are like ready to use tools. 

In the example I used Microsoft Learn MCP Tool - since github model credits were not sufficient switched to using OpenAI client. 

```
> What is the latest version of .Net ? Write just the version as answer.

Agent without tools> .NET 9

Agent with tools> .NET 10
```

