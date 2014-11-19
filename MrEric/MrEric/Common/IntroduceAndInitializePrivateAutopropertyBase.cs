using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;
using MrEric.Contexts;

namespace MrEric.Common
{
    public abstract class IntroduceAndInitializePrivateAutopropertyBase : BulbActionBase
    {
        protected PrivateAutoPropertyInitializationContext Context { get; set; }
        private CSharpLanguage Language { get; set; }
        
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
            if (!Context.IsValid) return false;
            return !HasNamingConflicts(Context.Constructor.GetContainingType(), Context.SuggestedPropertyName);
        }

        public override string Text
        {
            get
            {
                return string.Format("Create and initialize private auto-property '{0}'.", Context.SuggestedPropertyName);
            }
        }

        protected abstract IParameterDeclaration FindParameterDeclaration();

        private static bool HasNamingConflicts(ITypeElement typeElement, string memberName)
        {
            return typeElement.HasMembers(memberName, true);
        }

        protected void ExecuteInitialization()
        {
            var executor = new IntroduceAndInitializePrivateAutoPropertyExectutor(Context);
            executor.Execute();
        }
    }
}
