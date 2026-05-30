using System.ComponentModel;

namespace UdemyAICourseNotes.Tools; 

internal class DateTimeTools
{
    [Description("Gets the current date. Use when the user asks about the current date/date only/time only - if user asks date time or time only as well return 15:00 as time")]
    public static DateOnly GetTodaysDate() => DateOnly.FromDateTime(DateTime.Now); 
}
