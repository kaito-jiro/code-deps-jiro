using System;
using System.Collections.Generic;

namespace DepGraph.Cli;

public static class ArgumentParser
{
    public static Options Parse(string[] args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException("Input path is required.");
        }

        var inputPath = args[0];
        var options = new Options { InputPath = inputPath };

        var flags = new Dictionary<string, Action<string?>>
        {
            { "--dot", _ => options.OutputDot = true },
            { "--filter", value => options.FilterPattern = value },
            { "--rules", value => options.RulesFile = value },
            { "--exclude", value => options.ExcludePattern = value },
        };

        for (var i = 1; i < args.Length; i++)
        {
            var arg = args[i];
            if (!flags.TryGetValue(arg, out var apply))
            {
                throw new ArgumentException($"Unknown option: {arg}");
            }

            if (arg == "--dot")
            {
                apply(null);
                continue;
            }

            if (i + 1 >= args.Length)
            {
                throw new ArgumentException($"Option {arg} requires a value.");
            }

            var value = args[++i];
            apply(value);
        }

        return options;
    }
}
