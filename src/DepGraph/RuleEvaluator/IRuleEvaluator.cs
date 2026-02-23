using System.Collections.Generic;
using DepGraph.Models;

namespace DepGraph.RuleEvaluator;

public interface IRuleEvaluator
{
    IReadOnlyList<RuleViolation> Evaluate(Graph graph, RuleSet ruleSet);
}
