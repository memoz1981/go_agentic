## Samples 20-29

## Sample 20 - Using ContextProvider and Memory Agent
- It's possible to provide ContextProvider to provide/save additional context for an agent
- Here used a normal agent for memory + context provider
- A main agent acts as the user interface
a) Agent manages the user input
b) Context provider manages context to/from the user and uses memory agent for this
c) The facts are stored in a txt file - it's also possible to have other persistence types
- Memory vs Session
a) Session stores memory within same session - when from local for example stopping and starting the app resets everything. 
b) Memory is persisted in txt file and provides persistence between different sessions

```
> what is my name?
Agent response: > I don't know your name from what's shown here ...

> my name is Mehdi
Adding user fact: User's name is Mehdi.
Saved file with 1 count facts...
Agent response: > Nice to meet you, Mehdi! ?? I'll remember your name for this chat.
> what is my name?
Agent response: > Your name is **Mehdi**.

Stop and re-start the app - again select sample 20
Note: Check the referenced txt file - now the file has 1 entry for the file: 
Loading facts with 1 count user facts
> what is my name?
Agent response: > Your name is **Mehdi**.

> forget all my information
Removing user fact: User's name is Mehdi.
Saved file with 0 count facts...
Agent response: > I can't literally "forget" in the middle of this chat, but I will stop using any information you've shared and won't reference your name or other details. What would you like to do next?
> what is my name?
Agent response: > Sorry, I don't have access to your personal info anymore in this chat. I don't know your name.

```

**Notes**
- This implementation has some cons - if you tell my name is Mehdi, then my name is MehdiZ - it will store both - may use a dictionary
instead to store key/value pairs - and it may use a text serialization/deserialization to/from dictionary. 
- Will add Sample 21 - to just have a tool that will almost do the same - write to/from txt file - without the context provider. Still there's a need for an agent to manage memory to add / memory to remove... 

### Sample 21 - Using memory agent as tool (alternative to Sample 20 ContextProvider)
- We can create the memory agent directly calling memory service to get/set the memory
- Main agent has the memory agent as tool
- You may use sample 20 inputs - should get the same response
- Just in the example it goes to the file each time - instead could do it in memory, but not required for the sake of the sample. 