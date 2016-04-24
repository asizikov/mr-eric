using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace MrEric.Psi
{
    public static class ClassMemberFactory
    {
        public static IClassMemberDeclaration CreatePrivatePropertyDeclaration(this CSharpElementFactory factory,
            IType typeExpression, string memberName)
        {
            var declaration = factory.CreatePropertyDeclaration(typeExpression, memberName);
            declaration.SetAccessRights(AccessRights.PRIVATE);
            var getter = factory.CreateAccessorDeclaration(AccessorKind.GETTER, false);
            var setter = factory.CreateAccessorDeclaration(AccessorKind.SETTER, false);

            declaration.AddAccessorDeclarationAfter(getter, null);
            declaration.AddAccessorDeclarationBefore(setter, null);

            return declaration;
        }
    }
}