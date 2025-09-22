using System;
using System.Drawing;
using System.Windows.Forms;
using ModShardLauncher.Core.Models;
using Serilog;

namespace ModShardLauncher.Core.Errors
{
    public class MSLDiagnostic
    {
        public string ModName;
        public string? FileName;
        public string? PatchingMethod;
        public Exception exception;
        public MSLDiagnostic(Exception ex, ModFile modFile)
        {
            if (ex.Data.Contains("fileName"))
            {
                object? fileName = ex.Data["fileName"];
                FileName = $"{fileName}";
            }
            if (ex.Data.Contains("patchingWay"))
            {
                object? patchingWay = ex.Data["patchingWay"];
                PatchingMethod = $"{patchingWay}";
            }
            ModName = modFile.Name;
            exception = ex;
        }
        public void ToLog()
        {
            string extraInformation = " for {{{0}}}";
            if (FileName is not null)
            {
                extraInformation += " in file {{{1}}}";
            }
            if (PatchingMethod is not null)
            {
                extraInformation += " while patching by {{{2}}}";
            }
            Log.Error(exception, "Something went wrong" + extraInformation, ModName, FileName, PatchingMethod);
        }
        public string Title()
        {
            return $"Patching {ModName} failed";
        }
        public string MessageDialog()
        {
            RichTextBox message = new();

            message.SelectionColor = Color.Black;
            message.AppendText("An error was encountered while patching the mod ");
            message.SelectionColor = Color.Blue;
            message.AppendText(ModName);
            message.SelectionColor = Color.Black;
            message.AppendText(":\n");
            
            if (FileName is not null)
            {
                message.SelectionColor = Color.Black;
                message.AppendText("In file ");
                message.SelectionColor = Color.Blue;
                message.AppendText(FileName);
            }
            if (PatchingMethod is not null)
            {
                message.SelectionColor = Color.Black;
                message.AppendText(" by ");
                message.SelectionColor = Color.Blue;
                message.AppendText(PatchingMethod);
                message.SelectionColor = Color.Black;
                message.AppendText(":\n");
            }

            message.SelectionColor = Color.Black;
            message.AppendText("\n");
            message.SelectionFont = new Font(message.Font, FontStyle.Bold);
            message.AppendText(exception.ToString());
            return message.Rtf;
        }
    }
}