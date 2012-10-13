// Type: Microsoft.VisualStudio.PlatformUI.DialogWindow
// Assembly: Microsoft.VisualStudio.Shell.11.0, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 11.0\VSSDK\VisualStudioIntegration\Common\Assemblies\v4.0\Microsoft.VisualStudio.Shell.11.0.dll

using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.VSHelp;
using System;
using System.Windows;

namespace Microsoft.VisualStudio.PlatformUI
{
  /// <summary>
  /// The base class for all Visual Studio WPF (non-Gel) dialogs. When you implement a WPF dialog, you should derive from this class in order to have consistent styling with other Visual Studio dialogs, as well as help support. To display the dialog, call the <see cref="M:Microsoft.VisualStudio.PlatformUI.DialogWindow.ShowModal"/> method, which correctly parents the dialog in the shell, puts the shell in a modal state while the dialog is displayed, and other features.
  /// </summary>
  public class DialogWindow : DialogWindowBase
  {
    private string helpTopic;

    /// <summary>
    /// Initializes a <see cref="T:Microsoft.VisualStudio.PlatformUI.DialogWindow"/> without a Help button.
    /// </summary>
    public DialogWindow()
    {
    }

    /// <summary>
    /// Initializes a T:Microsoft.VisualStudio.PlatformUI.DialogWindow that has a Help topic and a button.
    /// </summary>
    /// <param name="helpTopic">The dialog's help topic</param>
    public DialogWindow(string helpTopic)
    {
      if (helpTopic == null)
        throw new ArgumentNullException("helpTopic");
      this.helpTopic = helpTopic;
      this.HasHelpButton = true;
    }

    /// <summary>
    /// Invokes the Help for the dialog window.
    /// </summary>
    protected override void InvokeDialogHelp()
    {
      if (this.helpTopic == null || !this.HasHelpButton)
        return;
      Help help = ServiceProvider.GlobalProvider.GetService(typeof (Help)) as Help;
      if (help == null)
        return;
      help.DisplayTopicFromF1Keyword(this.helpTopic);
    }

    /// <summary>
    /// Gets the parent or owner of the dialog from the Visual Studio shell and displays the dialog window. It also puts the shell in a modal state while the dialog is displayed, and centers the dialog window correctly in the parent window.
    /// </summary>
    /// 
    /// <returns>
    /// T:System.Nullable`1
    /// </returns>
    public bool? ShowModal()
    {
      int num = WindowHelper.ShowModal((Window) this);
      if (num == 0)
        return new bool?();
      else
        return new bool?(num == 1);
    }
  }
}
