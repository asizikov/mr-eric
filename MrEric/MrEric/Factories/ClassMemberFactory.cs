using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace MrEric.Factories
{
    public class ClassMemberFactory
    {
        public IClassMemberDeclaration CreatePropertyDeclaration(CSharpElementFactory factory,
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