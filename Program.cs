using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ScriptDataTool
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UnZipForm());
        }
    }
}

// < add name = "KIPOSDB" connectionString = "Data Source=DEVHOST;Initial Catalog=ILIB_PORTAL.DHY_20260208;Integrated Security=false;UID=sa;PWD=xCorp@2024;MultipleActiveResultSets=True" providerName = "System.Data.SqlClient" />

