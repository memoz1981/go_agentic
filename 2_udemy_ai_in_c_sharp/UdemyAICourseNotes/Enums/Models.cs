namespace UdemyAICourseNotes.Enums;

public static class Models
{
    public static class OpenAI
    {
        public const string GPT_4o_MINI = "gpt-4o-mini";
        public const string GPT_5_4_NANO = "gpt-5.4-nano";
        public const string GPT_5_4_MINI = "gpt-5.4-mini";
        public const string GPT_5_4 = "gpt-5.4";
        public const string GPT_4_1_NANO = "gpt-4.1-nano";
    }

    public static class OpenAIEmbedding
    {
        public const string SMALL_3 = "text-embedding-3-small";
        public const string LARGE_3 = "text-embedding-3-large";
    }

    public static class Claude
    {
        public const string OPUS_4_8 = "claude-opus-4-8";
        public const string SONNET_4_6 = "claude-sonnet-4-6";
        public const string HAIKU_4_5 = "claude-haiku-4-5";
    }
}
