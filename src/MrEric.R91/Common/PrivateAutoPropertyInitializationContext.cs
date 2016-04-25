using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using MrEric.Psi;

namespace MrEric.Common
{
    public class PrivateAutoPropertyInitializationContext
    {
        public IType Type { get; private set; }

        public IParameterDeclaration ParameterDeclaration { get; private set; }

        public IConstructorDeclaration ConstructorDeclaration { get; private set; }
        public IConstructor Constructor { get; private set; }
        public IParameter Parameter { get; private set; }

        public bool IsValid { get; private set; }

        public string SuggestedPropertyName => Parameter.GetSuggestedName();

        public IPsiSourceFile SourceFile { get; set; }

        public void Initialize(IParameterDeclaration parameterDeclaration) => IsValid = InitializeInternal(parameterDeclaration);

        private bool InitializeInternal(IParameterDeclaration parameterDeclaration)
        {
            if (!parameterDeclaration.IsCSharp3Supported())
            {
                return false;
            }
            var declaredElement = parameterDeclaration.DeclaredElement;
            if (declaredElement == null)
                return false;
            var containingNode = parameterDeclaration.GetContainingNode<IFunctionDeclaration>();
            if (containingNode == null)
                return false;
            ConstructorDeclaration = containingNode as IConstructorDeclaration;
            var constructor = containingNode.DeclaredElement as IConstructor;
            if (constructor == null || constructor.IsStatic)
            {
                return false;
            }
            Constructor = constructor;
            Type = parameterDeclaration.Type;
            ParameterDeclaration = parameterDeclaration;
            Parameter = parameterDeclaration.DeclaredElement;
            SourceFile = parameterDeclaration.GetSourceFile();
            return true;
        }
    }
}