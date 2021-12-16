using DXP;
using EDP;
using NLog;
using System;
using System.IO;
using System.Net.Mail;
using System.Windows.Forms;

public class ErrorMail
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);

    /// <summary>
    /// Log and email error report.
    /// </summary>
    /// <param name="ErrorMsg">Custom message to be attached to the exception call stack.</param>
    /// <param name="ex">Exception raised.</param>
    public static void LogError(string ErrorMsg, Exception ex, string ProjPath = "")
    {
        _Log.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);

#if !DEBUG

        try
        {

            string ZipPath = "G:\\CADTOOLS\\Altium_Error_Projects\\";
            IPluginsRegistry PluginRegistry;
            IPluginsRegistryItem PluginRegistryItem;
            PluginRegistry = DXP.GlobalVars.Client.GetPluginsRegistry();
            PluginRegistryItem = PluginRegistry.FindItemByHRID(Util.SERVERNAME);

            string User = System.Environment.UserName;
            string Machine = System.Environment.MachineName;
            string Date = DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss");
            //Build error message.
            ErrorMsg += "\nUser: " + User + "\n";
            ErrorMsg += "Computer: " + Machine + "\n";
            ErrorMsg += "Date: " + Date + "\n";
            ErrorMsg += "\n\n";
            if (PluginRegistryItem != null)
                ErrorMsg += Util.SERVERNAME+" Ver: " + PluginRegistryItem.GetVersion();
            ErrorMsg += "\n";
            ErrorMsg += ex.ToString();
            ErrorMsg += "\n\nStack Trace:\n";
            ErrorMsg += ex.StackTrace;
            //Send eror message via email if set up.
            ErrorMail Reporter = new ErrorMail();
            if (ToolsPreferences.SMTP_Enable)
                Reporter.EmailReport(ErrorMsg);

            //Log error message locally.
            //Reporter.LogReport(ErrorMsg);

            IProject project = DXP.GlobalVars.DXPWorkSpace.DM_FocusedProject() as IProject;
            if (project != null)
                ProjPath = Path.GetDirectoryName(project.DM_ProjectFullPath()) + "\\";



            if (ToolsPreferences.SMTP_Enable)
                MessageBox.Show("An Error has occured. The error has been logged and reported.\nContact tech support if this error continues.");
            else
                MessageBox.Show("An Error has occured. The error has been logged.\nContact tech support if this error continues.");

            if (ProjPath != "")
                if (MessageBox.Show("Do you wish to include a copy of the active project?", "Include Project", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    Helpers.Compression.SimpleZip(ProjPath, ZipPath + User + " " + Date.Replace(":",".") + ".zip");

        }
        catch
        {
            MessageBox.Show("An Error has occured. The error has been logged.\nContact tech support if this error continues.");
        }
        finally
        {
            DXP.Utils.PercentFinish();
        }
#endif
    }

    /// <summary>
    /// Email error message.
    /// </summary>
    /// <param name="ErrorMsg">Error message</param>
    private void EmailReport(string ErrorMsg)
    {
        _Log.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);

        //Configure emailer
        MailMessage message = new MailMessage(ToolsPreferences.FromAddress, ToolsPreferences.ToAddress);
        SmtpClient client = new SmtpClient();

        client.Host = ToolsPreferences.ClientHost;
        message.Subject = "!DO NOT REPLY! This is an error messgae from AD Extension " + Util.SERVERNAME + ".";
        message.Body = ErrorMsg;

        try
        {
            client.Send(message);

        }
        catch //(Exception ex)
        {
            Console.WriteLine("Error occured with email error logging. Please contact Randy Lyne x2259."); //direct to tech support for public release. Ok if just in SwRI.
        }
    }

    /// <summary>
    /// Log error message
    /// </summary>
    /// <param name="ErrorMsg">Error message</param>
    private void LogReport(string ErrorMsg)
    {
        _Log.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);

        string strPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\" + Util.SERVERNAME + " Logs\\";
        //"C:\\Users\\rlyne\\AppData\\Local"
        if (!Directory.Exists(strPath))
            Directory.CreateDirectory(strPath);
        StreamWriter LogWriter = new StreamWriter(strPath + "ErrorLog.txt", true);
        LogWriter.WriteLine("");
        LogWriter.WriteLine("");
        LogWriter.WriteLine(ErrorMsg);
        LogWriter.Close();
        LogWriter.Dispose();

    }
}

