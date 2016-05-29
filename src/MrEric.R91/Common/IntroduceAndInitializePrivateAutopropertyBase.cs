using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;

namespace MrEric.Common
{
    public abstract class IntroduceAndInitializePrivateAutopropertyBase : BulbActionBase
    {
        protected PrivateAutoPropertyInitializationContext Context { get; }
        private CSharpLanguage Language { get; }

        protected IntroduceAndInitializePrivateAutopropertyBase()
        {
            Context = new PrivateAutoPropertyInitializationContext();
            Language = CSharpLanguage.Instance;
        }

        public virtual bool IsAvailable(IUserDataHolder cache)
        {
            var parameterDeclaration = FindParameterDeclaration();
            if (parameterDeclaration == null || !parameterDeclaration.Language.IsLanguage(Language))
                return false;
            Context.Initialize(parameterDeclaration);
            if (!Context.IsValid)
                return false;
            return !HasNamingConflicts(Context.Constructor.GetContainingType(), Context.SuggestedPropertyName);
        }

        public override string Text => $"Create and initialize private auto-property '{Context.SuggestedPropertyName}'.";

        protected abstract IParameterDeclaration FindParameterDeclaration();

        private static bool HasNamingConflicts(ITypeElement typeElement, string memberName) => typeElement.HasMembers(memberName, true);

        protected void ExecuteInitialization(bool isReadOnly)
        {
            var executor = new IntroduceAndInitializePrivateAutoPropertyExectutor(Context, isReadOnly);
            executor.Execute();
        }
    }
}