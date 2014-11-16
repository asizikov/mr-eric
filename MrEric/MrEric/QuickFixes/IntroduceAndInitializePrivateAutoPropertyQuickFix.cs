using System;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.UsageChecking;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using MrEric.Contexts;

namespace MrEric.QuickFixes
{
    [QuickFix]
    public class IntroduceAndInitializePrivateAutoPropertyQuickFix : QuickFixBase
    {
        private PrivateAutoPropertyInitializationContext Context { get; set; }

        private IntroduceAndInitializePrivateAutoPropertyQuickFix(IParameterDeclaration parameterDeclaration)
        {
            Context = new PrivateAutoPropertyInitializationContext();
            Context.Initialize(parameterDeclaration);
        }

        public IntroduceAndInitializePrivateAutoPropertyQuickFix(UnusedParameterWarningBase warning)
            : this(warning.Declaration)
        {
        }

        public IntroduceAndInitializePrivateAutoPropertyQuickFix(NotAccessedParameterWarningBase error)
            : this(error.Declaration)
        {
        }


        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            throw new NotImplementedException();
        }

        public override string Text
        {
            get
            {
                return string.Format("Create and initialize private auto-property '{0}'.", Context.SuggestedPropertyName);
            }
        }

        public override bool IsAvailable(IUserDataHolder cache)
        {
            return Context.IsValid;
        }
    }
}