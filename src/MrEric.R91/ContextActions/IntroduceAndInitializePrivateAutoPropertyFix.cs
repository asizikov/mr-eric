using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.UsageChecking;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.Intentions;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using MrEric.Common;

namespace MrEric.ContextActions
{
    [QuickFix]
    public class IntroduceAndInitializePrivateAutoPropertyFix : IntroduceAndInitializePrivateAutopropertyBase,
        IQuickFix
    {
        public static readonly Key InstanceKey = new Key("IntroduceAndInitializePrivateAutoPropertyFix");

        private IntroduceAndInitializePrivateAutoPropertyFix(IParameterDeclaration parameterDeclaration)
        {
            Context.Initialize(parameterDeclaration);
        }

        public IntroduceAndInitializePrivateAutoPropertyFix(UnusedParameterLocalWarning warning)
            : this(warning.Declaration)
        {
        }

        public IntroduceAndInitializePrivateAutoPropertyFix(NotAccessedParameterLocalWarning error)
            : this(error.Declaration)
        {
        }


        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            ExecuteInitialization(false);
            return null;
        }

        [NotNull]
        private IBulbAction[] CreateItems()
        {
            if (!Context.ParameterDeclaration.IsCSharp6Supported())
            {
                return new IBulbAction[]
                {
                    new Private(Context)
                };
            }
            return new IBulbAction[]
            {
                new PrivateReadOnly(Context),
                new Private(Context)
            };
        }

        public IEnumerable<IntentionAction> CreateBulbItems()
        {
            return CreateItems().ToQuickFixIntentions();
        }

        public override bool IsAvailable(IUserDataHolder cache)
        {
            var isAvailable = base.IsAvailable(cache);
            if (isAvailable) cache.PutData(InstanceKey, this);
            return isAvailable;
        }

        protected override IParameterDeclaration FindParameterDeclaration() => Context.ParameterDeclaration;

        private class Private : BulbActionBase
        {
            private PrivateAutoPropertyInitializationContext Context { get; }

            public Private(PrivateAutoPropertyInitializationContext context)
            {
                Context = context;
            }

            protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution,
                IProgressIndicator progress)
            {
                var executor = new IntroduceAndInitializePrivateAutoPropertyExectutor(Context, false);
                executor.Execute();
                return null;
            }

            public override string Text
                => $"Create and initialize private auto-property '{Context.SuggestedPropertyName}'.";
        }

        private class PrivateReadOnly : BulbActionBase
        {
            private PrivateAutoPropertyInitializationContext Context { get; }

            public PrivateReadOnly(PrivateAutoPropertyInitializationContext context)
            {
                Context = context;
            }

            protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution,
                IProgressIndicator progress)
            {
                var executor = new IntroduceAndInitializePrivateAutoPropertyExectutor(Context, true);
                executor.Execute();
                return null;
            }

            public override string Text
                => $"Create and initialize private readonly auto-property '{Context.SuggestedPropertyName}'.";
        }
    }
}