//copied from https://github.com/rwjdk/agent-framework-course/blob/main/samples/SampleUtilities/Output.cs
namespace UdemyAICourseNotes.Helpers;

public static class Output
{
    public static void Red(string message)
    {
        Write(message, ConsoleColor.Red);
    }

    public static void RedLine(string message)
    {
        WriteLine(message, ConsoleColor.Red);
    }

    public static void Green(string message)
    {
        Write(message, ConsoleColor.Green);
    }

    public static void GreenLine(string message)
    {
        WriteLine(message, ConsoleColor.Green);
    }

    public static void Yellow(string message)
    {
        Write(message, ConsoleColor.Yellow);
    }

    public static void YellowLine(string message)
    {
        WriteLine(message, ConsoleColor.Yellow);
    }

    public static void Gray(string message)
    {
        Write(message, ConsoleColor.DarkGray);
    }

    public static void GrayLine(string message)
    {
        WriteLine(message, ConsoleColor.DarkGray);
    }

    public static void Blue(string message)
    {
        Write(message, ConsoleColor.Blue);
    }

    public static void BlueLine(string message)
    {
        WriteLine(message, ConsoleColor.Blue);
    }

    public static void Magenta(string message)
    {
        Write(message, ConsoleColor.DarkMagenta);
    }

    public static void MagentaLine(string message)
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

    public static void Write(string text, ConsoleColor color)
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

    public static void Write(string text, ConsoleColor foreGround, ConsoleColor backGround)
    {
        ConsoleColor foreGroundBefore = Console.ForegroundColor;
        ConsoleColor backGroundBefore = Console.BackgroundColor; 

        try
        {
            Console.ForegroundColor = foreGround;
            Console.BackgroundColor = backGround; 
            Console.Write(text);
        }
        finally
        {
            Console.ForegroundColor = foreGroundBefore;
            Console.BackgroundColor = backGroundBefore;
        }
    }

    public static void WriteLine(string text, ConsoleColor foreGround, ConsoleColor backGround)
    {
        ConsoleColor foreGroundBefore = Console.ForegroundColor;
        ConsoleColor backGroundBefore = Console.BackgroundColor;

        try
        {
            Console.ForegroundColor = foreGround;
            Console.BackgroundColor = backGround;
            Console.WriteLine(text);
        }
        finally
        {
            Console.ForegroundColor = foreGroundBefore;
            Console.BackgroundColor = backGroundBefore;
        }
    }

    public static void YellowBg(string text)
        => Write(text, ConsoleColor.Black, ConsoleColor.Yellow);

    public static void YellowBgLine(string text)
        => WriteLine(text, ConsoleColor.Black, ConsoleColor.Yellow);

    public static void Title(string title)
    {
        Green(title);
    }
}
