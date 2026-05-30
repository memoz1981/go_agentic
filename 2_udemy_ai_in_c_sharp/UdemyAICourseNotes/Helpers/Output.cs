//copied from https://github.com/rwjdk/agent-framework-course/blob/main/samples/SampleUtilities/Output.cs
namespace UdemyAICourseNotes.Helpers;

public static class Output
{
    public static void Red(string message)
    {
        Write(message, ConsoleColor.Red);
    }

    public static void Green(string message)
    {
        Write(message, ConsoleColor.Green);
    }

    public static void Yellow(string message)
    {
        WriteLine(message, ConsoleColor.Yellow);
    }

    public static void Gray(string message)
    {
        WriteLine(message, ConsoleColor.DarkGray);
    }

    public static void Blue(string message)
    {
        WriteLine(message, ConsoleColor.Blue);
    }

    public static void Magenta(string message)
    {
        WriteLine(message, ConsoleColor.DarkMagenta);
    }

    public static void Separator(bool preAndPostLinebreak = true)
    {
        if (preAndPostLinebreak)
        {
            Console.WriteLine();
        }

        WriteLine("".PadLeft(Console.WindowWidth, '-'), ConsoleColor.Gray);

        if (preAndPostLinebreak)
        {
            Console.WriteLine();
        }
    }

    private static void WriteLine(string text, ConsoleColor color)
    {
        ConsoleColor orgColor = Console.ForegroundColor;
        try
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
        }
        finally
        {
            Console.ForegroundColor = orgColor;
        }
    }

    private static void Write(string text, ConsoleColor color)
    {
        ConsoleColor orgColor = Console.ForegroundColor;
        try
        {
            Console.ForegroundColor = color;
            Console.Write(text);
        }
        finally
        {
            Console.ForegroundColor = orgColor;
        }
    }

    public static void Title(string title)
    {
        Green(title);
    }
}
