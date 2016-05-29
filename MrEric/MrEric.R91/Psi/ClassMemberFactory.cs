using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace MrEric.Psi
{
    public static class ClassMemberFactory
    {
        public static IClassMemberDeclaration CreatePrivatePropertyDeclaration(this CSharpElementFactory factory,
            IType typeExpression, string memberName, bool isReadOnly)
        {
            var declaration = factory.CreatePropertyDeclaration(typeExpression, memberName);
            declaration.SetAccessRights(AccessRights.PRIVATE);
            declaration.WithPrivateGetter(factory);
            if (!isReadOnly)
            {
                declaration.WithPrivateSetter(factory);
            }
            return declaration;
        }
    }
}