using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace MrEric.Psi
{
    public interface IIntroducePrivatePropertyFromParameterLanguageHelper
    {
        void AddAssignmentToBody(IConstructorDeclaration constructorDeclaration, IStatement anchorStatement,
                                 bool insertBefore, IParameter parameter, ITypeMember member);
    }
}