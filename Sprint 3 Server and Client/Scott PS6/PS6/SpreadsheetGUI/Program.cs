using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    /// <summary>
    /// Keeps track of how many top-level forms are running
    /// </summary>
    class NewApplicationContext : ApplicationContext
    {
        // Number of open forms
        private int formCount = 0;

        // Singleton ApplicationContext
        private static NewApplicationContext appContext;

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        private NewApplicationContext()
        {
        }

        /// <summary>
        /// Returns the one NewApplicationContext.
        /// </summary>
        public static NewApplicationContext getAppContext()
        {
            if (appContext == null)
            {
                appContext = new NewApplicationContext();
            }
            return appContext;
        }

        /// <summary>
        /// Runs the form
        /// </summary>
        public void RunForm(Form form)
        {
            // One more form is running
            formCount++;
            form.Text = "SpreadSheet" + formCount;
            // When this form closes, we want to find out
            form.FormClosed += (o, e) => { if (--formCount <= 0) ExitThread(); };
           
            // Run the form
            form.Show();
        }
        

    }

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Start an application context and run one form inside it
            NewApplicationContext appContext = NewApplicationContext.getAppContext();
            appContext.RunForm(new GUI());
            Application.Run(appContext);
        }
    }
}
