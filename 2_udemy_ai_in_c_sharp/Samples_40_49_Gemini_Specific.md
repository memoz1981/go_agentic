## Samples 40-49 - Google Gemini Specific Topics

### Sample 40 - First Gemini Demo
- Just to see if API Key / setups are working
- Almost same as Sample 1 - but this time using Gemini model

### Sample 41 - List Gemini Base Models
- Sample to list GEMINI Base models (can be retrieved from the client)
- Wonder if OpenAI/Anthropic has this feature... but really useful one. 

### Sample 42 - Reasoning with Google Gemini
- Sample provides 2 agents - one with default reasoning level and another with high
- The one with high reasoning level can apply high reasoning to a simple question like hi there - spending tokens here and there. 

### Sample 43 - Google Search Tool
- So far the best feature to use Gemini - you can do google search
- Tool can be passed in 2 ways 
a) using HostedWebSearchTool as in OpenAI
b) using the "Google way" - by passing to chat client agent options 
- Both of above are demonstrated in the demo
- So there's a fixed question - and we have 3 agents - 2 using above tools, 1 without a tools
- We ask agents to return response, date and link - as expected the one without tool returns the LLM model release date with old new
- Agents with tools return correctly. 
- Google is generous - seems that monthly 1000 web searches are free (for paid tiers)