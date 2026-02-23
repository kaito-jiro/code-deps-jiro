namespace CodeDepsJiro.SemanticAnalyzer;

public interface ISemanticAnalyzer
{
    SemanticAnalysisResult Analyze(SyntaxAnalyzer.SyntaxAnalysisResult syntaxResult);
}
