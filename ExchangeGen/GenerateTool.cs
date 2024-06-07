using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.IO;
using System.Text;

namespace ExchangeGen {

    public interface MetaProperty {
        Type PropertyType { get; }
        string PropertyName { get; }
    }

    public enum ParameterModifyType {
        None = 1,
        In = 2,
        Out = 3,
        Ref = 4,
    }
    
    public interface MetaMethodParameter {
        Type PropertyType { get; }
        string PropertyName { get; }
        ParameterModifyType ModifyType { get; }
    }
    
    public interface MetaMethod {
        SyntaxKind MethodModify { get; }
        Type RetType { get; }
        string MethodName { get; }
        MetaMethodParameter[] Paramters { get; }
    }
    
    public interface MetaClass {
        string ClassName { get; }
        string Namespcae { get; }
        MetaProperty[] Property { get; }
    }
    
    public class LuaStackTool {

        public string CreateClass(MetaClass metaClass) {
            
            // 创建类
            ClassDeclarationSyntax classDeclarationSyntax = CreateClassDeclaration(metaClass);
            
            // 创建名空间
            NamespaceDeclarationSyntax namespaceDeclarationSyntax = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(metaClass.Namespcae))
                .AddMembers(classDeclarationSyntax);

            // 创建编译单元
            CompilationUnitSyntax compilationUnitSyntax = SyntaxFactory.CompilationUnit()
                .AddMembers(namespaceDeclarationSyntax);

            // 字符串代码
            string formattedCode = FormatCode(compilationUnitSyntax);
            
            // 代码写入文件
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), $"{metaClass.ClassName}.cs");
            File.WriteAllText(filePath, formattedCode);
        }
        string FormatCode(CompilationUnitSyntax compilationUnitSyntax) {
            StringBuilder builder = new StringBuilder();
            using (StringWriter writer = new StringWriter(builder)) {
                compilationUnitSyntax.WriteTo(writer);
            }
            return builder.ToString();
        }

        ClassDeclarationSyntax CreateClassDeclaration(MetaClass metaClass) {
            // 类声明
            ClassDeclarationSyntax classDeclarationSyntax = SyntaxFactory.ClassDeclaration(metaClass.ClassName).
                AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            foreach (MetaProperty metaProperty in metaClass.Property) {
                PropertyDeclarationSyntax propertyDeclarationSyntax = SyntaxFactory.PropertyDeclaration(SyntaxFactory.IdentifierName(metaProperty.PropertyType.Name), SyntaxFactory.Identifier(metaProperty.PropertyName))
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                    .AddAccessorListAccessors(
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

                classDeclarationSyntax.AddMembers(propertyDeclarationSyntax);

            }
            
        }
    }
}
