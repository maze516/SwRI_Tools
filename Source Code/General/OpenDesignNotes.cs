using DXP;
using Ini;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;


class DesignNotes
{
    /// <summary>
    /// 
    /// </summary>
    public void OpenDesignNotes()
    {
        try
        {
            IDXPProject Project = DXP.GlobalVars.DXPWorkSpace.DM_FocusedProject();
            Dictionary<string, string> Params = GetParams();
            IOptionsStorage ProjOptions = Project.DM_OptionsStorage();
            if (Project == null)
            {
                MessageBox.Show("Project not found. Try again.");
                return;
            }

            string strProjPath = Util.ProjPath();

            if (strProjPath == "\\")
            {
                MessageBox.Show("Project not found. Try again.");
                return;
            }

            string DesignNotesPath = OneNotePath(strProjPath, "Open Notebook.onetoc2");
            string ProjAssy = "", ProjQual = "", PwbNum = "";


            for (int i = 0; i <= Project.DM_GetParameterCount() - 1; i++)
            {
                if (Project.DM_GetParameterName(i) == "SwRI_Pwb_Qual")
                    ProjQual = Project.DM_GetParameterValue(i);
                else if (Project.DM_GetParameterName(i) == "SwRI_Title_Assy")
                    ProjAssy = Project.DM_GetParameterValue(i);
                else if (Project.DM_GetParameterName(i) == "SwRI_Pwb_Number")
                    PwbNum = Project.DM_GetParameterValue(i);
            }

            if (ProjQual == null)
            {
                MessageBox.Show("SwRI_Pwb_Qual project parameter is blank. Please fill this parameter in and try again.");
                return;
            }
            if (ProjAssy == null)
            {
                MessageBox.Show("SwRI_Title_Assy project parameter is blank. Please fill this parameter in and try again.");
                return;
            }
            if (PwbNum == null)
            {
                MessageBox.Show("SwRI_Pwb_Number project parameter is blank. Please fill this parameter in and try again.");
                return;
            }
            if (PwbNum != "")
                PwbNum = PwbNum.Substring(0, 5);
            //SwRI_Pwb_Number
            System.IO.Directory.SetCurrentDirectory(Util.ProjPath());
            if (DesignNotesPath == "" && ProjQual != "" && ProjAssy != "")
            {
                CopyFile(Params["template"], ".\\" + PwbNum + "_" + ProjAssy + "_" + ProjQual + "_Notes");
                DesignNotesPath = Util.ProjPath() + PwbNum + "_" + ProjAssy + "_" + ProjQual + "_Notes\\Open Notebook.onetoc2";
            }
            //CopyFile(Params["template"], ".\\Design_Notes_" + ProjAssy + "_" + ProjQual);
            //DesignNotesPath = Util.ProjPath() + "Design_Notes_" + ProjAssy + "_" + ProjQual + "\\Open Notebook.onetoc2";

            else if (DesignNotesPath.Contains("Design_Notes\\Open Notebook.onetoc2"))
            {
                try
                {
                    Directory.Move(Path.GetDirectoryName(DesignNotesPath) + "\\", Util.ProjPath() + PwbNum + "_" + ProjAssy + "_" + ProjQual + "_Notes\\");
                    DesignNotesPath = Util.ProjPath() + PwbNum + "_" + ProjAssy + "_" + ProjQual + "_Notes\\Open Notebook.onetoc2";
                }
                catch //(IOException ex)
                {
                    //MessageBox.Show("")
                    //throw;
                }
            }
            else if (DesignNotesPath.Contains("Design_Notes_" + ProjAssy + "_" + ProjQual + "\\Open Notebook.onetoc2"))
            {
                try
                {
                    Directory.Move(Path.GetDirectoryName(DesignNotesPath) + "\\", Util.ProjPath() + PwbNum + "_" + ProjAssy + "_" + ProjQual + "_Notes\\");
                    DesignNotesPath = Util.ProjPath() + PwbNum + "_" + ProjAssy + "_" + ProjQual + "_Notes\\Open Notebook.onetoc2";
                }
                catch //(IOException ex)
                {
                    //MessageBox.Show("")
                    //throw;
                }
            }
            else if ((ProjQual == "" || ProjAssy == "") && DesignNotesPath == "")
            {
                CopyFile(Params["template"], ".\\" + Path.GetFileNameWithoutExtension(Project.DM_ProjectFileName()) + "_Notes");
                DesignNotesPath = Util.ProjPath() + Path.GetFileNameWithoutExtension(Project.DM_ProjectFileName()) + "_Notes" + "\\Open Notebook.onetoc2";
                //create folder using project name instead.
                //MessageBox.Show("Missing project Qual and Assembly parameters.");
                //return;
            }

            System.Diagnostics.Process proc = System.Diagnostics.Process.Start(DesignNotesPath);
            proc.Dispose();

        }
        catch (Exception ex)
        {
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);

        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ProjectFolderPath"></param>
    /// <param name="FileName"></param>
    /// <returns></returns>
    string OneNotePath(string ProjectFolderPath, string FileName)
    {
        string[] Files = Directory.GetFiles(ProjectFolderPath);
        string output = "";
        foreach (string item in Files)
        {
            if (item.Contains(FileName))
                return item;
        }

        string[] SubDir = Directory.GetDirectories(ProjectFolderPath);
        foreach (string item in SubDir)
        {
            output = OneNotePath(item, FileName);
            if (output != "")
                return output;
        }
        return "";
    }

    /// <summary>
    /// Copy a file from source to destination and create directory if needed.
    /// </summary>
    /// <param name="FromPath">Source file</param>
    /// <param name="DestPath">Destination path</param>
    /// <returns></returns>
    public bool CopyFile(string FromPath, string DestPath)
    {
        try
        {//todo: need to deal with template directories
         //Create directory if needed.

            DestPath = Path.GetFullPath(DestPath) + "\\";
            if (!System.IO.Directory.Exists(DestPath))
                System.IO.Directory.CreateDirectory(DestPath);
            //Copy all the files & Replaces any files with the same name

            foreach (string newPath in Directory.GetFiles(FromPath, "*.*", SearchOption.TopDirectoryOnly))
                File.Copy(newPath, newPath.Replace(FromPath, DestPath), true);

            return true;
        }
        catch //(Exception ex)
        {
            return false;
        }
    }

    /// <summary>
    /// Decode parameters passed from Altium.
    /// </summary>
    /// <returns>Dictionary of Altium passed parameters.</returns>
    private Dictionary<string, string> GetParams()
    {
        Dictionary<string, string> Params = new Dictionary<string, string>() { { "relative", "false" }, { "filename", "" }, { "argument", "" }, { "template", "" }, { "ref", "DesignNotes" } };

        //Get param info from the Ext file config file.
        if (File.Exists(ToolsPreferences.ExtFileConfig))
        {
            IniFile ini = new IniFile(ToolsPreferences.ExtFileConfig);
            if (ini.IniReadValue("DesignNotes", "filename") != "")
            {
                Params["relative"] = ini.IniReadValue(Params["ref"], "relative");
                Params["filename"] = ini.IniReadValue(Params["ref"], "filename");
                Params["argument"] = ini.IniReadValue(Params["ref"], "argument");
                Params["template"] = ini.IniReadValue(Params["ref"], "template");
            }
        }

        return Params;
    }
}

