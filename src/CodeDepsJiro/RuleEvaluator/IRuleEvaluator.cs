using System.Collections.Generic;
using CodeDepsJiro.Models;

namespace CodeDepsJiro.RuleEvaluator;

public interface IRuleEvaluator
{
    IReadOnlyList<RuleViolation> Evaluate(Graph graph, RuleSet ruleSet);
}
