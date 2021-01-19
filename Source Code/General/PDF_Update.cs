using DXP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class PDF_Update
{

    public void Update_PDF_Sch_Links()
    {
        string ProjPath = Util.ProjPath();


        IDXPProject Project = DXP.GlobalVars.DXPWorkSpace.DM_FocusedProject();
        IOptionsStorage ProjOptions = Project.DM_OptionsStorage();
        if (Project == null)
        {
            MessageBox.Show("Project not found. Try again.");
            return;
        }

        if (ProjPath == "\\")
        {
            MessageBox.Show("Project not found. Try again.");
            return;
        }

        string SchRev = "";
        //SwRI_SCH_Rev

        for (int i = 0; i <= Project.DM_GetParameterCount() - 1; i++)
        {
            if (Project.DM_GetParameterName(i) == "SwRI_SCH_Rev")
                SchRev = Project.DM_GetParameterValue(i);
        }
        if (SchRev == "")
        {
            MessageBox.Show("Schematic rev not found.");
            return;
        }

        string SchFolder = ProjPath + "Project Outputs\\sch" + SchRev + "\\";

        if (!Directory.Exists(SchFolder))
        {
            MessageBox.Show("Schematic project output folder missing.");
            return;
        }
        bool errors = false;
        int count = 0;
        foreach (string path in Directory.GetFiles(SchFolder))
        {
            if (path.ToLower().EndsWith("pdf"))
            {
                if (UpdatePDF(path) == false) errors = true;
                count++;
            }
        }

        if (errors) MessageBox.Show("Something went wrong. A PDF may have been open.\r\nPlease close the PDF and try again.");
        MessageBox.Show("Process complete.\r\n" + count + " file(s) processed");
    }
    bool UpdatePDF(string path)
    {
        try
        {
            if (!path.ToLower().EndsWith("pdf")) return true;
            Encoding encode = Encoding.Default;
            string text = File.ReadAllText(path, encode);
            text = text.Replace(@"/:\\s\(\(\\/\)|\(\\\\\\\\\)\).+/", @"/:\\s[a-z]:.+/i");
            File.WriteAllText(path, text, encode);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}