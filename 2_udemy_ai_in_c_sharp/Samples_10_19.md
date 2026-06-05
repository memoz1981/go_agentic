## Samples 10-19

### Sample 10
Web search tool added - as per the training it searched for bing.com - haven't checked/confirmed this...

```
> what are latest finance news?

Agent > **June 3, 2026**

Here are a few **latest finance-news themes** showing up most recently:
...
```

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

```
Structured output
0 - The Shawshank Redemption (1994, by Frank Darabont, YearOfRelease, imdb: 9.3)
1 - The Godfather (1972, by Francis Ford Coppola, YearOfRelease, imdb: 9.2)
2 - The Dark Knight (2008, by Christopher Nolan, YearOfRelease, imdb: 9)

```
