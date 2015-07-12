using System;
using System.Collections.Generic;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;

using JetBrains.ReSharper.Feature.Services.CSharp.Bulbs;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Intentions.Extensibility.Menu;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using MrEric.Common;
using MrEric.QuickFixes;

namespace MrEric.ContextActions
{
    [ContextAction(Name = "IntroduceAndInitializePrivateAutoPropertyAction",
        Description = "Create and initialize private auto-property", Group = "C#", Priority = 100)]
    public class IntroduceAndInitializePrivateAutoPropertyAction : IntroduceAndInitializePrivateAutopropertyBase, IContextAction
    {
        private ICSharpContextActionDataProvider Provider { get; set; }

        public IntroduceAndInitializePrivateAutoPropertyAction(ICSharpContextActionDataProvider provider)
        {
            Provider = provider;
        }

        public IEnumerable<IntentionAction> CreateBulbItems()
        {
            return this.ToContextAction();
        }

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

        protected override IParameterDeclaration FindParameterDeclaration()
        {
            return Provider.GetSelectedElement<IParameterDeclaration>(true, false);
        }
    }
}