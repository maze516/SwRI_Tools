using Ini;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
public class OpenExtFile
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);

    /// <summary>
    /// Open external file.
    /// </summary>
    /// <param name="parameters">CSV string of parameters.
    /// filename: file path (Required)
    /// relative: true, false (Required)
    /// template: path to file to copy if provided filename does not exist.  
    /// </param>
    public void OpenEXTFile(string parameters)
    {
        try
        {
            //Prefill parameter list.
            Dictionary<string, string> Params = GetParams(parameters);


            //If file path is relative then set working directory to the project path.
            if (Params["relative"] == "true")
            {
                System.IO.Directory.SetCurrentDirectory(Util.ProjPath());
            }

            //Verify file exists.
            if (!System.IO.File.Exists(Params["filename"]))
            {
                //If file doesnt exist and a template file provided then copy it over.
                if (Params["template"] != "")
                {
                    CopyFile(Params["template"], Params["filename"]);
                }
                else
                {
                    DXP.Utils.ShowError("File doesnt exist and no template file provided.");
                    return;
                }
            }
            //Open file then clean-up.
            System.Diagnostics.Process proc = System.Diagnostics.Process.Start(Params["filename"], Params["argument"]);
            proc.Dispose();
        }
        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(ex.ToString());
            _Log.Fatal(sb);
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);

        }
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
            if (File.Exists(FromPath))
            {
                if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(DestPath)))
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(DestPath));
                System.IO.File.Copy(FromPath, DestPath); //Copy file.
            }
            else
            {
                DestPath = System.IO.Path.GetDirectoryName(Path.GetFullPath(DestPath)) + "\\";
                if (!System.IO.Directory.Exists(DestPath))
                    System.IO.Directory.CreateDirectory(DestPath);
                //Copy all the files & Replaces any files with the same name

                foreach (string newPath in Directory.GetFiles(FromPath, "*.*", SearchOption.TopDirectoryOnly))
                    File.Copy(newPath, newPath.Replace(FromPath, DestPath), true);
            }
            return true;
        }
        catch //(Exception ex)
        {
            return false;
        }
    }

    /// <summary>
    /// Decode parameters passed from Altiu.
    /// </summary>
    /// <param name="Parameters">Altium parameter string.</param>
    /// <returns>Dictionary of Altium passed parameters.</returns>
    private Dictionary<string, string> GetParams(string Parameters)
    {
        Dictionary<string, string> Params = new Dictionary<string, string>() { { "relative", "false" }, { "filename", "" }, { "argument", "" }, { "template", "" }, { "ref", "" } };

        //Populate parameter list.
        foreach (string Param in Parameters.Split(','))
        {
            Params[Param.Split('=')[0].ToLower()] = Param.Split('=')[1];
        }

        //Get param info from the Ext file config file.
        if (File.Exists(ToolsPreferences.ExtFileConfig) && Params["ref"] != "")
        {
            IniFile ini = new IniFile(ToolsPreferences.ExtFileConfig);
            if (ini.IniReadValue(Params["ref"], "filename") != "")
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
