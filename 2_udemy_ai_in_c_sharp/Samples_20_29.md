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

### Sample 22 - Session storage using ChatHistoryProvider
- This is the behaviour currently implemented by public LLMs like OpenAI, Claude etc. when logging to account - the chat history is presented which has
a) The timestamps
b) The descriptions 
c) Raw history
- To add above functionality ChatHistoryProvider can be used - all we need to do is to have some session knowledge (unfortunately it doesn't come built in - so had to add CustomSession class) - once we identify the session we are in (new session, or an existing session):
a) We need to ensure that related history can be loaded from the unique session identifier
b) We could save into the history

```
Select one of the following sessions:
0 - 6/14/2026 6:31:58 AM - who is barack obama?
1 - 6/14/2026 6:32:50 AM - who is putin?
-1 to start a new session:

0

user > who is barack obama?
assistant > Barack Obama(???·???)is an American politician and public figure. He served as the **44th President of the United States** from **2009 to 2017**.

- He is a member of the **Democratic Party**.
- Before being president, he worked as a **lawyer** and served in **the U.S. Senate** (from Illinois).
- He is well known for major changes in U.S. policy, including the **Affordable Care Act** (often called Obamacare).
- He was the **first African American** to become U.S. president.
user > how tall is he?
assistant > Barack Obama is **about 6 feet 1 inch** tall (around **185 cm**).
user > when was he the president?
assistant > Barack Obama was the President of the United States from **January 20, 2009** to **January 20, 2017**.


> what is his age?
Agent response: > Barack Obama was born on **August 4, 1961**.
As of **today (June 14, 2026)**, he is **64 years old** (turning **65** on August 4, 2026).
```

As seen from above - once session is loaded - all messages from the session can be accessed just like OpenAI or Claude (and other LLMs).

### Sample 23 - Filters using AI
- Used when scenarios where we are working with large data - which costs a lot to send to LLM and we need filtering
- If we try to send whole data each time to LLM - it has following cons: 
a) A lot of input token usage
b) Repeated data 
c) A lot of reasoning token usage
d) Potentially a lot of output token usage
e) It will be quite slow, especially with large data size...
- What we want to achieve is another layer from a normal Web App with filtering - where we want to convert text to normal back-end filter classes 
- The main difference from the vectors is that - vectors are similarity search (like dog-animal), while filters provide exact searches (like contains text, year between dates etc.)
- The solution works as follows:
a) Again if we try to send raw data - we need to send all books to LLM (like normalAgent in the example) concatted with the question - LLM will reason and send the filtered data back - cons of which are described above. Advanced models are very good in reasoning, so probably the data will be all right - with drawback of cost and efficiency
b) We have a book class and book filter class - all we have to do is to ensure book filter class can fully represent the filter functionality
c) We ask LLM to convert the query to book filter array (query may include multiple filter like author containing, year equals etc.)
d) We don't send the book list even the book class structure to LLM - all it needs to know is the question and the BookFilter class structure
e) LLM populates array of the book filters 
f) We send the book filters to Back-End and get the results - we don't need to send it back to LLM unless we want to do anything further with it... 

```
> give me all books written after 18th century (inclusive) and name including adventures
Normal Agent response: > Books **written after the 18th century (inclusive)** from your list (and with "adventures" in the title):
- **Alice's Adventures in Wonderland** - *1865* - Lewis Carroll
- **The Adventures of Huckleberry Finn** - *1884* - Mark Twain

Filter Agent response (filters): > 
YearOfRelease - GreaterThanOrEqual - 1700
Title - Contains - adventures

Filtered Output as per the filters provided by the filter agent:
Alice's Adventures in Wonderland - 1865 - Lewis Carroll - Fantasy - A girl named Alice falls through a rabbit hole into a fantasy world.
The Adventures of Huckleberry Finn - 1884 - Mark Twain - Adventure - A young boy and a runaway slave travel down the Mississippi River.

```
As seen from above (and from the code:) - we don't need to send book list o LLM - just the request itself + bookFilter (to ask for structured output). 

### Sample 25 - Workflows Introduction

Workflows provide functionality to manage agent flow - agents can run sequentially, conditionally branch, merge, go back etc. Almost like the "Basic programming language" functionality. 

In the provided example multiple agent/workflows are defined for followings:
- Take appointment details
- Convert details to structured output
- Filter details based on the availability
a) If no slots - say sorry
b) If one slot - book and confirm
c) If multiple slots go back to user to select single hour. 

### Sample 26 - Workflow without workflows
- Basically trying to set up the workflow using logical statements - trying to use agents directly
- What I found that with complex workflows, it's getting tricky - while it's possible to use if/else for switch statements, 
or concurrently run the tasks, or chain events - it's very tricky to go back to a previous stage for example. 

It's almost like "Basic Programming Language" functionality with go-to statements. 

Notes on workflows:
- Sometimes it's much easier and cleaner to use just logical expressions in the code and directly use agents instead of workflows.
But when we need to go back to a previous stage - it's becoming tricky. 

In next example will try to explore using an orchestrator agent, let's see how it goes. 

**Important Note:** What I found that the agent interactions were much more precise and clear when directly using agents than in workflow.
That may be due to I was passing instructions/chat messages in workflows, while the agents had instructions only. 
Also the instructions I wrote in agents were much clean/precise. Besides - sessions should really handle everything - no need for 
manual interventions. 

### Sample 27 - Agent as Workflow Orchestrator
- Using agent as orchestrator - to my surprize it worked 
- So far the cleanest solution is Sample 26 - where we just use logical expressions to manage the workflow, with one con - it doesn't go back if the user asks - I think that's the sole reason of having workflows
- Overall this solution works, not very clean - some adjustments required, but will not spend much time on it. 

See on Next Section - Will try workflows to have the appointment app - will improve to go back and to be able to accept multiple requests and be able to identify booked slots. 

**Important Note:** In above examples what I found tricky is - initial start and links between the agents. 
- For example in Sample 27 - main agent starts taking appointment details, then passes to appointmnet agent - which does the same, and since the session is not shared - everything starts from start
- In work flow related examples - hard to go back - will try in next example
