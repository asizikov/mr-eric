using System;
using System.Collections.Generic;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.UsageChecking;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.Intentions;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.UI.BulbMenu;
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
            ExecuteInitialization();
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
            if (isAvailable) cache.PutData(InstanceKey, this);
            return isAvailable;
        }

        protected override IParameterDeclaration FindParameterDeclaration() => Context.ParameterDeclaration;
    }
    
    public static class BulbMenuAnchorPositions
    {
        public static readonly IAnchorPosition PermanentTopForIconPosition = AnchorPosition.BasePosition;
        public static readonly IAnchorPosition FirstClassContextItemsPosition = BulbMenuAnchorPositions.PermanentTopForIconPosition.GetNext();
        public static readonly IAnchor FirstClassContextItems = new InvisibleAnchor(RootAnchor.Instance, true);
    }
}