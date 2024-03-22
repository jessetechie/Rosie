using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Rosie;

public class Input
{
    public List<int> Numbers { get; init; } = new();
}

public class Program
{
    static async Task Main()
    {
        var input = new Input
        {
            Numbers = new List<int> { 1, 2, 3 }
        };

        string[] scripts =
        { 
            "return Numbers.Where(x => x > 1).Select(x => x * 2).ToList();",
            "return Numbers.Concat(new []{9,10,11}).ToList();",
            "return Numbers.Select(x => x * 2).ToList();",
            "return Numbers.Where(x => x < 20).ToList();",
            """
            var results = new List<int>();
            foreach (var number in Numbers) 
            {
                if (number.ToString().Contains("8"))
                {
                    continue;
                }
                else
                {
                    results.Add(number);
                }
            }
            return results;
            """
        };
        
        foreach (var script in scripts)
        {
            var result = await RunScript(script, input);
            Console.WriteLine($"Results: {string.Join(", ", result)}");
            input = new Input{Numbers = result};
        }
    }

    private static async Task<List<int>> RunScript(string script, Input input)
    {
        var options = ScriptOptions.Default
            .WithReferences(typeof(System.Linq.Expressions.Expression).Assembly)
            .WithImports("System", "System.Linq", "System.Collections.Generic");
        
        return await CSharpScript.EvaluateAsync<List<int>>(script, options, input);
    }
}