using DXP;
using PCB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NLog;
using System.Reflection;

public class Util
{

    public const string SERVERNAME = "SwRI_Tools";
    private static IPCB_ServerInterface PCBServer;
    //    public static readonly Logger _Log= LogManager.GetCurrentClassLogger();


    public static void UpdateLogger(LogLevel logLevel)
    {

        var config = new NLog.Config.LoggingConfiguration();
        string logPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\" + Util.SERVERNAME + " Logs\\";

        // Targets where to log to: File and Console
        var logfile = new NLog.Targets.FileTarget("logfile")
        {
            FileName = logPath + "\\SwRI_Tools.log",
            ArchiveNumbering = NLog.Targets.ArchiveNumberingMode.Rolling,
            ArchiveEvery = NLog.Targets.FileArchivePeriod.Day,
            ConcurrentWrites = true,
            MaxArchiveFiles = 14,
            Layout = "${longdate} ${callsite} ${uppercase:${level}} ${message} ${exception:format=toString}"
        };
        //var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

        // Rules for mapping loggers to targets            
        // config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
        config.AddRule(logLevel, LogLevel.Fatal, logfile);

        // Apply config           
        NLog.LogManager.Configuration = config;
    }
    /// <summary>
    /// Returns an Object set of all PCB primatives.
    /// </summary>
    public static PCB.TObjectSet PCBAllPrimitiveSet = new PCB.TObjectSet(new PCB.TObjectId[]
    {
        PCB.TObjectId.eArcObject,   //1
        PCB.TObjectId.ePadObject,   //2
        PCB.TObjectId.eViaObject,   //3
        PCB.TObjectId.eTrackObject, //4
        PCB.TObjectId.eTextObject,  //5
        PCB.TObjectId.eFillObject,  //6
        PCB.TObjectId.eConnectionObject,//7
        PCB.TObjectId.eNetObject,       //8
        PCB.TObjectId.eComponentObject, //9
        PCB.TObjectId.ePolyObject,      //10
        PCB.TObjectId.eComponentBodyObject,//12
        PCB.TObjectId.eDimensionObject, //13
        PCB.TObjectId.eCoordinateObject,//14
        PCB.TObjectId.eFromToObject,    //17
        PCB.TObjectId.eEmbeddedObject,  //20
        PCB.TObjectId.eEmbeddedBoardObject//21
    });

    /// <summary>
    /// Returns a Object set of all PCB objects.
    /// </summary>
    public static PCB.TObjectSet PCBAllObject = new PCB.TObjectSet(new PCB.TObjectId[]
    {
        PCB.TObjectId.eArcObject,           //1
        PCB.TObjectId.ePadObject,           //2
        PCB.TObjectId.eViaObject,           //3
        PCB.TObjectId.eTrackObject,         //4
        PCB.TObjectId.eTextObject,          //5
        PCB.TObjectId.eFillObject,          //6
        PCB.TObjectId.eConnectionObject,    //7
        PCB.TObjectId.eNetObject,           //8
        PCB.TObjectId.eComponentObject,     //9
        PCB.TObjectId.ePolyObject,          //10
        PCB.TObjectId.eRegionObject,        //11
        PCB.TObjectId.eComponentBodyObject, //12
        PCB.TObjectId.eDimensionObject,     //13
        PCB.TObjectId.eCoordinateObject,    //14
        PCB.TObjectId.eClassObject,         //15
        PCB.TObjectId.eRuleObject,          //16
        PCB.TObjectId.eFromToObject,        //17
        PCB.TObjectId.eDifferentialPairObject, //18
        PCB.TObjectId.eViolationObject,     //19
        PCB.TObjectId.eEmbeddedObject,      //20
        PCB.TObjectId.eEmbeddedBoardObject, //21
        PCB.TObjectId.eSplitPlaneObject,    //22
        PCB.TObjectId.eTraceObject,         //23
        PCB.TObjectId.eSpareViaObject,      //24
        PCB.TObjectId.eBoardObject,         //25
        PCB.TObjectId.eBoardOutlineObject   //26
    });


    /// <summary>
    /// Creates T6 layerset
    /// </summary>
    /// <param name="LayerArgs">Provide an array of T6 Layers</param>
    /// <returns>T6_LayerSet</returns>
    public static TV6_LayerSet MKset(params TV6_Layer[] LayerArgs)
    {
        TV6_LayerSet ReturnSet = new TV6_LayerSet();
        foreach (TV6_Layer tmpLayer in LayerArgs)
        {
            ReturnSet.Add(tmpLayer);
        }
        return ReturnSet;
    }
    /// <summary>
    /// Creates PCB object set
    /// </summary>
    /// <param name="ObjectArgs">Array of PCB Object Ids</param>
    /// <returns>PCB TObjectSet</returns>
    public static PCB.TObjectSet MKset(params PCB.TObjectId[] ObjectArgs)
    {
        PCB.TObjectSet ReturnSet = new PCB.TObjectSet();
        foreach (PCB.TObjectId tmpObject in ObjectArgs)
        {
            ReturnSet.Add(tmpObject);
        }
        return ReturnSet;
    }
    /// <summary>
    /// Creates SCH object set
    /// </summary>
    /// <param name="ObjectArgs">Array of SCH Object Ids</param>
    /// <returns>SCH TObjectSet</returns>
    public static SCH.TObjectSet MKset(params SCH.TObjectId[] ObjectArgs)
    {
        SCH.TObjectSet ReturnSet = new SCH.TObjectSet();
        foreach (SCH.TObjectId tmpObject in ObjectArgs)
        {
            ReturnSet.Add(tmpObject);
        }
        return ReturnSet;
    }

    /// <summary>
    /// Get the path of the project for the selected document.
    /// "T:\\users\\RLYNE\\test projects\\OMAP_L138_SOM (2-17-2020 9-04-00 AM)\\"
    /// </summary>
    /// <returns>Path of project.</returns>
    public static string ProjPath()
    {
        try
        {
            IDXPWorkSpace CurrentWorkspace = DXP.GlobalVars.DXPWorkSpace;

            IDXPProject CurrentProject;
            CurrentProject = CurrentWorkspace.DM_FocusedProject();
            if (CurrentProject == null) return "";
            return System.IO.Path.GetDirectoryName(CurrentProject.DM_ProjectFullPath()) + "\\";
        }
        catch (Exception ex)
        {
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            return "";

        }
    }

    /// <summary>
    /// Gets the focused projects output path.
    /// </summary>
    /// <returns>Path as string</returns>
    public static string OutputPath()
    {
        try
        {
            IDXPWorkSpace CurrentWorkspace = DXP.GlobalVars.DXPWorkSpace;
            IDXPProject CurrentProject = CurrentWorkspace.DM_FocusedProject();

            return CurrentProject.DM_GetOutputPath();
        }
        catch (Exception ex)
        {
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            return "";

        }
    }

    /// <summary>
    /// Array of schematic objects to be filtered.
    /// </summary>
    /// <returns>TObjectId array of object types to select.</returns>
    public static SCH.TObjectId[] AllSCHObjects()
    {
        return new SCH.TObjectId[] { SCH.TObjectId.eWire, SCH.TObjectId.eNote, SCH.TObjectId.eLine, SCH.TObjectId.eLabel, SCH.TObjectId.eDesignator, SCH.TObjectId.eSchComponent, SCH.TObjectId.eSheetSymbol, SCH.TObjectId.eSymbol, SCH.TObjectId.ePowerObject, SCH.TObjectId.ePort, SCH.TObjectId.eProbe, SCH.TObjectId.eRectangle, SCH.TObjectId.eRoundRectangle, SCH.TObjectId.eSignalHarness, SCH.TObjectId.ePin, SCH.TObjectId.eNetLabel, SCH.TObjectId.eNoERC, SCH.TObjectId.eLine, SCH.TObjectId.eJunction, SCH.TObjectId.eTextFrame };
    }

    /// <summary>
    /// Retrieve the current open PCB.
    /// </summary>
    /// <returns>Returns IPCB_Board if PCB file is active. Returns null if no PCB file is active.</returns>
    public static IPCB_Board GetCurrentPCB(bool OpenPCB = false)
    {
        try
        {
            PCBServer = PCB.GlobalVars.PCBServer;
            if (OpenPCB)
            {
                IDXPWorkSpace CurrentWorkspace = DXP.GlobalVars.DXPWorkSpace; //Get workspace
                IDXPProject CurrentProject;
                int LogicalDocumentCount;
                int LoopIterator;
                IDXPDocument CurrentSheet;
                CurrentProject = CurrentWorkspace.DM_FocusedProject(); //Get current project.
                LogicalDocumentCount = CurrentProject.DM_LogicalDocumentCount(); //Get count of documents in the selected project.

                IClient Client = DXP.GlobalVars.Client;
                IServerDocument ServerDoc;
                IDXPDocument ActiveDoc = DXP.GlobalVars.DXPWorkSpace.DM_FocusedDocument(); //Save current open document so it can be reopened after process is done.

                //Loop through all project documents.
                for (LoopIterator = 1; LoopIterator <= LogicalDocumentCount; LoopIterator++)
                {
                    CurrentSheet = CurrentProject.DM_LogicalDocuments(LoopIterator - 1);

                    //Find the first PCB in the project.
                    if (CurrentSheet.DM_DocumentKind() == "PCB")
                    {
                        IPCB_Board PCBDoc = CurrentSheet as IPCB_Board;
                        //Open PCB file if not already open.
                        if (Client.IsDocumentOpen(CurrentSheet.DM_FullPath()))
                        {
                            ServerDoc = Client.GetDocumentByPath(CurrentSheet.DM_FullPath());
                        }
                        else
                            ServerDoc = Client.OpenDocument("PCB", CurrentSheet.DM_FullPath());

                        Client.ShowDocument(ServerDoc);
                        break;
                    }
                }
            }


            IPCB_Board Board;
            if (PCBServer == null)
                return null;

            Board = PCBServer.GetCurrentPCBBoard(); //Get current board
            if (Board == null)
                return null;
            return Board;
        }
        catch (Exception ex)
        {
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            return null;
        }
    }

    /// <summary>
    /// Gets a list of signal layers for the provided board file.
    /// </summary>
    /// <param name="Board">Current board</param>
    /// <returns>List of layers.</returns>
    public static List<V7_Layer> GetV7SigLayers(IPCB_Board Board)
    {
        List<V7_Layer> tempList = new List<V7_Layer>();
        //IPCB_LayerSet test = Board.ElectricalLayers();
        IPCB_LayerIterator LayerIterator = Board.LayerIterator();
        LayerIterator.AddFilter_SignalLayers();
        LayerIterator.First();
        V7_Layer tmpLayer;
        //tmpLayer =  new V7_Layer( LayerIterator.Layer().Data);

        do
        {
            tmpLayer = new V7_Layer(LayerIterator.Layer().Data);
            //if (EDP.Utils.LayerToString(tmpLayer).Contains("Mid"))
            tempList.Add(tmpLayer);

        } while (LayerIterator.Next());
        return tempList;
    }

    /// <summary>
    /// Gets a list of electrical layers for the provided board file.
    /// </summary>
    /// <param name="Board">Current board</param>
    /// <returns>List of layers.</returns>
    public static List<V7_Layer> GetV7ElectLayers(IPCB_Board Board)
    {
        List<V7_Layer> tempList = new List<V7_Layer>();
        IPCB_LayerIterator LayerIterator = Board.LayerIterator();
        LayerIterator.AddFilter_ElectricalLayers();
        LayerIterator.First();
        V7_Layer tmpLayer;
        //tmpLayer =  new V7_Layer( LayerIterator.Layer().Data);

        do
        {
            tmpLayer = new V7_Layer(LayerIterator.Layer().Data);
            //if (EDP.Utils.LayerToString(tmpLayer).Contains("Mid"))
            tempList.Add(tmpLayer);

        } while (LayerIterator.Next());
        return tempList;
    }

    /// <summary>
    /// Gets a list of plane layers for the provided board file.
    /// </summary>
    /// <param name="Board">Current board</param>
    /// <returns>List of layers.</returns>
    public static List<V7_Layer> GetV7PlaneLayers(IPCB_Board Board)
    {
        List<V7_Layer> tempList = new List<V7_Layer>();
        //IPCB_LayerSet test = Board.ElectricalLayers();
        IPCB_LayerIterator LayerIterator = Board.LayerIterator();
        LayerIterator.AddFilter_InternalPlaneLayers();
        LayerIterator.First();
        V7_Layer tmpLayer;
        //tmpLayer =  new V7_Layer( LayerIterator.Layer().Data);

        do
        {
            tmpLayer = new V7_Layer(LayerIterator.Layer().Data);
            //if (EDP.Utils.LayerToString(tmpLayer).Contains("Mid"))
            tempList.Add(tmpLayer);

        } while (LayerIterator.Next());
        return tempList;
    }

    /// <summary>
    /// Gets a list of signal layers for the provided board file.
    /// </summary>
    /// <param name="Board">Current board</param>
    /// <returns>List of layers.</returns>
    public static List<TV6_Layer> GetV6SigLayers(IPCB_Board Board)
    {
        List<TV6_Layer> tempList = new List<TV6_Layer>();
        IPCB_LayerIterator LayerIterator = Board.LayerIterator();
        LayerIterator.AddFilter_SignalLayers();
        LayerIterator.First();
        TV6_Layer tmpLayer;
        //tmpLayer =  new V7_Layer( LayerIterator.Layer().Data);

        do
        {
            tmpLayer = (TV6_Layer)LayerIterator.Layer().GetDEBUGV6LAYER();
            //if (EDP.Utils.LayerToString(tmpLayer).Contains("Mid"))
            tempList.Add(tmpLayer);

        } while (LayerIterator.Next());
        return tempList;
    }

    /// <summary>
    /// Gets a list of electrical layers for the provided board file.
    /// </summary>
    /// <param name="Board">Current board</param>
    /// <returns>List of layers.</returns>
    public static List<TV6_Layer> GetV6ElectLayers(IPCB_Board Board)
    {
        List<TV6_Layer> tempList = new List<TV6_Layer>();
        IPCB_LayerIterator LayerIterator = Board.LayerIterator();
        LayerIterator.AddFilter_ElectricalLayers();
        LayerIterator.First();
        TV6_Layer tmpLayer;

        do
        {
            tmpLayer = (TV6_Layer)LayerIterator.Layer().GetDEBUGV6LAYER();
            tempList.Add(tmpLayer);

        } while (LayerIterator.Next());
        return tempList;
    }

    /// <summary>
    /// Gets a list of all layers for the provided board file.
    /// </summary>
    /// <param name="Board">Current board</param>
    /// <returns>List of layers.</returns>
    public static List<TV6_Layer> GetV6Layers(IPCB_Board Board)
    {
        List<TV6_Layer> tempList = new List<TV6_Layer>();
        IPCB_LayerIterator LayerIterator = Board.LayerIterator();
        //LayerIterator.AddFilter_ElectricalLayers();
        LayerIterator.First();
        TV6_Layer tmpLayer;

        do
        {
            tmpLayer = (TV6_Layer)LayerIterator.Layer().GetDEBUGV6LAYER();
            tempList.Add(tmpLayer);

        } while (LayerIterator.Next());
        return tempList;
    }

    /// <summary>
    /// Will get the user layername based on the layer ID.
    /// </summary>
    /// <param name="Board">Board to get the layer name from.</param>
    /// <param name="LayerID">Layer data</param>
    /// <returns>User created name for provided layer.</returns>
    public static string GetLayerName(IPCB_Board Board, V7_LayerBase LayerID)
    {
        IPCB_LayerStack_V7 tmpLayerstack = Board.GetState_LayerStack_V7();
        IPCB_LayerObject_V7 objLayer;
        objLayer = tmpLayerstack.FirstLayer();
        while (objLayer != null)
        {
            if (LayerID.ID == objLayer.V7_LayerID().ID)
                return objLayer.GetState_LayerName();
            objLayer = tmpLayerstack.NextLayer(objLayer);
        }
        return "";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="component"></param>
    /// <returns></returns>
    public static IPCB_Component Place(IPCB_Component component)
    {
        var pcbServer = PCB.GlobalVars.PCBServer;
        var pcbBoard = pcbServer?.GetCurrentPCBBoard();

        if (pcbBoard == null)
            return null;

        component.SetState_Board(pcbBoard);
        pcbBoard.AddPCBObject(component);
        component.SetState_Selected(true);

        DXP.Utils.RunCommand("PCB:PlaceComponent", "RepositionSelected=True");

        if (component.GetState_XLocation().Equals(-1) || component.GetState_YLocation().Equals(-1))
        {
            pcbBoard.RemovePCBObject(component);
            component = null;
        }
        else
        {
            component.SetState_Selected(false);
            component.ResetDisplacement();
            pcbBoard.GraphicalView_ZoomRedraw();
        }

        return component;
    }

    public static void Log(List<string> output, string Path)
    {
        StreamWriter sw = new StreamWriter(Path);
        foreach (string item in output)
        {
            sw.WriteLine(item);

        }
        sw.Close();
    }
    public static void AppendLog(List<string> output, string Path)
    {
        StreamWriter sw = new StreamWriter(Path, true);
        foreach (string item in output)
        {
            sw.WriteLine(item);

        }
        sw.Close();
    }
}

/// <summary>
/// Custon string comparer
/// </summary>
internal class StringListCompare : IComparer<string>
{
    /// <summary>
    /// Will sort two strings with numbers at the end.
    /// </summary>
    /// <param name="x">String 1</param>
    /// <param name="y">String 2</param>
    /// <returns></returns>
    public int Compare(string x, string y)
    {
        string pattern = "([A-Za-z])([0-9]+)";
        string h1 = Regex.Match(x, pattern).Groups[1].Value;
        string h2 = Regex.Match(y, pattern).Groups[1].Value;
        if (h1 != h2)
            return h1.CompareTo(h2);
        string t1 = Regex.Match(x, pattern).Groups[2].Value;
        string t2 = Regex.Match(y, pattern).Groups[2].Value;
        return int.Parse(t1).CompareTo(int.Parse(t2));
    }

}

/// <summary>
/// Custom KVP comparer
/// </summary>
public class KvpKeyComparer : IComparer<KeyValuePair<string, string>>

{
    /// <summary>
    /// Will compare two KeyValuePair<string,string> based on 
    /// the key string value with a number at the end.
    /// </summary>
    /// <param name="x">KeyValuePair<string, string></param>
    /// <param name="y">KeyValuePair<string, string></param>
    /// <returns></returns>
    public int Compare(KeyValuePair<string, string> x, KeyValuePair<string, string> y)
    {
        //if (x.Key == null)
        //{
        //    if (y.Key == null)
        //        return 0;
        //    return -1;
        //}

        //if (y.Key == null)
        //    return 1;

        //return x.Key.CompareTo(y.Key);
        string pattern = "([A-Za-z])([0-9]+)";
        string h1 = Regex.Match(x.Key, pattern).Groups[1].Value;
        string h2 = Regex.Match(y.Key, pattern).Groups[1].Value;
        if (h1 != h2)
            return h1.CompareTo(h2);
        string t1 = Regex.Match(x.Key, pattern).Groups[2].Value;
        string t2 = Regex.Match(y.Key, pattern).Groups[2].Value;
        return int.Parse(t1).CompareTo(int.Parse(t2));
    }

}
