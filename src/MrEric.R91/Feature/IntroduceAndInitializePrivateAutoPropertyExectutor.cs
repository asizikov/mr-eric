using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.I18n.Services;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Naming.Extentions;
using JetBrains.ReSharper.Psi.Naming.Impl;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using MrEric.Psi;

namespace MrEric.Feature
{
    internal sealed class IntroduceAndInitializePrivateAutoPropertyExectutor
    {
        private PrivateAutoPropertyInitializationContext Context { get; }
        private bool IsReadOnly { get; }

        public IntroduceAndInitializePrivateAutoPropertyExectutor(
            [NotNull] PrivateAutoPropertyInitializationContext context, bool isReadOnly)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Context = context;
            IsReadOnly = isReadOnly;
        }

        public void Execute()
        {
            var factory = CSharpElementFactory.GetInstance(Context.ParameterDeclaration);
            CreateStatement(factory, Context.Parameter.CreateExpression(Context.Parameter) as ICSharpExpression,
                Context.SourceFile);
        }

        private void CreateStatement(CSharpElementFactory factory, ICSharpExpression expression,
            IPsiSourceFile sourceFile)
        {
            var statement = (IExpressionStatement) factory.CreateStatement("'__' = expression;");
            var propertyDeclaration = factory.CreatePrivatePropertyDeclaration(Context.Type,
                Context.SuggestedPropertyName, IsReadOnly);
            var assignment = (IAssignmentExpression) statement.Expression;
            assignment.SetSource(expression);
            var psiServices = expression.GetPsiServices();
            var suggestionManager = psiServices.Naming.Suggestion;
            var classDeclaration = Context.ParameterDeclaration.GetContainingNode<IClassDeclaration>().NotNull();

            var suggestion = suggestionManager.CreateEmptyCollection(
                PluralityKinds.Unknown, expression.Language, true, sourceFile);

            suggestion.Add(expression, new EntryOptions
            {
                SubrootPolicy = SubrootPolicy.Decompose,
                PredefinedPrefixPolicy = PredefinedPrefixPolicy.Remove
            });

            suggestion.Prepare(propertyDeclaration.DeclaredElement, new SuggestionOptions
            {
                UniqueNameContext = (ITreeNode) classDeclaration.Body ?? classDeclaration
            });

            propertyDeclaration.SetName(suggestion.FirstName());

            var memberAnchor = GetAnchorMember(classDeclaration.MemberDeclarations.ToList());
            classDeclaration.AddClassMemberDeclarationAfter(propertyDeclaration, (IClassMemberDeclaration) memberAnchor);

            var languageHelper =
                LanguageManager.Instance.TryGetService<IIntroducePrivatePropertyFromParameterLanguageHelper>(
                    Context.Parameter.PresentationLanguage);

            if (languageHelper == null) return;

            var anchorInitializationAnchorMember = GetAnchorInitializationAnchorMember(Context.ConstructorDeclaration);
            languageHelper.AddAssignmentToBody(Context.ConstructorDeclaration, anchorInitializationAnchorMember, false,
                Context.Parameter, propertyDeclaration.DeclaredElement);
        }


        [CanBeNull]
        private static IStatement GetAnchorInitializationAnchorMember(
            [NotNull] ICSharpFunctionDeclaration constructorDeclaration)
        {
            if (constructorDeclaration == null) throw new ArgumentNullException(nameof(constructorDeclaration));
            var statements = constructorDeclaration.Body.Statements;
            if (statements.IsEmpty) return null;

            var expressionStatement =
                statements.LastOrDefault(statement => statement is IExpressionStatement) as IExpressionStatement;
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
    }
}