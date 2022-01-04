using PCB;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NLog;
using DXP;
using SCH;

/// <summary>
/// 
/// </summary>
public class PlaceReplicate
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);
    public List<string> SelectedSourceRef, SelectedDestRef;
    public clsSelectedObjects selectedSourceObjects, selectedDestinationObjects;
    public clsOutput Source, Destination;
    public Dictionary<string, IPCB_Net> BoardNets;
    public Dictionary<string, List<structNet>> SourceNets, DestNets;


    public PlaceReplicate()
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        Source = new clsOutput();
        Destination = new clsOutput();
        BoardNets = new Dictionary<string, IPCB_Net>();
        SourceNets = new Dictionary<string, List<structNet>>();
        DestNets = new Dictionary<string, List<structNet>>();
    }

    /// <summary>
    /// Collect all the selected source objects.
    /// </summary>
    public void GetInitialParts()
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        try
        {
            SelectedSourceRef = new List<string>();
            selectedSourceObjects = new clsSelectedObjects();
            SourceNets = new Dictionary<string, List<structNet>>();
            Source = new clsOutput();

            IPCB_BoardIterator BoardIterator;

            IPCB_Primitive Primitive;

            IPCB_Board Board = Util.GetCurrentPCB();

            if (Board == null)
                return;

            //Iterate theough all components on the board.
            BoardIterator = Board.BoardIterator_Create();
            PCB.TObjectSet FilterSet = new PCB.TObjectSet();
            BoardIterator.AddFilter_ObjectSet(Util.PCBAllObject);
            BoardIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet); //Filter all layers.
            BoardIterator.AddFilter_Method(TIterationMethod.eProcessAll);

            Primitive = BoardIterator.FirstPCBObject();

            ///Collect selected object info
            ///Add these objects to the Source var.
            while (Primitive != null)
            {
                if (Primitive.GetState_Selected())
                {
                    switch (Primitive.GetState_ObjectID())
                    {
                        #region Arcs
                        case PCB.TObjectId.eArcObject:  //Arcs
                            IPCB_Arc arcObject = Primitive as IPCB_Arc;
                            selectedSourceObjects.arcObjects.Add(arcObject);
                            clsOutput.st_IPCB_Arc newArc = new clsOutput.st_IPCB_Arc();

                            newArc.StartX = arcObject.GetState_StartX();
                            newArc.StartY = arcObject.GetState_StartY();
                            newArc.EndX = arcObject.GetState_EndX();
                            newArc.EndY = arcObject.GetState_EndY();
                            newArc.StartAngle = arcObject.GetState_StartAngle();
                            newArc.EndAngle = arcObject.GetState_EndAngle();
                            newArc.CenterX = arcObject.GetState_CenterX();
                            newArc.CenterY = arcObject.GetState_CenterY();

                            newArc.KeepOut = arcObject.GetState_IsKeepout();

                            newArc.LineWidth = arcObject.GetState_LineWidth();

                            if (arcObject.GetState_Net() != null)
                                newArc.Net = arcObject.GetState_Net().GetState_Name();
                            else
                                newArc.Net = null;

                            newArc.PasteMaskExpansion = arcObject.GetState_PasteMaskExpansion();
                            newArc.Radius = arcObject.GetState_Radius();
                            newArc.SolderMaskExpansion = arcObject.GetState_SolderMaskExpansion();
                            newArc.Layer = Util.GetLayerName(arcObject.GetState_Board(), arcObject.GetState_V7Layer());

                            Source.Arcs.Add(newArc);

                            _Log.Debug(Primitive.GetState_DescriptorString());
                            break;
                        #endregion
                        #region Pads
                        case PCB.TObjectId.ePadObject:
                            IPCB_Pad padObject = Primitive as IPCB_Pad;
                            selectedSourceObjects.padObjects.Add(padObject);
                            _Log.Debug(Primitive.GetState_DescriptorString());
                            break;
                        #endregion
                        #region Vias
                        case PCB.TObjectId.eViaObject:
                            _Log.Debug(Primitive.GetState_DescriptorString());
                            IPCB_Via ViaObject = Primitive as IPCB_Via;

                            clsOutput.st_IPCB_Via newVia = new clsOutput.st_IPCB_Via();
                            string test = "";
                            ViaObject.Export_ToParameters(ref test);

                            newVia.HoleSize = ViaObject.GetState_HoleSize();
                            newVia.IsTestPoint_Bottom = ViaObject.GetState_IsTestPoint_Bottom();
                            newVia.IsTestPoint_Top = ViaObject.GetState_IsTestPoint_Top();
                            newVia.IsTenting = ViaObject.GetState_IsTenting();
                            newVia.IsTenting_Bottom = ViaObject.GetState_IsTenting_Bottom();
                            newVia.IsTenting_Top = ViaObject.GetState_IsTenting_Top();
                            //ViaObject.GetState_Layer(); ViaObject.GetState_V7Layer();
                            newVia.Net = ViaObject.GetState_Net().GetState_Name();
                            newVia.Size = ViaObject.GetState_Size();
                            newVia.SolderMaskExpansion = ViaObject.GetState_SolderMaskExpansion();
                            newVia.HighLayer = Util.GetLayerName(ViaObject.GetState_Board(), ViaObject.GetState_HighLayer());
                            newVia.LowLayer = Util.GetLayerName(ViaObject.GetState_Board(), ViaObject.GetState_LowLayer());
                            newVia.TearDrop = ViaObject.GetState_TearDrop();
                            newVia.XLocation = ViaObject.GetState_XLocation();
                            newVia.YLocation = ViaObject.GetState_YLocation();

                            Source.Vias.Add(newVia);

                            selectedSourceObjects.ViaObjects.Add(ViaObject);
                            break;
                        #endregion
                        #region Tracks
                        case PCB.TObjectId.eTrackObject:
                            IPCB_Track trackObject = Primitive as IPCB_Track;
                            _Log.Debug(Primitive.GetState_DescriptorString());
                            selectedSourceObjects.trackObjects.Add(trackObject);

                            clsOutput.st_IPCB_Track newTrack = new clsOutput.st_IPCB_Track();

                            newTrack.X1 = trackObject.GetState_X1();
                            newTrack.X2 = trackObject.GetState_X2();
                            newTrack.Y1 = trackObject.GetState_Y1();
                            newTrack.Y2 = trackObject.GetState_Y2();
                            newTrack.Width = trackObject.GetState_Width();
                            newTrack.Layer = Util.GetLayerName(trackObject.GetState_Board(), trackObject.GetState_V7Layer());

                            if (trackObject.GetState_Net() == null)
                                newTrack.Net = null;
                            else
                                newTrack.Net = trackObject.GetState_Net().GetState_Name();

                            newTrack.Keepout = trackObject.GetState_IsKeepout();

                            Source.Tracks.Add(newTrack);

                            break;
                        #endregion
                        #region Text
                        case PCB.TObjectId.eTextObject:
                            IPCB_Text textObject = Primitive as IPCB_Text;
                            selectedSourceObjects.textObjects.Add(textObject);
                            _Log.Debug(Primitive.GetState_DescriptorString());
                            break;
                        #endregion
                        #region Fills
                        case PCB.TObjectId.eFillObject:
                            IPCB_Fill fillObject = Primitive as IPCB_Fill;
                            selectedSourceObjects.fillObjects.Add(fillObject);

                            clsOutput.st_IPCB_Fill newFill = new clsOutput.st_IPCB_Fill();

                            newFill.Length = fillObject.GetState_Length();
                            newFill.LocationX = fillObject.GetState_LocationX();
                            newFill.LocationY = fillObject.GetState_LocationY();
                            if (fillObject.GetState_Net() != null)
                                newFill.Net = fillObject.GetState_Net().GetState_Name();
                            newFill.PasteMaskExpansion = fillObject.GetState_PasteMaskExpansion();
                            if (fillObject.GetState_Polygon() != null)
                                MessageBox.Show("Polygon error. Please fix.");
                            //fillObject.GetState_Polygon(); //convert to something
                            newFill.Rotation = fillObject.GetState_Rotation();
                            newFill.SolderMaskExpansion = fillObject.GetState_SolderMaskExpansion();
                            newFill.Layer = Util.GetLayerName(fillObject.GetState_Board(), fillObject.GetState_V7Layer());
                            newFill.Width = fillObject.GetState_Width();
                            newFill.X1Location = fillObject.GetState_X1Location();
                            newFill.X2Location = fillObject.GetState_X2Location();
                            newFill.XLocation = fillObject.GetState_XLocation();
                            newFill.Y1Location = fillObject.GetState_Y1Location();
                            newFill.Y2Location = fillObject.GetState_Y2Location();
                            newFill.YLocation = fillObject.GetState_YLocation();
                            newFill.Keepout = fillObject.GetState_IsKeepout();

                            Source.Fills.Add(newFill);

                            _Log.Debug(Primitive.GetState_DescriptorString());
                            break;
                        #endregion
                        //case PCB.TObjectId.eConnectionObject:
                        //    _Log.Debug(Primitive.GetState_DescriptorString());
                        //    break;
                        //case PCB.TObjectId.eNetObject:
                        //    _Log.Debug(Primitive.GetState_DescriptorString());
                        //    break;
                        #region Components
                        case PCB.TObjectId.eComponentObject:
                            IPCB_Component componentObject = Primitive as IPCB_Component;
                            selectedSourceObjects.componentObjects.Add(componentObject);

                            st_IPCB_Component newComp = new st_IPCB_Component();

                            IPCB_GroupIterator compIterator = componentObject.GroupIterator_Create();

                            PCB.TObjectSet compFilterset = new PCB.TObjectSet();
                            compFilterset.Add(PCB.TObjectId.ePadObject);
                            compIterator.AddFilter_ObjectSet(compFilterset);
                            compIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet); //Filter all layers.

                            IPCB_Pad pad = compIterator.FirstPCBObject() as IPCB_Pad;
                            int pinCount = 0;

                            newComp.ID = componentObject.GetState_UniqueId();
                            //Collecting nets
                            newComp.Nets = new Dictionary<string, string>();
                            while (pad != null)
                            {
                                if (pad.GetState_Net() != null)
                                {
                                    if (!newComp.Nets.ContainsKey(pad.GetState_Name()))
                                        newComp.Nets.Add(pad.GetState_Name(), pad.GetState_Net().GetState_Name());
                                    if (SourceNets.ContainsKey(pad.GetState_Net().GetState_Name()))
                                    {
                                        string tmp = componentObject.GetState_Name().GetState_Text();
                                        structNet tmpNet = new structNet
                                        {
                                            Pin = pad.GetState_Name(),
                                            RefDes = componentObject.GetState_Name().GetState_Text()
                                        };

                                        if (!SourceNets[pad.GetState_Net().GetState_Name()].Contains(tmpNet))
                                            SourceNets[pad.GetState_Net().GetState_Name()].Add(tmpNet);
                                    }
                                    else
                                    {
                                        SourceNets.Add(pad.GetState_Net().GetState_Name(), new List<structNet>
                                        {
                                            new structNet {
                                                Pin = pad.GetState_Name(),
                                                RefDes = componentObject.GetState_Name().GetState_Text()
                                            }
                                        });
                                    }
                                }
                                pinCount++;
                                pad = compIterator.NextPCBObject() as IPCB_Pad;
                            }

                            componentObject.GroupIterator_Destroy(ref compIterator);


                            newComp.Parameters = new Dictionary<string, string>();

                            IPCB_PrimitiveParameters componentParameters = componentObject as IPCB_PrimitiveParameters;
                            for (int i = 0; i < componentParameters.Count(); i++)
                            {
                                IPCB_Parameter parameter = componentParameters.GetParameterByIndex(i);
                                if (newComp.Parameters.ContainsKey(parameter.GetName()))
                                    _Log.Debug(parameter.GetName());
                                else
                                    newComp.Parameters.Add(parameter.GetName(), parameter.GetValue());
                            }


                            newComp.RefDes = componentObject.GetState_Name().GetState_Text();
                            newComp.Footprint = componentObject.GetState_Pattern();
                            newComp.PinCount = pinCount;
                            newComp.Dupe = false;

                            _Log.Trace(newComp.RefDes + ", " + newComp.RefDes.GetHashCode());
                            _Log.Trace(newComp.Footprint);
                            _Log.Trace(newComp.GetHashCode());
                            _Log.Trace("ID " + componentObject.GetState_UniqueId());

                            string srcKey;
                            if (Source.Components.ContainsRefdes(newComp.RefDes, out srcKey))
                            {
                                st_IPCB_Component tmp = Source.Components[srcKey];
                                tmp.Dupe = true;
                                Source.Components[srcKey] = tmp;

                                for (int i = 0; i < SelectedSourceRef.Count; i++)
                                {
                                    if (SelectedSourceRef[i] == newComp.RefDes)
                                    {
                                        SelectedSourceRef[i] = tmp.RefDes + " (" + tmp.Footprint + ")";
                                        break;
                                    }
                                }

                                newComp.Dupe = true;
                                Source.Components.Add(newComp.ID, newComp);
                                _Log.Debug(newComp.RefDes);
                            }
                            else
                                Source.Components.Add(newComp.ID, newComp);

                            if (newComp.Dupe)
                                SelectedSourceRef.Add(newComp.RefDes + " (" + newComp.Footprint + ")");
                            else
                                SelectedSourceRef.Add(newComp.RefDes);

                            //?componentObject.GetState_DescriptorString()
                            //"SOIC Component U5FM-QT#94L9#-20.000MHz (5528.098mil,6358.425mil) on Top Layer"
                            //? componentObject.GetState_DetailString()
                            //"Component U5FM Comment:QT#94L9#-20.000MHz Footprint: QT194"
                            //?componentObject.GetState_Layer()
                            //componentObject.GetState_Pattern() Footprint
                            //componentObject.GetState_SourceDesignator() Refdes
                            //componentObject.GetState_SourceFootprintLibrary() library
                            //componentObject.GetState_SourceLibReference() corp number
                            //componentObject.GetState_XLocation()
                            //componentObject.GetState_YLocation()

                            _Log.Debug(componentObject.GetState_DescriptorString());
                            break;
                        #endregion
                        #region Polygons
                        case PCB.TObjectId.ePolyObject:
                            IPCB_Polygon polygonObject = Primitive as IPCB_Polygon;
                            selectedSourceObjects.polygonObjects.Add(polygonObject);

                            clsOutput.st_IPCB_Polygon newPoly = new clsOutput.st_IPCB_Polygon();

                            //polygonObject.GetState_BorderWidth();
                            //polygonObject.GetState_Coordinate();
                            newPoly.Keepout = polygonObject.GetState_IsKeepout();
                            newPoly.Layer = Util.GetLayerName(polygonObject.GetState_Board(), polygonObject.GetState_V7Layer());
                            newPoly.MitreCorners = polygonObject.GetState_MitreCorners();

                            if (polygonObject.GetState_Net() != null)
                                newPoly.Net = polygonObject.GetState_Net().GetState_Name();

                            newPoly.PasteMaskExpansion = polygonObject.GetState_PasteMaskExpansion();

                            if (polygonObject.GetState_Polygon() != null)
                                MessageBox.Show("Polygon error. Please fix.");

                            newPoly.PolySegments = new List<SPolySegment>();
                            for (int i = 0; i < polygonObject.GetState_PointCount(); i++)
                                newPoly.PolySegments.Add(polygonObject.GetState_Segments(i).Data);

                            newPoly.SolderMaskExpansion = polygonObject.GetState_SolderMaskExpansion();
                            newPoly.TrackSize = polygonObject.GetState_TrackSize();//???
                            newPoly.XLocation = polygonObject.GetState_XLocation();
                            newPoly.YLocation = polygonObject.GetState_YLocation();
                            newPoly.PolyType = polygonObject.GetState_PolygonType();

                            Source.Polygons.Add(newPoly);
                            _Log.Debug(polygonObject.GetState_DescriptorString());
                            break;
                        #endregion
                        #region Regions
                        case PCB.TObjectId.eRegionObject:

                            IPCB_Region regionObject = Primitive as IPCB_Region;
                            selectedSourceObjects.regionObjects.Add(regionObject);

                            clsOutput.st_IPCB_Region newRegion = new clsOutput.st_IPCB_Region();

                            if (regionObject.GetHoleCount() > 0)
                                MessageBox.Show("Region issue");


                            newRegion.Keepout = regionObject.GetState_IsKeepout();
                            newRegion.RegionKind = regionObject.GetState_Kind();
                            newRegion.Layer = Util.GetLayerName(regionObject.GetState_Board(), regionObject.GetState_V7Layer());
                            if (regionObject.GetState_Net() != null)
                                newRegion.Net = regionObject.GetState_Net().GetState_Name();
                            newRegion.PasteMaskExpansion = regionObject.GetState_PasteMaskExpansion();
                            newRegion.SolderMaskExpansion = regionObject.GetState_SolderMaskExpansion();

                            List<clsOutput.st_Contour> Contours;

                            newRegion.GeometricPolygons = new List<List<clsOutput.st_Contour>>();

                            for (int i = 0; i < regionObject.GetGeometricPolygon().GetState_Count(); i++)
                            {
                                Contours = new List<clsOutput.st_Contour>();
                                for (int j = 0; j < regionObject.GetGeometricPolygon().GetState_Contour(i).GetState_Count(); j++)
                                {
                                    Contours.Add(new clsOutput.st_Contour()
                                    {
                                        X = regionObject.GetGeometricPolygon().GetState_Contour(i).GetState_PointX(j),
                                        Y = regionObject.GetGeometricPolygon().GetState_Contour(i).GetState_PointY(j)

                                    });
                                }
                                newRegion.GeometricPolygons.Add(Contours);
                            }

                            //?regionObject.GetGeometricPolygon().GetState_Count()
                            //?regionObject.GetGeometricPolygon().GetState_Contour(0)

                            //?regionObject.GetGeometricPolygon().GetState_Contour(0).GetState_Count()
                            //?regionObject.GetGeometricPolygon().GetState_Contour(0).GetState_PointY(0)
                            //?regionObject.GetGeometricPolygon().GetState_Contour(0).GetState_PointX(0)

                            Source.Regions.Add(newRegion);

                            _Log.Debug(Primitive.GetState_DescriptorString());

                            break;
                        #endregion
                        //case PCB.TObjectId.eComponentBodyObject:
                        //    _Log.Debug(Primitive.GetState_DescriptorString());
                        //    break;
                        //case PCB.TObjectId.eDimensionObject:
                        //    _Log.Debug(Primitive.GetState_DescriptorString());
                        //    break;
                        //case PCB.TObjectId.eCoordinateObject:
                        //    _Log.Debug(Primitive.GetState_DescriptorString());
                        //    break;
                        //case PCB.TObjectId.eClassObject:
                        //    _Log.Debug(Primitive.GetState_DescriptorString());
                        //    break;
                        //case PCB.TObjectId.eRuleObject:
                        //    _Log.Debug(Primitive.GetState_DescriptorString());
                        //    break;
                        //case PCB.TObjectId.eFromToObject:
                        //    _Log.Debug(Primitive.GetState_DescriptorString());
                        //    break;
                        //case PCB.TObjectId.eDifferentialPairObject:
                        //    _Log.Debug(Primitive.GetState_DescriptorString());
                        //    break;
                        //case PCB.TObjectId.eViolationObject:
                        //    _Log.Debug(Primitive.GetState_DescriptorString());
                        //    break;
                        //case PCB.TObjectId.eEmbeddedObject:
                        //    _Log.Debug(Primitive.GetState_DescriptorString());
                        //    break;
                        //case PCB.TObjectId.eEmbeddedBoardObject:
                        //    _Log.Debug(Primitive.GetState_DescriptorString());
                        //    break;
                        //case PCB.TObjectId.eSplitPlaneObject:
                        //    _Log.Debug(Primitive.GetState_DescriptorString());
                        //    break;
                        //case PCB.TObjectId.eTraceObject:
                        //    _Log.Debug(Primitive.GetState_DescriptorString());
                        //    break;
                        //case PCB.TObjectId.eSpareViaObject:
                        //    _Log.Debug(Primitive.GetState_DescriptorString());
                        //    break;
                        //case PCB.TObjectId.eBoardObject:
                        //    _Log.Debug(Primitive.GetState_DescriptorString());
                        //    break;
                        //case PCB.TObjectId.eBoardOutlineObject:
                        //    _Log.Debug(Primitive.GetState_DescriptorString());
                        //    break;
                        #region Default
                        default:
                            selectedSourceObjects.primitiveObjects.Add(Primitive);
                            _Log.Debug(Primitive.GetState_DescriptorString());
                            break;
                            #endregion
                    }


                    //Primitive.Export_ToParameters(ref RefDes);
                }
                Primitive = BoardIterator.NextPCBObject();

            }
            //Iterator clean-up
            Board.BoardIterator_Destroy(ref BoardIterator);

            //clsOutput outputb;
            //outputb = new clsOutput();
            //XmlSerialization.WriteToXmlFile<clsOutput>(@"S:\Dropbox\Altium Extensions\test.xml", Output);

            //outputb = XmlSerialization.ReadFromXmlFile<clsOutput>(@"S:\Dropbox\Altium Extensions\test.xml");
            //@"S:\Dropbox\Altium Extensions\test.xml"
            //"C:\\Users\\rlyne\\Dropbox\\Altium Extensions\\SwRI_Tools\\bin\\test.xml"
        }
        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(ex.ToString());
            _Log.Fatal(sb);
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            return;
        }
    }

    /// <summary>
    /// Collect all net objects on the board so they can be 
    /// applied to objects during placement process.
    /// </summary>
    public void GetNets()
    {
        _Log.Debug(">GetNets");
        IPCB_BoardIterator BoardIterator;
        IPCB_Net Net;
        BoardNets = new Dictionary<string, IPCB_Net>();

        IPCB_Board Board = Util.GetCurrentPCB();

        if (Board == null)
            return;

        //Iterate theough all components on the board.
        BoardIterator = Board.BoardIterator_Create();
        PCB.TObjectSet FilterSet = new PCB.TObjectSet();
        FilterSet.Add(PCB.TObjectId.eNetObject);
        BoardIterator.AddFilter_ObjectSet(FilterSet);
        BoardIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet); //Filter all layers.
        BoardIterator.AddFilter_Method(TIterationMethod.eProcessAll);

        Net = BoardIterator.FirstPCBObject() as IPCB_Net;

        while (Net != null)
        {
            if (!BoardNets.ContainsKey(Net.GetState_Name()))
                BoardNets.Add(Net.GetState_Name(), Net);

            Net = BoardIterator.NextPCBObject() as IPCB_Net;
        }

        Board.BoardIterator_Destroy(ref BoardIterator);

    }

    /// <summary>
    /// Collect all the selected destination components.
    /// </summary>
    public void GetDestinationParts()
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        IDXPDocument ActiveDoc = DXP.GlobalVars.DXPWorkSpace.DM_FocusedDocument();

        if (ActiveDoc.DM_DocumentKind() == "PCB")
        {
            SelectedDestRef = new List<string>();
            DestNets = new Dictionary<string, List<structNet>>();
            selectedDestinationObjects = new clsSelectedObjects();
            Destination = new clsOutput();

            string parameterName, parameterValue;

            IPCB_BoardIterator BoardIterator;
            IPCB_Parameter parameter;
            IPCB_Primitive Primitive;

            IPCB_Board Board = Util.GetCurrentPCB();

            if (Board == null)
                return;

            //Iterate theough all components on the board.
            BoardIterator = Board.BoardIterator_Create();
            PCB.TObjectSet FilterSet = new PCB.TObjectSet();
            BoardIterator.AddFilter_ObjectSet(Util.PCBAllObject);
            BoardIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet); //Filter all layers.
            BoardIterator.AddFilter_Method(TIterationMethod.eProcessAll);

            Primitive = BoardIterator.FirstPCBObject();

            while (Primitive != null)
            {
                if (Primitive.GetState_Selected())
                {
                    switch (Primitive.GetState_ObjectID())
                    {
                        #region Components
                        case PCB.TObjectId.eComponentObject:
                            IPCB_Component componentObject = Primitive as IPCB_Component;
                            selectedDestinationObjects.componentObjects.Add(componentObject);

                            st_IPCB_Component newComp = new st_IPCB_Component();

                            IPCB_GroupIterator compIterator = componentObject.GroupIterator_Create();

                            PCB.TObjectSet compFilterset = new PCB.TObjectSet();
                            compFilterset.Add(PCB.TObjectId.ePadObject);
                            compIterator.AddFilter_ObjectSet(compFilterset);
                            compIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet); //Filter all layers.

                            IPCB_Pad pad = compIterator.FirstPCBObject() as IPCB_Pad;
                            int pinCount = 0;

                            newComp.ID = componentObject.GetState_UniqueId();

                            newComp.Nets = new Dictionary<string, string>();
                            while (pad != null)
                            {
                                if (pad.GetState_Net() != null)
                                {
                                    if (!newComp.Nets.ContainsKey(pad.GetState_Name()))
                                        newComp.Nets.Add(pad.GetState_Name(), pad.GetState_Net().GetState_Name());
                                    if (DestNets.ContainsKey(pad.GetState_Net().GetState_Name()))
                                    {
                                        //string tmp = componentObject.GetState_Name().GetState_Text();
                                        structNet tmpNet = new structNet
                                        {
                                            Pin = pad.GetState_Name(),
                                            RefDes = componentObject.GetState_Name().GetState_Text()
                                        };

                                        if (!DestNets[pad.GetState_Net().GetState_Name()].Contains(tmpNet))
                                            DestNets[pad.GetState_Net().GetState_Name()].Add(tmpNet);
                                    }
                                    else
                                    {
                                        DestNets.Add(pad.GetState_Net().GetState_Name(), new List<structNet>
                                        {
                                            new structNet {
                                                Pin = pad.GetState_Name(),
                                                RefDes = componentObject.GetState_Name().GetState_Text()
                                            }
                                        });
                                    }
                                }
                                pinCount++;
                                pad = compIterator.NextPCBObject() as IPCB_Pad;
                            }

                            componentObject.GroupIterator_Destroy(ref compIterator);

                            IPCB_PrimitiveParameters componentParameters = componentObject as IPCB_PrimitiveParameters;
                            newComp.Parameters = new Dictionary<string, string>();
                            for (int i = 0; i < componentParameters.Count(); i++)
                            {
                                parameter = componentParameters.GetParameterByIndex(i);
                                parameterName = parameter.GetName();
                                if (parameter.GetValue() == null)
                                    parameterValue = "null";
                                else
                                    parameterValue = parameter.GetValue();

                                newComp.Parameters.Add(parameterName, parameterValue);
                            }

                            newComp.RefDes = componentObject.GetState_Name().GetState_Text();
                            newComp.Footprint = componentObject.GetState_Pattern();
                            newComp.PinCount = pinCount;
                            newComp.Dupe = false;


                            string dstKey;
                            if (Destination.Components.ContainsRefdes(newComp.RefDes, out dstKey))
                            {
                                st_IPCB_Component tmp = Destination.Components[dstKey];
                                tmp.Dupe = true;
                                Destination.Components[dstKey] = tmp;

                                for (int i = 0; i < SelectedDestRef.Count; i++)
                                {
                                    if (SelectedDestRef[i] == newComp.RefDes)
                                    {
                                        SelectedDestRef[i] = tmp.RefDes + " (" + tmp.Footprint + ")";
                                        break;
                                    }
                                }

                                newComp.Dupe = true;
                                Destination.Components.Add(newComp.ID, newComp);
                                _Log.Debug(newComp.RefDes);
                            }
                            else
                                Destination.Components.Add(newComp.ID, newComp);

                            if (newComp.Dupe)
                                SelectedDestRef.Add(newComp.RefDes + " (" + newComp.Footprint + ")");
                            else
                                SelectedDestRef.Add(newComp.RefDes);


                            //?componentObject.GetState_DescriptorString()
                            //"SOIC Component U5FM-QT#94L9#-20.000MHz (5528.098mil,6358.425mil) on Top Layer"
                            //? componentObject.GetState_DetailString()
                            //"Component U5FM Comment:QT#94L9#-20.000MHz Footprint: QT194"
                            //?componentObject.GetState_Layer()
                            //componentObject.GetState_Pattern() Footprint
                            //componentObject.GetState_SourceDesignator() Refdes 
                            //componentObject.GetState_SourceFootprintLibrary() library
                            //componentObject.GetState_SourceLibReference() corp number
                            //componentObject.GetState_XLocation()
                            //componentObject.GetState_YLocation()
                            _Log.Debug(componentObject.GetState_DescriptorString());
                            break;
                        #endregion
                        #region Default
                        default:
                            //selectedSourceObjects.primitiveObjects.Add(Primitive);
                            _Log.Debug(Primitive.GetState_DescriptorString());
                            break;
                            #endregion
                    }

                    //Primitive.Export_ToParameters(ref RefDes);
                }
                Primitive = BoardIterator.NextPCBObject();

            }
            //Iterator clean-up
            Board.BoardIterator_Destroy(ref BoardIterator);
        }
        else if (ActiveDoc.DM_DocumentKind() == "SCH")
        {

            List<string> SelectedPart = new List<string>();
            ISch_Document SchDoc = SCH.GlobalVars.SchServer.GetCurrentSchDocument();

            ISch_Iterator SchIterator;
            ISch_BasicContainer SchObject;
            List<string> SelectedIDs = new List<string>();
            if (SchDoc == null)
                return;

            //Iterate theough all components on the schematic.
            SchIterator = SchDoc.SchIterator_Create();
            SchIterator.AddFilter_ObjectSet(new SCH.TObjectSet(SCH.TObjectId.eSchComponent));

            SchObject = SchIterator.FirstSchObject();

            while (SchObject != null)
            {
                ISch_Component componentObject = SchObject as ISch_Component;

                if (componentObject.GetState_Selection())
                    SelectedPart.Add(componentObject.GetState_IdentifierString());

                SchObject = SchIterator.NextSchObject();

            }

            GetDestPartsFromSCH(SelectedPart);

        }
        else
        {
            MessageBox.Show("Unknown document type. Please only use PCB or SCH documents.");
        }
    }

    void GetDestPartsFromSCH(List<string> Components)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        Util.GetCurrentPCB(true);

        SelectedDestRef = new List<string>();
        DestNets = new Dictionary<string, List<structNet>>();
        selectedDestinationObjects = new clsSelectedObjects();
        Destination = new clsOutput();

        string parameterName, parameterValue;

        IPCB_BoardIterator BoardIterator;
        IPCB_Parameter parameter;
        IPCB_Component componentObject;

        IPCB_Board Board = Util.GetCurrentPCB();

        if (Board == null)
            return;

        //Iterate theough all components on the board.
        BoardIterator = Board.BoardIterator_Create();
        PCB.TObjectSet FilterSet = new PCB.TObjectSet();
        FilterSet.Add(PCB.TObjectId.eComponentObject);
        BoardIterator.AddFilter_ObjectSet(FilterSet);
        BoardIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet); //Filter all layers.
        BoardIterator.AddFilter_Method(TIterationMethod.eProcessAll);

        componentObject = BoardIterator.FirstPCBObject() as IPCB_Component;

        while (componentObject != null)
        {
            if (Components.Contains(componentObject.GetState_Name().GetState_Text()))
            {
                #region Components


                selectedDestinationObjects.componentObjects.Add(componentObject);

                st_IPCB_Component newComp = new st_IPCB_Component();

                IPCB_GroupIterator compIterator = componentObject.GroupIterator_Create();

                PCB.TObjectSet compFilterset = new PCB.TObjectSet();
                compFilterset.Add(PCB.TObjectId.ePadObject);
                compIterator.AddFilter_ObjectSet(compFilterset);
                compIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet); //Filter all layers.

                IPCB_Pad pad = compIterator.FirstPCBObject() as IPCB_Pad;
                int pinCount = 0;

                newComp.ID = componentObject.GetState_UniqueId();

                newComp.Nets = new Dictionary<string, string>();
                while (pad != null)
                {
                    if (pad.GetState_Net() != null)
                    {
                        if (!newComp.Nets.ContainsKey(pad.GetState_Name()))
                            newComp.Nets.Add(pad.GetState_Name(), pad.GetState_Net().GetState_Name());
                        if (DestNets.ContainsKey(pad.GetState_Net().GetState_Name()))
                        {
                            //string tmp = componentObject.GetState_Name().GetState_Text();
                            structNet tmpNet = new structNet
                            {
                                Pin = pad.GetState_Name(),
                                RefDes = componentObject.GetState_Name().GetState_Text()
                            };

                            if (!DestNets[pad.GetState_Net().GetState_Name()].Contains(tmpNet))
                                DestNets[pad.GetState_Net().GetState_Name()].Add(tmpNet);
                        }
                        else
                        {
                            DestNets.Add(pad.GetState_Net().GetState_Name(), new List<structNet>
                                        {
                                            new structNet {
                                                Pin = pad.GetState_Name(),
                                                RefDes = componentObject.GetState_Name().GetState_Text()
                                            }
                                        });
                        }
                    }
                    pinCount++;
                    pad = compIterator.NextPCBObject() as IPCB_Pad;
                }

                componentObject.GroupIterator_Destroy(ref compIterator);

                IPCB_PrimitiveParameters componentParameters = componentObject as IPCB_PrimitiveParameters;
                newComp.Parameters = new Dictionary<string, string>();
                for (int i = 0; i < componentParameters.Count(); i++)
                {
                    parameter = componentParameters.GetParameterByIndex(i);
                    parameterName = parameter.GetName();
                    if (parameter.GetValue() == null)
                        parameterValue = "null";
                    else
                        parameterValue = parameter.GetValue();

                    newComp.Parameters.Add(parameterName, parameterValue);
                }

                newComp.RefDes = componentObject.GetState_Name().GetState_Text();
                newComp.Footprint = componentObject.GetState_Pattern();
                newComp.PinCount = pinCount;
                newComp.Dupe = false;


                string dstKey;
                if (Destination.Components.ContainsRefdes(newComp.RefDes, out dstKey))
                {
                    st_IPCB_Component tmp = Destination.Components[dstKey];
                    tmp.Dupe = true;
                    Destination.Components[dstKey] = tmp;

                    for (int i = 0; i < SelectedDestRef.Count; i++)
                    {
                        if (SelectedDestRef[i] == newComp.RefDes)
                        {
                            SelectedDestRef[i] = tmp.RefDes + " (" + tmp.Footprint + ")";
                            break;
                        }
                    }

                    newComp.Dupe = true;
                    Destination.Components.Add(newComp.ID, newComp);
                    _Log.Debug(newComp.RefDes);
                }
                else
                    Destination.Components.Add(newComp.ID, newComp);

                if (newComp.Dupe)
                    SelectedDestRef.Add(newComp.RefDes + " (" + newComp.Footprint + ")");
                else
                    SelectedDestRef.Add(newComp.RefDes);


                //?componentObject.GetState_DescriptorString()
                //"SOIC Component U5FM-QT#94L9#-20.000MHz (5528.098mil,6358.425mil) on Top Layer"
                //? componentObject.GetState_DetailString()
                //"Component U5FM Comment:QT#94L9#-20.000MHz Footprint: QT194"
                //?componentObject.GetState_Layer()
                //componentObject.GetState_Pattern() Footprint
                //componentObject.GetState_SourceDesignator() Refdes 
                //componentObject.GetState_SourceFootprintLibrary() library
                //componentObject.GetState_SourceLibReference() corp number
                //componentObject.GetState_XLocation()
                //componentObject.GetState_YLocation()
                _Log.Debug(componentObject.GetState_DescriptorString());
                #endregion

                //Primitive.Export_ToParameters(ref RefDes);
            }
            componentObject = BoardIterator.NextPCBObject() as IPCB_Component;

        }
        //Iterator clean-up
        Board.BoardIterator_Destroy(ref BoardIterator);
    }

}

/// <summary>
/// Structure for a net.
/// </summary>
public struct structNet
{
    public string RefDes;
    public string Pin;

}

/// <summary>
/// Adds new functions to the component dictionary.
/// </summary>
public class cls_IPCB_Component : Dictionary<string, st_IPCB_Component>
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);

    public bool ContainsRefdes(string RefDes, out string Key)
    {
        //_Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);
        if (RefDes == null)
        {
            Key = null;
            return false;
        }
        string Footprint = "";
        if (RefDes.Contains(" "))
        {
            string[] tmp = RefDes.Split(' ');
            RefDes = tmp[0];
            Footprint = tmp[1].Replace("(", "").Replace(")", "");
        }

        foreach (KeyValuePair<string, st_IPCB_Component> item in this)
            if (Footprint == "")
            {
                if (item.Value.RefDes == RefDes)
                {
                    Key = item.Key;
                    return true;
                }
            }
            else
            {
                if (item.Value.RefDes == RefDes && item.Value.Footprint == Footprint)
                {
                    Key = item.Key;
                    return true;
                }
            }
        Key = null;
        return false;
    }


    public st_IPCB_Component GetComponent(string RefDes, string Footprint = "")
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        foreach (st_IPCB_Component item in this.Values)
        {
            if (Footprint == "")
            {
                if (item.RefDes == RefDes)
                    if (item.Dupe) return null;
                    else return item;
            }
            else
            {
                if (item.Footprint == Footprint && item.RefDes == RefDes) return item;
            }
        }

        return null;
    }


    public List<string> GetRefDesKeys(string RefDes)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        List<string> output = new List<string>();

        foreach (KeyValuePair<string, st_IPCB_Component> item in this)
            if (item.Value.RefDes == RefDes)
                output.Add(item.Key);

        return output;

    }

    public bool DuplicateRef(string RefDes)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        int cnt = 0;
        foreach (KeyValuePair<string, st_IPCB_Component> item in this)
            if (item.Value.RefDes == RefDes) cnt++; ;

        if (cnt > 1) return true;
        return false;

    }
}

/// <summary>
/// Component structure.
/// </summary>
public class st_IPCB_Component
{
    public string RefDes;
    public string Footprint;
    public bool Dupe;
    public string ID;
    public Dictionary<string, string> Parameters;

    public Dictionary<string, string> Nets;

    public int PinCount;

}

/// <summary>
/// Used to store easily accessable info for altium primitives.
/// </summary>
public class clsOutput
{
    public List<st_IPCB_Arc> Arcs = new List<st_IPCB_Arc>();
    //public Dictionary<string, st_IPCB_Component> Components = new Dictionary<string, st_IPCB_Component>();
    public cls_IPCB_Component Components = new cls_IPCB_Component();
    public List<st_IPCB_Fill> Fills = new List<st_IPCB_Fill>();
    public List<st_IPCB_Pad> Pads = new List<st_IPCB_Pad>();
    public List<st_IPCB_Polygon> Polygons = new List<st_IPCB_Polygon>();
    public List<st_IPCB_Primitive> Primitives = new List<st_IPCB_Primitive>();
    public List<st_IPCB_Text> Text = new List<st_IPCB_Text>();
    public List<st_IPCB_Track> Tracks = new List<st_IPCB_Track>();
    public List<st_IPCB_Via> Vias = new List<st_IPCB_Via>();
    public List<st_IPCB_Region> Regions = new List<st_IPCB_Region>();


    #region Structs


    public struct st_IPCB_Arc
    {
        public int StartX, StartY, EndX, EndY, CenterX, CenterY, LineWidth, PasteMaskExpansion, Radius, SolderMaskExpansion;
        public double StartAngle, EndAngle;
        /// <summary>
        /// 1 = true, 0 = false
        /// </summary>
        public bool KeepOut;
        public string Layer;
        public string Net;
    }

    public struct st_IPCB_Pad
    {

    }

    public struct st_IPCB_Via
    {
        public int HoleSize, Size, SolderMaskExpansion, XLocation, YLocation;
        public bool IsTestPoint_Bottom, IsTestPoint_Top, IsTenting, IsTenting_Bottom, IsTenting_Top, TearDrop;
        public string Net, HighLayer, LowLayer;
    }

    public struct st_IPCB_Track
    {
        public int X1, X2, Y1, Y2, Width;

        public string Net, Layer;
        public bool Keepout;

    }

    public struct st_IPCB_Text
    {

    }

    public struct st_IPCB_Fill
    {
        public int Length, LocationX, LocationY, PasteMaskExpansion, SolderMaskExpansion, Width, X1Location, X2Location, Y1Location, Y2Location, XLocation, YLocation;
        public double Rotation;
        public string Net, Layer;
        public bool Keepout;
    }

    public struct st_IPCB_Polygon
    {
        public int Length, PasteMaskExpansion, SolderMaskExpansion, XLocation, YLocation, TrackSize;
        public string Net, Layer;
        public bool Keepout, MitreCorners;
        public List<PCB.SPolySegment> PolySegments;
        public PCB.TPolygonType PolyType;
    }

    public struct st_IPCB_Region
    {
        public int PasteMaskExpansion, SolderMaskExpansion;
        public string Net, Layer;
        public bool Keepout;
        public List<List<st_Contour>> GeometricPolygons;
        public PCB.TRegionKind RegionKind;

    }

    public struct st_Contour
    {
        public int X, Y;
    }

    public struct st_IPCB_Primitive
    {

    }
    #endregion

}
//switch (Primitive.GetState_ObjectID())
//                {
//                    case TObjectId.eNoObject:
//                        _Log.Debug(Primitive.GetState_DescriptorString());
//                        break;
//                    case TObjectId.eArcObject:
//                        IPCB_Arc arcObject = Primitive as IPCB_Arc;
//_Log.Debug(Primitive.GetState_DescriptorString());
//                        break;
//                    case TObjectId.ePadObject:
//                        IPCB_Pad padObject = Primitive as IPCB_Pad;
//_Log.Debug(Primitive.GetState_DescriptorString());
//                        break;
//                    case TObjectId.eViaObject:
//                        _Log.Debug(Primitive.GetState_DescriptorString());
//                        IPCB_Via ViaObject = Primitive as IPCB_Via;
//SelectedObjects.Add(new PCBObject()
//{
//    objectID = "eViaObject",
//                            holeSize = ViaObject.GetState_HoleSize(),
//                            x1 = ViaObject.GetState_XLocation(),
//                            y1 = ViaObject.GetState_YLocation(),
//                            Layer1 = ViaObject.GetState_StartLayer().GetState_LayerName(),
//                            Layer2 = ViaObject.GetState_StopLayer().GetState_LayerName(),

//                        });
//                        break;
//                    case TObjectId.eTrackObject:
//                        IPCB_Track trackObject = Primitive as IPCB_Track;
////?trackObject.GetState_IsKeepout()

////trackObject.GetState_Layer()
////trackObject.GetState_Net()
////trackObject.GetState_Width()
////trackObject.GetState_X1(), X2, Y1, Y2
//_Log.Debug(Primitive.GetState_DescriptorString());
//                        break;
//                    case TObjectId.eTextObject:
//                        IPCB_Text textObject = Primitive as IPCB_Text;
//_Log.Debug(Primitive.GetState_DescriptorString());
//                        break;
//                    case TObjectId.eFillObject:
//                        IPCB_Fill fillObject = Primitive as IPCB_Fill;
//_Log.Debug(Primitive.GetState_DescriptorString());
//                        break;
//                    //case TObjectId.eConnectionObject:
//                    //    _Log.Debug(Primitive.GetState_DescriptorString());
//                    //    break;
//                    //case TObjectId.eNetObject:
//                    //    _Log.Debug(Primitive.GetState_DescriptorString());
//                    //    break;
//                    case TObjectId.eComponentObject:
//                        IPCB_Component componentObject = Primitive as IPCB_Component;
////?componentObject.GetState_DescriptorString()
////"SOIC Component U5FM-QT#94L9#-20.000MHz (5528.098mil,6358.425mil) on Top Layer"
////? componentObject.GetState_DetailString()
////"Component U5FM Comment:QT#94L9#-20.000MHz Footprint: QT194"
////?componentObject.GetState_Layer()
////componentObject.GetState_Pattern() Footprint
////componentObject.GetState_SourceDesignator() Refdes
////componentObject.GetState_SourceFootprintLibrary() library
////componentObject.GetState_SourceLibReference() corp number
////componentObject.GetState_XLocation()
////componentObject.GetState_YLocation()
//_Log.Debug(componentObject.GetState_DescriptorString());
//                        break;
//                    case TObjectId.ePolyObject:
//                        IPCB_Polygon polygonObject = Primitive as IPCB_Polygon;
////?polygonObject.GetState_DetailString() Net
////?polygonObject.GetState_RemoveDead()
//_Log.Debug(polygonObject.GetState_DescriptorString());
//                        break;
//                    //case TObjectId.eRegionObject:
//                    //    _Log.Debug(Primitive.GetState_DescriptorString());
//                    //    break;
//                    //case TObjectId.eComponentBodyObject:
//                    //    _Log.Debug(Primitive.GetState_DescriptorString());
//                    //    break;
//                    //case TObjectId.eDimensionObject:
//                    //    _Log.Debug(Primitive.GetState_DescriptorString());
//                    //    break;
//                    //case TObjectId.eCoordinateObject:
//                    //    _Log.Debug(Primitive.GetState_DescriptorString());
//                    //    break;
//                    //case TObjectId.eClassObject:
//                    //    _Log.Debug(Primitive.GetState_DescriptorString());
//                    //    break;
//                    //case TObjectId.eRuleObject:
//                    //    _Log.Debug(Primitive.GetState_DescriptorString());
//                    //    break;
//                    //case TObjectId.eFromToObject:
//                    //    _Log.Debug(Primitive.GetState_DescriptorString());
//                    //    break;
//                    //case TObjectId.eDifferentialPairObject:
//                    //    _Log.Debug(Primitive.GetState_DescriptorString());
//                    //    break;
//                    //case TObjectId.eViolationObject:
//                    //    _Log.Debug(Primitive.GetState_DescriptorString());
//                    //    break;
//                    case TObjectId.eEmbeddedObject:
//                        _Log.Debug(Primitive.GetState_DescriptorString());
//                        break;
//                    case TObjectId.eEmbeddedBoardObject:
//                        _Log.Debug(Primitive.GetState_DescriptorString());
//                        break;
//                    //case TObjectId.eSplitPlaneObject:
//                    //    _Log.Debug(Primitive.GetState_DescriptorString());
//                    //    break;
//                    case TObjectId.eTraceObject:
//                        _Log.Debug(Primitive.GetState_DescriptorString());
//                        break;
//                    case TObjectId.eSpareViaObject:
//                        _Log.Debug(Primitive.GetState_DescriptorString());
//                        break;
//                    //case TObjectId.eBoardObject:
//                    //    _Log.Debug(Primitive.GetState_DescriptorString());
//                    //    break;
//                    //case TObjectId.eBoardOutlineObject:
//                    //    _Log.Debug(Primitive.GetState_DescriptorString());
//                    //    break;
//                    default:
//                        _Log.Debug(Primitive.GetState_DescriptorString());
//                        break;
//                }


//[XmlType("PCBObject")]
//public class PCBObject
//{
//                                    // |Via   
//    public string objectID;         // |
//                                    // |
//    public string Net;              // |
//                                    // |
//    public int x1, y1, x2, y2;      // |x1,x2
//    public int radius;              // |
//    public int viaSize;             // |
//    public int holeSize;            // |
//    public string Layer1;           // |Start Layer
//    public string Layer2;           // |Stop Layer
//}

/// <summary>
/// Contains lists of all selected objects as Altium objects.
/// </summary>
public class clsSelectedObjects
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);

    public List<IPCB_Arc> arcObjects;
    public List<IPCB_Pad> padObjects;
    public List<IPCB_Via> ViaObjects;
    public List<IPCB_Track> trackObjects;
    public List<IPCB_Text> textObjects;
    public List<IPCB_Fill> fillObjects;
    public List<IPCB_Component> componentObjects;
    public List<IPCB_Polygon> polygonObjects;
    public List<IPCB_Primitive> primitiveObjects;
    public List<IPCB_Region> regionObjects;

    public clsSelectedObjects()
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        arcObjects = new List<IPCB_Arc>();
        padObjects = new List<IPCB_Pad>();
        ViaObjects = new List<IPCB_Via>();
        trackObjects = new List<IPCB_Track>();
        textObjects = new List<IPCB_Text>();
        fillObjects = new List<IPCB_Fill>();
        componentObjects = new List<IPCB_Component>();
        polygonObjects = new List<IPCB_Polygon>();
        primitiveObjects = new List<IPCB_Primitive>();
        regionObjects = new List<IPCB_Region>();
    }

    internal IPCB_Component GetComponent(string ID)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);


        foreach (IPCB_Component item in componentObjects)
        {
            if (item.GetState_UniqueId() == ID)
                return item;
        }
        return null;
    }
}
