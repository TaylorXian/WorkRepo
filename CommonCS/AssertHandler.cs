using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Forms;

namespace CommonCS
{
	/// <summary>
	/// Summary description for AssertHandler.
	/// </summary>
    /// <summary>
    /// Summary description for AssertHandler.
    /// </summary>
    public class Assert : DefaultTraceListener
    {
        public Assert()
        {}

        public override void Fail(string message)
        {
            Fail(message,null);
        }

        public override void Fail(string message1, string message2)
        {
            StackTrace stack = new StackTrace();

            Environment.GetEnvironmentVariables();
            string sDisplay = message1 + "\n" + message2 + "\n" + "Launch Debugger?\n\n" + stack.ToString();

            DialogResult dr = MessageBox.Show(sDisplay, "Assertion failed.",MessageBoxButtons.YesNo,MessageBoxIcon.Error ,MessageBoxDefaultButton.Button1,MessageBoxOptions.DefaultDesktopOnly);
            if (dr == DialogResult.Yes)
            {
                Debugger.Break();
            }
        }
    }
}
