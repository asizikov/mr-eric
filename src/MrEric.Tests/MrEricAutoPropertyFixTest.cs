using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl;
using MrEric.Feature.Intentions;
using NUnit.Framework;

namespace MrEric.Tests
{
    public class MrEricAutoPropertyFixTest : CSharpQuickFixTestBase<IntroduceAndInitializePrivateAutoPropertyFix>
    {
        protected override string RelativeTestDataPath => "Feature";


        [Test]
        public void TestPrivateProperty01() { DoNamedTest2(); }

        protected override void DoTest(IProject project)
        {
            var languageLevelProjectProperty = project.GetComponent<CSharpLanguageLevelProjectProperty>();
            languageLevelProjectProperty.ExecuteWithLanguageLevel(project, CSharpLanguageLevel.CSharp60, () =>
            {
                base.DoTest(project);
            });
        }
    }
}
