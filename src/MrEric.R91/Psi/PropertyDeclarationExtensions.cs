using System;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace MrEric.Psi
{
    public static class PropertyDeclarationExtensions
    {
        public static IPropertyDeclaration WithPrivateGetter([NotNull] this IPropertyDeclaration declaration,
            CSharpElementFactory factory)
        {
            if (declaration == null) throw new ArgumentNullException(nameof(declaration));
            var getter = factory.CreateAccessorDeclaration(AccessorKind.GETTER, false);
            declaration.AddAccessorDeclarationAfter(getter, null);
            return declaration;
        }

        public static IPropertyDeclaration WithPrivateSetter([NotNull] this IPropertyDeclaration declaration,
            CSharpElementFactory factory)
        {
            if (declaration == null) throw new ArgumentNullException(nameof(declaration));
            var setter = factory.CreateAccessorDeclaration(AccessorKind.SETTER, false);
            declaration.AddAccessorDeclarationBefore(setter, null);
            return declaration;
        }
    }
}