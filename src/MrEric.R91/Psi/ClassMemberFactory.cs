using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace MrEric.Psi
{
    public static class ClassMemberFactory
    {
        public static IClassMemberDeclaration CreatePropertyDeclaration(this CSharpElementFactory factory,
         IType typeExpression, string memberName, bool isReadOnly, AccessRights accessRights)
        {
            var declaration = factory.CreatePropertyDeclaration(typeExpression, memberName);
            declaration.SetAccessRights(accessRights);
            declaration.WithPrivateGetter(factory);
            if (!isReadOnly)
            {
                declaration.WithPrivateSetter(factory);
            }
            return declaration;
        }

    }
}