using System.Text.Json;
using CodeDepsJiro.Models;

namespace CodeDepsJiro.RuleEvaluator;

/// <summary>
/// ルールファイル（JSON）を読み込み、<see cref="RuleSet"/> を構築します。
/// </summary>
public static class RuleSetLoader
{
    /// <summary>
    /// 指定した JSON ファイルを読み込み、<see cref="RuleSet"/> を返します。
    /// </summary>
    /// <param name="path">ルールファイルのパス。</param>
    /// <returns>読み込んだ <see cref="RuleSet"/>。</returns>
    /// <exception cref="ArgumentException">パス不正、未存在、JSON 不正、必須項目欠落時に投げます。</exception>
    public static RuleSet LoadFromFile(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Rules file path is required.");
        }

        if (!File.Exists(path))
        {
            throw new ArgumentException($"Rules file not found: {path}");
        }

        var json = File.ReadAllText(path);
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("Rules file is empty.");
        }

        RuleSetDto? dto;
        try
        {
            dto = JsonSerializer.Deserialize<RuleSetDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        }
        catch (JsonException ex)
        {
            throw new ArgumentException($"Rules file is not valid JSON: {ex.Message}");
        }

        if (dto is null)
        {
            throw new ArgumentException("Rules file is invalid or empty.");
        }

        var layers = new List<LayerRule>();
        if (dto.Layers is not null)
        {
            foreach (var layer in dto.Layers)
            {
                if (layer is null)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(layer.Name))
                {
                    throw new ArgumentException("Each layer must have a non-empty name.");
                }

                layers.Add(new LayerRule
                {
                    Name = layer.Name,
                    Patterns = layer.Patterns ?? new List<string>(),
                });
            }
        }

        var violations = new List<ViolationRule>();
        if (dto.Violations is not null)
        {
            foreach (var violation in dto.Violations)
            {
                if (violation is null)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(violation.From) || string.IsNullOrWhiteSpace(violation.To))
                {
                    throw new ArgumentException("Each violation must have non-empty 'from' and 'to' values.");
                }

                violations.Add(new ViolationRule
                {
                    From = violation.From,
                    To = violation.To,
                });
            }
        }

        return new RuleSet
        {
            Layers = layers,
            Violations = violations,
        };
    }
}
