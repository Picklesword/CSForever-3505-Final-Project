// program.cs by Jonathan Warner 3500 based heavily on code from demo 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    /// <summary>
    /// The SpreadsheetGUIApplicationContext will allow for mulitple 
    /// instances of the smae form type to exist without all closing when single 
    /// closes
    /// </summary>
    class SpreadsheetGUIApplicationContext : ApplicationContext
    {
        private int instance_count = 0; // keeps track of how many instances 
        private List<Form1> all_instances = new List<Form1>();  // exists as part of a future plan for gui 
        private static SpreadsheetGUIApplicationContext spreadappcontext;  // makes it a singleton 

        /// <summary>
        /// creates instance of spreadsheetguiApplicationcontext
        /// </summary>
        private SpreadsheetGUIApplicationContext()
        {

        }

        /// <summary>
        /// Creates a new app context if one does not exist 
        /// </summary>
        /// <returns></returns>
        public static SpreadsheetGUIApplicationContext getSpreadAppContext()
        {
            if (spreadappcontext == null)
            {
                spreadappcontext = new SpreadsheetGUIApplicationContext(); 
            }
            return spreadappcontext; 
        }

        /// <summary>
        /// Make sure some instance is running and checks 
        /// if any instances are running when one closes 
        /// so thread can end. 
        /// </summary>
        /// <param name="form"></param>
        public void RunInstance(Form form)
        {
            instance_count++;
            all_instances.Add((Form1)form); 
            form.FormClosed += (o, e) => { if (--instance_count <= 0) ExitThread(); };

            form.Show(); 
        }
        
    }

    /// <summary>
    /// The main program lanches the gui 
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application to start.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            SpreadsheetGUIApplicationContext spreadsheet_context = SpreadsheetGUIApplicationContext.getSpreadAppContext();
            spreadsheet_context.RunInstance(new Form1());
            
            Application.Run(spreadsheet_context);
        }
    }
}
