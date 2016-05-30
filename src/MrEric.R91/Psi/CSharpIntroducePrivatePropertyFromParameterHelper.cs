using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace MrEric.Psi
{
    [Language(typeof (CSharpLanguage))]
    public class CSharpIntroducePrivatePropertyFromParameterHelper : IIntroducePrivatePropertyFromParameterLanguageHelper
    {
        public void AddAssignmentToBody(IConstructorDeclaration constructorDeclaration, IStatement anchorStatement,
                                        bool insertBefore, IParameter parameter, ITypeMember member)
        {
            var constructorDeclaration1 = constructorDeclaration;
            var instance = CSharpElementFactory.GetInstance(constructorDeclaration1);
            if (constructorDeclaration1.Body == null)
                constructorDeclaration1.SetBody(instance.CreateEmptyBlock());
            var shortName1 = parameter.ShortName;
            var shortName2 = member.ShortName;
            var statement = instance.CreateStatement("$0 = $1;", (object) shortName2, (object) shortName1);
            CodeStyleUtil.ApplyRecursive(insertBefore
                ? constructorDeclaration1.Body.AddStatementBefore(statement,
                    (ICSharpStatement) anchorStatement)
                : constructorDeclaration1.Body.AddStatementAfter(statement,
                    (ICSharpStatement) anchorStatement));
        }
    }
}