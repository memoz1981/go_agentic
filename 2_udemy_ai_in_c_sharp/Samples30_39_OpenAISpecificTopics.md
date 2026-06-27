## Samples 30-39 - Open AI Specific Topics

### Caching Tokens - No sample exists
- Caching tokens work when input tokens are above some pre-set value (typically 1000 or above)
- The advantage of the caching is it's 10 times cheaper than input tokens
- Cache is not saved by session, rather by API_KEY used - and typically expires in an hour or so. 

### Service Tiers
- There are two service tiers - default and priority 
- Probably 'priority' tier is the one for use for production apps
- There are following ways to set service tier
a) Thru project settings - then an api key can be produced for that project that will use the corresponding tier. 
b) Thru chat client options - See Sample 31

### Sample 31 - Setting Service Tier
- Have 2 agents - one for priority one for default
- Ask questions and observe the time spent to get the response
- Basically didn't observe much difference even often default returned before priority
- Latency is not guaranteed with default. 
- My examples were basic - which may be why I didn't observe much difference. 

