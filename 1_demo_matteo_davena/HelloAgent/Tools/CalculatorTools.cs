using System.ComponentModel;

namespace HelloAgent.Tools;

public static class CalculatorTools
{
    [Description("Calculates a math expression. Use for any mathematical calculation.")]
    public static string Calculate(
        [Description("First operand")] double a,
        [Description("Operator: +, -, *, /")] string op,
        [Description("Second operand")] double b)
    {
        var result = op switch
        {
            "+" => a + b,
            "-" => a - b,
            "*" => a * b,
            "/" => b != 0 ? a / b : double.NaN,
            _ => double.NaN
        };
        return double.IsNaN(result)
            ? $"Error: invalid operation '{a} {op} {b}'"
            : $"{a} {op} {b} = {result}";
    }

    [Description("Calculates a percentage of a number.")]
    public static string CalculatePercentage(
        [Description("The percentage value (e.g., 15 for 15%)")] double percentage,
        [Description("The number to calculate the percentage of")] double number)
    {
        var result = number * percentage / 100;
        return $"{percentage}% of {number} = {result}";
    }
}
