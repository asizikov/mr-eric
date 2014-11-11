using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.CSharp.Bulbs;
using JetBrains.ReSharper.I18n.Services;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Intentions.QuickFixes.UsageChecking;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Naming.Extentions;
using JetBrains.ReSharper.Psi.Naming.Impl;
using JetBrains.ReSharper.Psi.Naming.Settings;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.TextControl;
using JetBrains.Util;
using MrEric.Factories;

namespace MrEric.ContextActions
{
    [ContextAction(Name = "IntroduceAndInitializePrivateAutoPropertyAction",
        Description = "Create and initialize private auto-property", Group = "C#", Priority = 100)]
    public class IntroduceAndInitializePrivateAutoPropertyAction : ContextActionBase
    {
        private ClassMemberFactory ClassMemberFactory { get; set; }
        private ICSharpContextActionDataProvider Provider { get; set; }
        private CSharpLanguage Language { get; set; }
        private IType Type { get; set; }
        private IParameterDeclaration ParameterDeclaration { get; set; }
        private IParameter Parameter { get; set; }
        private IConstructorDeclaration ConstructorDeclaration { get; set; }

        public IntroduceAndInitializePrivateAutoPropertyAction(ICSharpContextActionDataProvider provider)
        {
            Provider = provider;
            Language = CSharpLanguage.Instance;
            ClassMemberFactory = new ClassMemberFactory();
        }


        public override string Text
        {
            get { return string.Format("Create and initialize private auto-property '{0}'.", PropertyName); }
        }

        public override bool IsAvailable(IUserDataHolder cache)
        {
            var parameterDeclaration = FindParameterDeclaration();
            if (parameterDeclaration == null || !parameterDeclaration.Language.IsLanguage(Language))
                return false;
            if (!parameterDeclaration.IsCSharp3Supported())
            {
                return false;
            }
            var declaredElement = parameterDeclaration.DeclaredElement;
            if (declaredElement == null)
                return false;
            var containingNode = parameterDeclaration.GetContainingNode<IFunctionDeclaration>();
            if (containingNode == null)
                return false;
            ConstructorDeclaration = containingNode as IConstructorDeclaration;
            var constructor = containingNode.DeclaredElement as IConstructor;
            if (constructor == null || constructor.IsStatic)
            {
                return false;
            }
            Type = parameterDeclaration.Type;
            ParameterDeclaration = parameterDeclaration;
            Parameter = parameterDeclaration.DeclaredElement;
            return !HasNamingConflicts(constructor.GetContainingType(), PropertyName);
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            var factory = CSharpElementFactory.GetInstance(ParameterDeclaration);
            CreateStatement(factory, Parameter.CreateExpression(Parameter) as ICSharpExpression);

            return null;
        }

        private void CreateStatement(CSharpElementFactory factory, ICSharpExpression expression)
        {
            var statement = (IExpressionStatement) factory.CreateStatement("'__' = expression;");
            var newDeclaration = ClassMemberFactory.CreatePropertyDeclaration(factory, Type, PropertyName);
            var assignment = (IAssignmentExpression) statement.Expression;
            assignment.SetSource(expression);

            var psiServices = expression.GetPsiServices();
            var suggestionManager = psiServices.Naming.Suggestion;
            var classDeclaration = ParameterDeclaration.GetContainingNode<IClassDeclaration>().NotNull();

            var suggestion = suggestionManager.CreateEmptyCollection(
                PluralityKinds.Unknown, expression.Language, true, expression);

            suggestion.Add(expression, new EntryOptions
            {
                SubrootPolicy = SubrootPolicy.Decompose,
                PredefinedPrefixPolicy = PredefinedPrefixPolicy.Remove
            });

            suggestion.Prepare(newDeclaration.DeclaredElement, new SuggestionOptions
            {
                UniqueNameContext = (ITreeNode) classDeclaration.Body ?? classDeclaration
            });

            newDeclaration.SetName(suggestion.FirstName());

            var memberAnchor = GetAnchorMember(classDeclaration.MemberDeclarations.ToList());
            classDeclaration.AddClassMemberDeclarationAfter(newDeclaration, (IClassMemberDeclaration) memberAnchor);

            var languageHelper =
                LanguageManager.Instance.TryGetService<IIntroduceFromParameterLanguageHelper>(
                    Parameter.PresentationLanguage);
            if (languageHelper != null)
                languageHelper.AddAssignmentToBody(ConstructorDeclaration, null, false, Parameter, PropertyName);
        }


        private string PropertyName
        {
            get
            {
                return
                    Parameter.GetPsiServices()
                        .Naming.Suggestion.GetDerivedName(Parameter,
                            NamedElementKinds.MethodPropertyEvent, ScopeKind.Common,
                            Parameter.PresentationLanguage, new SuggestionOptions
                            {
                                DefaultName = "unknown"
                            }, Parameter.GetSourceFiles().FirstOrDefault());
            }
        }


        private static ICSharpTypeMemberDeclaration GetAnchorMember(IList<ICSharpTypeMemberDeclaration> members)
        {
            var anchor = members.LastOrDefault(member =>
                member.DeclaredElement is IProperty && !member.IsStatic &&
                member.GetAccessRights() == AccessRights.PRIVATE) ??
                         members.LastOrDefault(member =>
                             member.DeclaredElement is IField && !member.IsStatic &&
                             member.GetAccessRights() == AccessRights.PRIVATE);
            return anchor;
        }

        private static bool HasNamingConflicts(ITypeElement typeElement, string memberName)
        {
            return typeElement.HasMembers(memberName, true);
        }

        private IParameterDeclaration FindParameterDeclaration()
        {
            return Provider.GetSelectedElement<IParameterDeclaration>(true, false);
        }
    }
}