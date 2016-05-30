using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.UsageChecking;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.Intentions;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.TextControl;
using JetBrains.Util;

namespace MrEric.Feature.Intentions
{
    [QuickFix]
    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public sealed class IntroduceAndInitializePrivateAutoPropertyFix : IQuickFix
    {
        private PrivateAutoPropertyInitializationContext Context { get; }
        private CSharpLanguage Language { get; }

        private IntroduceAndInitializePrivateAutoPropertyFix(IParameterDeclaration parameterDeclaration)
        {
            Context = new PrivateAutoPropertyInitializationContext();
            Language = CSharpLanguage.Instance;
            Context.Initialize(parameterDeclaration);
        }

        public IntroduceAndInitializePrivateAutoPropertyFix(UnusedParameterLocalWarning warning) : this(warning.Declaration)
        {
        }

        public IntroduceAndInitializePrivateAutoPropertyFix(NotAccessedParameterLocalWarning error) : this(error.Declaration)
        {
        }

        public IEnumerable<IntentionAction> CreateBulbItems() => CreateItems().ToQuickFixIntentions();

        public bool IsAvailable(IUserDataHolder cache)
        {
            var parameterDeclaration = FindParameterDeclaration();
            if (parameterDeclaration == null || !parameterDeclaration.Language.IsLanguage(Language))
                return false;
            Context.Initialize(parameterDeclaration);
            if (!Context.IsValid)
                return false;
            return !HasNamingConflicts(Context.Constructor.GetContainingType(), Context.SuggestedPropertyName);
        }

        private IParameterDeclaration FindParameterDeclaration() => Context.ParameterDeclaration;
        private static bool HasNamingConflicts(ITypeElement typeElement, string memberName) => typeElement.HasMembers(memberName, true);
        [NotNull]
        private IEnumerable<IBulbAction> CreateItems()
            =>
                !Context.ParameterDeclaration.IsCSharp6Supported()
                    ? new IBulbAction[] { new Private(Context) }
                    : new IBulbAction[] { new PrivateReadOnly(Context), new Private(Context) };

        private sealed class Private : BulbActionBase
        {
            public override string Text => $"Create and initialize private auto-property '{Context.SuggestedPropertyName}'";

            private PrivateAutoPropertyInitializationContext Context { get; }

            public Private(PrivateAutoPropertyInitializationContext context)
            {
                Context = context;
            }

            protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
            {
                var executor = new IntroduceAndInitializePrivateAutoPropertyExectutor(Context, false);
                executor.Execute();
                return null;
            }
        }

        private sealed class PrivateReadOnly : BulbActionBase
        {
            public override string Text => $"Create and initialize private readonly auto-property '{Context.SuggestedPropertyName}'";
            private PrivateAutoPropertyInitializationContext Context { get; }

            public PrivateReadOnly(PrivateAutoPropertyInitializationContext context)
            {
                Context = context;
            }

            protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
            {
                var executor = new IntroduceAndInitializePrivateAutoPropertyExectutor(Context, true);
                executor.Execute();
                return null;
            }
        }
    }
}