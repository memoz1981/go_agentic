using Microsoft.Extensions.AI;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Extensions;

internal static class EmbeddingExtensions
{
    internal static void Print(this Embedding<float> data)
    {
        RedLine($"Embedding data dimension: {data.Dimensions} - printing top 10 elements");

        RedLine(string.Join(',', data.Vector.ToArray().Take(10)));
        Console.WriteLine();
    }

    internal static void Print(this ReadOnlyMemory<float> data)
    {
        MagentaLine($"Embedding data dimension: {data.Length} - printing top 10 elements");

        MagentaLine(string.Join(',', data.ToArray().Take(10)));
        Console.WriteLine();
    }

    internal static bool IsSame(this Embedding<float> data, ReadOnlyMemory<float> dataToCompare)
    {
        if (data.Dimensions != dataToCompare.Length)
            return false;
        var array = data.Vector.ToArray();
        var arrayToCompare = dataToCompare.ToArray();

        for (int i = 0; i < data.Dimensions; i++)
        {
            if (array[i] != arrayToCompare[i])
                return false; 
        }

        return true; 
    }

    internal static bool IsSame(this IEnumerable<Embedding<float>> data)
    {
        if (data.Select(dt => dt.Dimensions).Distinct().Count() != 1)
        {
            GrayLine("Dimensions were not same...");
            Console.WriteLine();
            return false;
        }
            
        var arrays = data.Select(dt => dt.Vector.ToArray()).ToArray();

        var dimension = arrays[0].Length; 

        for (int i = 0; i < dimension; i++)
        {
            if (arrays.Select(dt => dt[i]).Distinct().Count() != 1)
            {
                GrayLine($"Vectors had different elements at index {i} as below:");
                GrayLine(string.Join(',', arrays.Select(dt => dt[i]).Distinct())); 
                return false;
            }
        }

        GrayLine($"{data.Count()} vectors same");
        Console.WriteLine();
        return true;
    }
}
