using System.Text.Json.Serialization;

namespace CodeDepsJiro.RuleEvaluator;

/// <summary>
/// ルールファイル読み込み用の DTO 群です。
/// </summary>
public sealed class RuleSetDto
{
    /// <summary>
    /// ルールファイル内の layers を取得または設定します。
    /// </summary>
    [JsonPropertyName("layers")]
    public List<LayerRuleDto>? Layers { get; set; }

    /// <summary>
    /// ルールファイル内の violations を取得または設定します。
    /// </summary>
    [JsonPropertyName("violations")]
    public List<ViolationRuleDto>? Violations { get; set; }
}

/// <summary>
/// レイヤー定義の DTO です。
/// </summary>
public sealed class LayerRuleDto
{
    /// <summary>
    /// レイヤー名を取得または設定します。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// レイヤーのパターン配列を取得または設定します。
    /// </summary>
    [JsonPropertyName("patterns")]
    public List<string>? Patterns { get; set; }
}

/// <summary>
/// 禁止依存ルールの DTO です。
/// </summary>
public sealed class ViolationRuleDto
{
    /// <summary>
    /// 依存元レイヤー名を取得または設定します。
    /// </summary>
    [JsonPropertyName("from")]
    public string? From { get; set; }

    /// <summary>
    /// 依存先レイヤー名を取得または設定します。
    /// </summary>
    [JsonPropertyName("to")]
    public string? To { get; set; }
}
