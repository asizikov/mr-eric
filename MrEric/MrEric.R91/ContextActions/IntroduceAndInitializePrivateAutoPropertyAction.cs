using System;
using System.Collections.Generic;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Feature.Services.Intentions;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using MrEric.Common;

namespace MrEric.ContextActions
{
    [ContextAction(Name = "IntroduceAndInitializePrivateAutoPropertyAction", Description = "Create and initialize private auto-property", Group = "C#",
        Priority = 100)]
    public class IntroduceAndInitializePrivateAutoPropertyAction : IntroduceAndInitializePrivateAutopropertyBase, IContextAction
    {
        private ICSharpContextActionDataProvider Provider { get; }

        public IntroduceAndInitializePrivateAutoPropertyAction(ICSharpContextActionDataProvider provider)
        {
            Provider = provider;
        }

        public IEnumerable<IntentionAction> CreateBulbItems() => this.ToContextActionIntentions();

        public override bool IsAvailable(IUserDataHolder cache)
        {
            var hasFix = cache.GetData(IntroduceAndInitializePrivateAutoPropertyFix.InstanceKey) != null;
            return !hasFix && base.IsAvailable(cache);
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            ExecuteInitialization();
            return null;
        }

        protected override IParameterDeclaration FindParameterDeclaration() => Provider.GetSelectedElement<IParameterDeclaration>(true, false);
    }
}