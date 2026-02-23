namespace DepGraph.SemanticAnalyzer;

public interface ISemanticAnalyzer
{
    SemanticAnalysisResult Analyze(SyntaxAnalyzer.SyntaxAnalysisResult syntaxResult);
}
