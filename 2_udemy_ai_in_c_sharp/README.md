## My Notes and Practices from Udemy Course "AI in C# using the Microsoft Agent Framework"

**Course:** AI in C# using the Microsoft Agent Framework
**Instructor:** Rasmus Wulff Jensen

The notes/samples may not always follow the order provided by the udemy course. 

See "Samples" folder for the examples - with the notes backed up here.

### _1_Basic_Chat_Client
This is just a demonstration of end-to-end working chat client - also to hook up the agent construction. 
Using github models and OpenAI model "gpt-4o-mini". 

Construction of the AI agent: 
`new OpenAIClient(new ApiKeyCredential(token), new OpenAIClientOptions { Endpoint = new Uri(OPEN_AI_ENDPPOINT) })` - returns new OpenAIClient object

`openAIClient.GetChatClient(model)` - returns ChatClient object - note there seems to be lots of other clients like `AssistantClient`, `AudioClient`, `ResponseClient` etc. 

`chatClient.AsAIAgent(name, instructions, tools)` - returns ChatClientAgent object which is the agent itself. 

`chatClientAgent(input)` - returns `AgentResponse` object that includes the response text that can be retrieved by converting the object `ToString()`



