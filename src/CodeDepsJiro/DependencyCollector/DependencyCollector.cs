using CodeDepsJiro.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeDepsJiro.DependencyCollector;

public sealed class DependencyCollector : IDependencyCollector
{
    /// <summary>
    /// セマンティック解析結果から依存関係を抽出する。
    /// </summary>
    /// <param name="semanticResult">セマンティック解析結果。</param>
    /// <returns>依存関係の一覧。</returns>
    public IReadOnlyList<DependencyEdge> Collect(SemanticAnalyzer.SemanticAnalysisResult semanticResult)
    {
        if (semanticResult == null)
        {
            throw new ArgumentNullException(nameof(semanticResult));
        }

        var edges = new List<DependencyEdge>();
        foreach (var tree in semanticResult.SyntaxTrees)
        {
            var model = semanticResult.Compilation.GetSemanticModel(tree);
            var root = tree.GetRoot();
            var typeDecls = root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>();

            foreach (var typeDecl in typeDecls)
            {
                var fromSymbol = model.GetDeclaredSymbol(typeDecl) as INamedTypeSymbol;
                if (fromSymbol == null)
                {
                    continue;
                }

                var fromNode = CreateNode(fromSymbol);

                CollectBaseTypeDependencies(fromSymbol, fromNode, edges);
                CollectMemberTypeDependencies(typeDecl, model, fromNode, edges);
            }
        }

        return edges;
    }

    /// <summary>
    /// 継承/実装の依存を収集する。
    /// </summary>
    /// <param name="fromSymbol">依存元の型シンボル。</param>
    /// <param name="fromNode">依存元ノード。</param>
    /// <param name="edges">依存関係の蓄積先。</param>
    private static void CollectBaseTypeDependencies(
        INamedTypeSymbol fromSymbol,
        Node fromNode,
        List<DependencyEdge> edges)
    {
        if (fromSymbol.BaseType != null && fromSymbol.BaseType.SpecialType != SpecialType.System_Object)
        {
            edges.Add(CreateEdge(fromNode, fromSymbol.BaseType, RelationType.Inherits));
        }

        foreach (var iface in fromSymbol.Interfaces)
        {
            edges.Add(CreateEdge(fromNode, iface, RelationType.Implements));
        }
    }

    /// <summary>
    /// フィールド/プロパティ/メソッド/生成式の依存を収集する。
    /// </summary>
    /// <param name="typeDecl">型宣言ノード。</param>
    /// <param name="model">セマンティックモデル。</param>
    /// <param name="fromNode">依存元ノード。</param>
    /// <param name="edges">依存関係の蓄積先。</param>
    private static void CollectMemberTypeDependencies(
        BaseTypeDeclarationSyntax typeDecl,
        SemanticModel model,
        Node fromNode,
        List<DependencyEdge> edges)
    {
        if (typeDecl is not TypeDeclarationSyntax typeWithMembers)
        {
            return;
        }

        foreach (var field in typeWithMembers.Members.OfType<FieldDeclarationSyntax>())
        {
            var type = model.GetTypeInfo(field.Declaration.Type).Type;
            AddEdgeIfType(fromNode, type, RelationType.Field, edges);
        }

        foreach (var prop in typeWithMembers.Members.OfType<PropertyDeclarationSyntax>())
        {
            var type = model.GetTypeInfo(prop.Type).Type;
            AddEdgeIfType(fromNode, type, RelationType.Property, edges);
        }

        foreach (var method in typeWithMembers.Members.OfType<MethodDeclarationSyntax>())
        {
            var returnType = model.GetTypeInfo(method.ReturnType).Type;
            AddEdgeIfType(fromNode, returnType, RelationType.Return, edges);

            foreach (var param in method.ParameterList.Parameters)
            {
                var paramType = model.GetTypeInfo(param.Type!).Type;
                AddEdgeIfType(fromNode, paramType, RelationType.Parameter, edges);
            }
        }

        foreach (var creation in typeDecl.DescendantNodes().OfType<ObjectCreationExpressionSyntax>())
        {
            var type = model.GetTypeInfo(creation).Type;
            AddEdgeIfType(fromNode, type, RelationType.New, edges);
        }
    }

    /// <summary>
    /// 型情報が存在する場合に依存エッジを追加する。
    /// </summary>
    /// <param name="fromNode">依存元ノード。</param>
    /// <param name="type">参照型。</param>
    /// <param name="relation">依存種別。</param>
    /// <param name="edges">依存関係の蓄積先。</param>
    private static void AddEdgeIfType(Node fromNode, ITypeSymbol? type, RelationType relation, List<DependencyEdge> edges)
    {
        if (type == null)
        {
            return;
        }

        if (type is INamedTypeSymbol named)
        {
            edges.Add(CreateEdge(fromNode, named, relation));
        }
        else if (type is IArrayTypeSymbol arrayType)
        {
            if (arrayType.ElementType is INamedTypeSymbol elementNamed)
            {
                edges.Add(CreateEdge(fromNode, elementNamed, relation));
            }
        }
    }

    /// <summary>
    /// 依存エッジを生成する。
    /// </summary>
    /// <param name="fromNode">依存元ノード。</param>
    /// <param name="toSymbol">依存先の型シンボル。</param>
    /// <param name="relation">依存種別。</param>
    /// <returns>依存エッジ。</returns>
    private static DependencyEdge CreateEdge(Node fromNode, INamedTypeSymbol toSymbol, RelationType relation)
    {
        var toNode = CreateNode(toSymbol);
        return new DependencyEdge
        {
            From = fromNode,
            To = toNode,
            RelationType = relation,
        };
    }

    /// <summary>
    /// 型シンボルからノードを生成する。
    /// </summary>
    /// <param name="symbol">型シンボル。</param>
    /// <returns>ノード。</returns>
    private static Node CreateNode(INamedTypeSymbol symbol)
    {
        return new Node
        {
            Id = symbol.ToDisplayString(),
            Name = symbol.Name,
            Namespace = symbol.ContainingNamespace?.ToDisplayString() ?? string.Empty,
            Kind = ToNodeKind(symbol),
        };
    }

    /// <summary>
    /// 型シンボルの種別をノード種別に変換する。
    /// </summary>
    /// <param name="symbol">型シンボル。</param>
    /// <returns>ノード種別。</returns>
    private static NodeKind ToNodeKind(INamedTypeSymbol symbol)
    {
        if (symbol.TypeKind == TypeKind.Interface)
        {
            return NodeKind.Interface;
        }

        if (symbol.IsAbstract && symbol.TypeKind == TypeKind.Class)
        {
            return NodeKind.Abstract;
        }

        return NodeKind.Class;
    }
}
