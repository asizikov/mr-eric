using System;
using System.Collections.Generic;
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
    public class IntroduceAndInitializePrivateReadOnlyAutoPropertyFix : IntroduceAndInitializePrivateAutopropertyBase,
        IQuickFix
    {
        public static readonly Key InstanceKey = new Key("IntroduceAndInitializePrivateReadOnlyAutoPropertyFix");

        private IntroduceAndInitializePrivateReadOnlyAutoPropertyFix(IParameterDeclaration parameterDeclaration)
        {
            Context.Initialize(parameterDeclaration);
        }

        public IntroduceAndInitializePrivateReadOnlyAutoPropertyFix(UnusedParameterLocalWarning warning)
            : this(warning.Declaration)
        {
        }

        public IntroduceAndInitializePrivateReadOnlyAutoPropertyFix(NotAccessedParameterLocalWarning error)
            : this(error.Declaration)
        {
        }

        public override string Text => $"Create and initialize private readonly auto-property '{Context.SuggestedPropertyName}'.";

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            ExecuteInitialization(true);
            return null;
        }

        public IEnumerable<IntentionAction> CreateBulbItems()
        {
            var anchor = BulbMenuAnchorPositions.FirstClassContextItems;

            return this.ToQuickFixIntentions(anchor);
        }

        public override bool IsAvailable(IUserDataHolder cache)
        {
            var isAvailable = base.IsAvailable(cache);
            if (!Context.ParameterDeclaration.IsCSharp6Supported())
            {
                isAvailable = false;
            }
            if (isAvailable) cache.PutData(InstanceKey, this);
            return isAvailable;
        }

        protected override IParameterDeclaration FindParameterDeclaration() => Context.ParameterDeclaration;
    }
}