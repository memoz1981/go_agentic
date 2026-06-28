## Samples 50-59 - Anthropic Specific Topics

### Sample 50 - Reasoning in Anthropic
- Important catcha is Antrhopic models don't return back the number of the reasoning tokens used
- Default reasoning is very weak - so need to specify it
- So with OPUS 4.8 - not sure about above - but for weaker models like Haiko it may be required
- For OPUS it didn't allow me to set thinking enabled to true - rather it should be adaptive
- Overall with this configuration, with Haiko model, although we asked it to return 3 words, default agent returned more than 3
- Agent with reasoning token could return correctly with 3 words output
- Reasoning tokens not returned (added to output tokens)