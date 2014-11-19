using System;
using System.Collections.Generic;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.UsageChecking;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Intentions.Extensibility.Menu;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.UI.BulbMenu;
using JetBrains.Util;
using MrEric.Common;

namespace MrEric.QuickFixes
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
            return this.ToQuickFixAction(anchor);
        }

        public override bool IsAvailable(IUserDataHolder cache)
        {
            var isAvailable = base.IsAvailable(cache);
            if (isAvailable) cache.PutData(InstanceKey, this);
            return isAvailable;
        }

        protected override IParameterDeclaration FindParameterDeclaration()
        {
            return Context.ParameterDeclaration;
        }
    }
}