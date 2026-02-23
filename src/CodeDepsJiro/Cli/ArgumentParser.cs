using System;
using System.Collections.Generic;

namespace CodeDepsJiro.Cli;

public static class ArgumentParser
{
    /// <summary>
    /// CLI 引数を解析してオプションに変換します。
    /// </summary>
    /// <param name="args">コマンドライン引数。</param>
    /// <returns>解析済みのオプション。</returns>
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
            { "--format", value => options.OutputFormat = ParseFormat(value) },
            { "--output", value => options.OutputPath = value },
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

            if (i + 1 >= args.Length)
            {
                throw new ArgumentException($"Option {arg} requires a value.");
            }

            var value = args[++i];
            apply(value);
        }

        return options;
    }

    /// <summary>
    /// 出力形式の文字列を列挙値に変換します。
    /// </summary>
    /// <param name="value">出力形式（plain/json/csv）。</param>
    /// <returns>出力形式。</returns>
    private static OutputFormat ParseFormat(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Option --format requires a value.");
        }

        return value.Trim().ToLowerInvariant() switch
        {
            "plain" => OutputFormat.Plain,
            "json" => OutputFormat.Json,
            "csv" => OutputFormat.Csv,
            _ => throw new ArgumentException($"Unknown format: {value}"),
        };
    }
}
