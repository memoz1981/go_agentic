using System.ComponentModel;

namespace UdemyAICourseNotes.Tools; 

public class FakeWeatherTool
{
    [Description("Use this tool when the weather info is asked for a date")]
    public static string GetWeather([Description("The date passed as the argument - weather is provided based on the date")]DateOnly date)
    {
        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            return $"The weather for {date} - Sunny weather, 30 degC, no wind";

        if (date.Day > 15)
            return $"The weather for {date} - Cloudy, 20 degC with mild wind";

        return $"The weather for {date} - Rain, 15 degC with strong wind"; 
    }
}
