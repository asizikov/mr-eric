using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.CSharp.Bulbs;
using JetBrains.ReSharper.Feature.Services.CSharp.CompleteStatement;
using JetBrains.ReSharper.I18n.Services;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Intentions.QuickFixes.UsageChecking;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Naming.Extentions;
using JetBrains.ReSharper.Psi.Naming.Impl;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.TextControl;
using JetBrains.Util;
using MrEric.Contexts;
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
        private PrivateAutoPropertyInitializationContext Context { get; set; }

        public IntroduceAndInitializePrivateAutoPropertyAction(ICSharpContextActionDataProvider provider)
        {
            Provider = provider;
            Language = CSharpLanguage.Instance;
            ClassMemberFactory = new ClassMemberFactory();
            Context = new PrivateAutoPropertyInitializationContext();
        }


        public override string Text
        {
            get { return string.Format("Create and initialize private auto-property '{0}'.", Context.SuggestedPropertyName); }
        }

        public override bool IsAvailable(IUserDataHolder cache)
        {
            var parameterDeclaration = FindParameterDeclaration();
            if (parameterDeclaration == null || !parameterDeclaration.Language.IsLanguage(Language))
                return false;
            Context.Initialize(parameterDeclaration);
            if (!Context.IsValid) return false;
            return !HasNamingConflicts(Context.Constructor.GetContainingType(), Context.SuggestedPropertyName);
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            var factory = CSharpElementFactory.GetInstance(Context.ParameterDeclaration);
            CreateStatement(factory, Context.Parameter.CreateExpression(Context.Parameter) as ICSharpExpression);
            return null;
        }

        private void CreateStatement(CSharpElementFactory factory, ICSharpExpression expression)
        {
            var statement = (IExpressionStatement) factory.CreateStatement("'__' = expression;");
            var newDeclaration = ClassMemberFactory.CreatePropertyDeclaration(factory, Context.Type, Context.SuggestedPropertyName);
            var assignment = (IAssignmentExpression) statement.Expression;
            assignment.SetSource(expression);

            var psiServices = expression.GetPsiServices();
            var suggestionManager = psiServices.Naming.Suggestion;
            var classDeclaration = Context.ParameterDeclaration.GetContainingNode<IClassDeclaration>().NotNull();

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
                    Context.Parameter.PresentationLanguage);
            if (languageHelper != null)
            {
                var anchorInitializationAnchorMember = GetAnchorInitializationAnchorMember(Context.ConstructorDeclaration);
                languageHelper.AddAssignmentToBody(Context.ConstructorDeclaration, anchorInitializationAnchorMember, false, Context.Parameter, Context.SuggestedPropertyName);
            }
        }

        [CanBeNull]
        private static IStatement GetAnchorInitializationAnchorMember(
            [NotNull] IConstructorDeclaration constructorDeclaration)
        {
            if (constructorDeclaration == null) throw new ArgumentNullException("constructorDeclaration");
            var statements = constructorDeclaration.Body.Statements;
            if (statements.IsEmpty) return null;

            var expressionStatement = statements.LastOrDefault(statement => statement is IExpressionStatement) as IExpressionStatement;
            if (expressionStatement != null) return expressionStatement;

            var ifStatement = statements.LastOrDefault(statement => statement is IIfStatement);
            return ifStatement ?? statements.LastOrDefault();
        }

        [CanBeNull]
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