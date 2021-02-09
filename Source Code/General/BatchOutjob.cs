using DXP;
using EDP;
using PCB;
using System;

public class BatchOutjob
{
   
    public IPCB_ServerInterface PCBServer = PCB.GlobalVars.PCBServer;


    /// <summary>
    /// Allows for the user to select for multiple outjobs to be run in a batch process.
    /// </summary>
    public void StartOutjobBatch()
    {
        try
        {
            //Initialize the form.
            frmBatchOutjob frmOutjobForm = new frmBatchOutjob();
            frmOutjobForm.Show();
        }
        catch (Exception ex)
        {
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
        }
    }

}


public class clsODBFiles
{
    /// <summary>
    /// Initialize the class
    /// </summary>
    public clsODBFiles()
    {
        Board = null;
        PCBServerDoc = null;
        WasOpen = true;
    }

    /// <summary>
    /// Reference to PCB.
    /// </summary>
    public IPCB_Board Board { get; set; }
    /// <summary>
    /// Reference to PCB server document.
    /// </summary>
    public IServerDocument PCBServerDoc { get; set; }
    /// <summary>
    /// Set true if PCB file was already open.
    /// </summary>
    public bool WasOpen { set; get; }
}

/// <summary>
/// Used to store info of an Outjob doc.
/// </summary>
public class clsOutJob
{
    /// <summary>
    /// Outjob medium information.
    /// </summary>
    /// <param name="Medium">The outjob that is actually generated</param>
    /// <param name="Document">The outjob document</param>
    /// <param name="ODB">True if outjob is and ODB++ medium.</param>
    public clsOutJob(IOutputMedium Medium, IServerDocument Document, bool ODB)
    {
        OutputMedium = Medium;
        ServerDoc = Document;
        this.ODB = ODB;
    }

    public IOutputMedium OutputMedium { get; set; }

    public IServerDocument ServerDoc { get; set; }

    public bool ODB { get; set; }
}
