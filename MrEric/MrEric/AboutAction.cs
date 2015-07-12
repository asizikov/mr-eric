#if RESHARPER8
using System.Windows.Forms;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
#endif

namespace MrEric
{
    [ActionHandler("MrEric.About")]
    public class AboutAction : IActionHandler
    {
        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            // return true or false to enable/disable this action
            return false;
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            MessageBox.Show(
              "Mr. Eric\nAnton Sizikov\n\nCreate and initialize private auto-property action.",
              "About Mr. Eric",
              MessageBoxButtons.OK,
              MessageBoxIcon.Information);
        }
    }
}