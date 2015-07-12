using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Naming.Extentions;
using JetBrains.ReSharper.Psi.Naming.Impl;
using JetBrains.ReSharper.Psi.Naming.Settings;

namespace MrEric.Psi
{
    public static class ParameterExt
    {
        public static string GetSuggestedName(this IParameter parameter)
        {
            if (parameter == null) return string.Empty;

            return parameter.GetPsiServices()
                .Naming.Suggestion.GetDerivedName(parameter,
                    NamedElementKinds.MethodPropertyEvent, ScopeKind.Common,
                    parameter.PresentationLanguage, new SuggestionOptions
                    {
                        DefaultName = "unknown"
                    }, parameter.GetSourceFiles().FirstOrDefault());
        }
    }
}