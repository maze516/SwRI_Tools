using PCB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Windows.Forms;

public class PlaceReplicate
{
    public List<string> SelectedSourceRef, SelectedDestRef;
    public clsSelectedObjects selectedSourceObjects, selectedDestinationObjects;
    public clsOutput Source, Destination;
    public Dictionary<string, IPCB_Net> BoardNets;

    //[XmlArray("SelectedObjects")]
    //[XmlArrayItem("Object")]
    //List<PCBObject> SelectedObjects = new List<PCBObject>();


    public PlaceReplicate()
    {

        Source = new clsOutput();
        Destination = new clsOutput();
        BoardNets = new Dictionary<string, IPCB_Net>();
    }

    public void GetInitialParts()
    {
        try
        {
            SelectedSourceRef = new List<string>();
            selectedSourceObjects = new clsSelectedObjects();
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

            while (Primitive != null)
            {
                if (Primitive.GetState_Selected())
                {
                    switch (Primitive.GetState_ObjectID())
                    {
                        case TObjectId.eArcObject:
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

                            System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
                            break;
                        case TObjectId.ePadObject:
                            IPCB_Pad padObject = Primitive as IPCB_Pad;
                            selectedSourceObjects.padObjects.Add(padObject);
                            System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
                            break;
                        case TObjectId.eViaObject:
                            System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
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
                        case TObjectId.eTrackObject:
                            IPCB_Track trackObject = Primitive as IPCB_Track;
                            System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
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
                        case TObjectId.eTextObject:
                            IPCB_Text textObject = Primitive as IPCB_Text;
                            selectedSourceObjects.textObjects.Add(textObject);
                            System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
                            break;

                        case TObjectId.eFillObject:
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

                            System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
                            break;
                        //case TObjectId.eConnectionObject:
                        //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
                        //    break;
                        //case TObjectId.eNetObject:
                        //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
                        //    break;
                        case TObjectId.eComponentObject:
                            IPCB_Component componentObject = Primitive as IPCB_Component;
                            selectedSourceObjects.componentObjects.Add(componentObject);

                            clsOutput.st_IPCB_Component newComp = new clsOutput.st_IPCB_Component();

                            IPCB_GroupIterator compIterator = componentObject.GroupIterator_Create();

                            PCB.TObjectSet compFilterset = new PCB.TObjectSet();
                            compFilterset.Add(TObjectId.ePadObject);
                            compIterator.AddFilter_ObjectSet(compFilterset);
                            compIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet); //Filter all layers.

                            IPCB_Pad pad = compIterator.FirstPCBObject() as IPCB_Pad;
                            int pinCount = 0;
                            newComp.Nets = new SerializableDictionary<string, string>();
                            while (pad != null)
                            {
                                if (pad.GetState_Net() != null)
                                    if (!newComp.Nets.ContainsKey(pad.GetState_Name()))
                                        newComp.Nets.Add(pad.GetState_Name(), pad.GetState_Net().GetState_Name());

                                pinCount++;
                                pad = compIterator.NextPCBObject() as IPCB_Pad;
                            }

                            componentObject.GroupIterator_Destroy(ref compIterator);


                            newComp.Parameters = new SerializableDictionary<string, string>();

                            IPCB_PrimitiveParameters componentParameters = componentObject as IPCB_PrimitiveParameters;
                            for (int i = 0; i < componentParameters.Count(); i++)
                            {
                                IPCB_Parameter parameter = componentParameters.GetParameterByIndex(i);
                                newComp.Parameters.Add(parameter.GetName(), parameter.GetValue());
                            }


                            newComp.RefDes = componentObject.GetState_Name().GetState_Text();
                            newComp.Footprint = componentObject.GetState_Pattern();
                            newComp.PinCount = pinCount;

                            Source.Components.Add(newComp.RefDes, newComp);

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
                            System.Diagnostics.Debug.WriteLine(componentObject.GetState_DescriptorString());
                            break;
                        case TObjectId.ePolyObject:
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
                            System.Diagnostics.Debug.WriteLine(polygonObject.GetState_DescriptorString());
                            break;
                        case TObjectId.eRegionObject:

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

                            System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());

                            break;
                        //case TObjectId.eComponentBodyObject:
                        //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
                        //    break;
                        //case TObjectId.eDimensionObject:
                        //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
                        //    break;
                        //case TObjectId.eCoordinateObject:
                        //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
                        //    break;
                        //case TObjectId.eClassObject:
                        //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
                        //    break;
                        //case TObjectId.eRuleObject:
                        //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
                        //    break;
                        //case TObjectId.eFromToObject:
                        //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
                        //    break;
                        //case TObjectId.eDifferentialPairObject:
                        //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
                        //    break;
                        //case TObjectId.eViolationObject:
                        //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
                        //    break;
                        //case TObjectId.eEmbeddedObject:
                        //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
                        //    break;
                        //case TObjectId.eEmbeddedBoardObject:
                        //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
                        //    break;
                        //case TObjectId.eSplitPlaneObject:
                        //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
                        //    break;
                        //case TObjectId.eTraceObject:
                        //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
                        //    break;
                        //case TObjectId.eSpareViaObject:
                        //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
                        //    break;
                        //case TObjectId.eBoardObject:
                        //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
                        //    break;
                        //case TObjectId.eBoardOutlineObject:
                        //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
                        //    break;
                        default:
                            selectedSourceObjects.primitiveObjects.Add(Primitive);
                            System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
                            break;
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
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            return;
        }
    }

    public void GetNets()
    {
        IPCB_BoardIterator BoardIterator;
        IPCB_Net Net;
        BoardNets = new Dictionary<string, IPCB_Net>();

        IPCB_Board Board = Util.GetCurrentPCB();

        if (Board == null)
            return;

        //Iterate theough all components on the board.
        BoardIterator = Board.BoardIterator_Create();
        PCB.TObjectSet FilterSet = new PCB.TObjectSet();
        FilterSet.Add(TObjectId.eNetObject);
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
    public void GetDestinationParts()
    {

        SelectedDestRef = new List<string>();
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

                    case TObjectId.eComponentObject:
                        IPCB_Component componentObject = Primitive as IPCB_Component;
                        selectedDestinationObjects.componentObjects.Add(componentObject);

                        clsOutput.st_IPCB_Component newComp = new clsOutput.st_IPCB_Component();

                        IPCB_GroupIterator compIterator = componentObject.GroupIterator_Create();

                        PCB.TObjectSet compFilterset = new PCB.TObjectSet();
                        compFilterset.Add(TObjectId.ePadObject);
                        compIterator.AddFilter_ObjectSet(compFilterset);
                        compIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet); //Filter all layers.

                        IPCB_Pad pad = compIterator.FirstPCBObject() as IPCB_Pad;
                        int pinCount = 0;
                        newComp.Nets = new SerializableDictionary<string, string>();
                        while (pad != null)
                        {
                            if (pad.GetState_Net() != null)
                                if(!newComp.Nets.ContainsKey(pad.GetState_Name()))
                                newComp.Nets.Add(pad.GetState_Name(), pad.GetState_Net().GetState_Name());

                            pinCount++;
                            pad = compIterator.NextPCBObject() as IPCB_Pad;
                        }

                        componentObject.GroupIterator_Destroy(ref compIterator);

                        IPCB_PrimitiveParameters componentParameters = componentObject as IPCB_PrimitiveParameters;
                        newComp.Parameters = new SerializableDictionary<string, string>();
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

                        Destination.Components.Add(newComp.RefDes, newComp);

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
                        System.Diagnostics.Debug.WriteLine(componentObject.GetState_DescriptorString());
                        break;

                    default:
                        //selectedSourceObjects.primitiveObjects.Add(Primitive);
                        System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
                        break;
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

    public void SavePlacement()
    {
        XmlSerializer x = new XmlSerializer(selectedSourceObjects.GetType());
        TextWriter writer = new StreamWriter("C:\\Users\\rlyne\\Dropbox\\Altium Extensions\\SwRI_Tools\\bin\\test.txt");
        x.Serialize(writer, selectedSourceObjects);
    }

    /// <summary>
    /// Functions for performing common XML Serialization operations.
    /// <para>Only public properties and variables will be serialized.</para>
    /// <para>Use the [XmlIgnore] attribute to prevent a property/variable from being serialized.</para>
    /// <para>Object to be serialized must have a parameterless constructor.</para>
    /// </summary>
    public static class XmlSerialization
    {
        /// <summary>
        /// Writes the given object instance to an XML file.
        /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
        /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [XmlIgnore] attribute.</para>
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToXmlFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                writer = new StreamWriter(filePath, append);
                serializer.Serialize(writer, objectToWrite);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        /// <summary>
        /// Reads an object instance from an XML file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the XML file.</returns>
        public static T ReadFromXmlFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                reader = new StreamReader(filePath);
                return (T)serializer.Deserialize(reader);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
    }

}





public class clsOutput
{

    public List<st_IPCB_Arc> Arcs = new List<st_IPCB_Arc>();
    public SerializableDictionary<string, st_IPCB_Component> Components = new SerializableDictionary<string, st_IPCB_Component>();
    public List<st_IPCB_Fill> Fills = new List<st_IPCB_Fill>();
    public List<st_IPCB_Pad> Pads = new List<st_IPCB_Pad>();
    public List<st_IPCB_Polygon> Polygons = new List<st_IPCB_Polygon>();
    public List<st_IPCB_Primitive> Primitives = new List<st_IPCB_Primitive>();
    public List<st_IPCB_Text> Text = new List<st_IPCB_Text>();
    public List<st_IPCB_Track> Tracks = new List<st_IPCB_Track>();
    public List<st_IPCB_Via> Vias = new List<st_IPCB_Via>();
    public List<st_IPCB_Region> Regions = new List<st_IPCB_Region>();

    public struct st_IPCB_Component
    {
        public string RefDes;
        public string Footprint;
        public SerializableDictionary<string, string> Parameters;

        public SerializableDictionary<string, string> Nets;

        public int PinCount;

    }

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
}
//switch (Primitive.GetState_ObjectID())
//                {
//                    case TObjectId.eNoObject:
//                        System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
//                        break;
//                    case TObjectId.eArcObject:
//                        IPCB_Arc arcObject = Primitive as IPCB_Arc;
//System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
//                        break;
//                    case TObjectId.ePadObject:
//                        IPCB_Pad padObject = Primitive as IPCB_Pad;
//System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
//                        break;
//                    case TObjectId.eViaObject:
//                        System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
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
//System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
//                        break;
//                    case TObjectId.eTextObject:
//                        IPCB_Text textObject = Primitive as IPCB_Text;
//System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
//                        break;
//                    case TObjectId.eFillObject:
//                        IPCB_Fill fillObject = Primitive as IPCB_Fill;
//System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
//                        break;
//                    //case TObjectId.eConnectionObject:
//                    //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
//                    //    break;
//                    //case TObjectId.eNetObject:
//                    //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
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
//System.Diagnostics.Debug.WriteLine(componentObject.GetState_DescriptorString());
//                        break;
//                    case TObjectId.ePolyObject:
//                        IPCB_Polygon polygonObject = Primitive as IPCB_Polygon;
////?polygonObject.GetState_DetailString() Net
////?polygonObject.GetState_RemoveDead()
//System.Diagnostics.Debug.WriteLine(polygonObject.GetState_DescriptorString());
//                        break;
//                    //case TObjectId.eRegionObject:
//                    //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
//                    //    break;
//                    //case TObjectId.eComponentBodyObject:
//                    //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
//                    //    break;
//                    //case TObjectId.eDimensionObject:
//                    //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
//                    //    break;
//                    //case TObjectId.eCoordinateObject:
//                    //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
//                    //    break;
//                    //case TObjectId.eClassObject:
//                    //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
//                    //    break;
//                    //case TObjectId.eRuleObject:
//                    //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
//                    //    break;
//                    //case TObjectId.eFromToObject:
//                    //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
//                    //    break;
//                    //case TObjectId.eDifferentialPairObject:
//                    //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
//                    //    break;
//                    //case TObjectId.eViolationObject:
//                    //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
//                    //    break;
//                    case TObjectId.eEmbeddedObject:
//                        System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
//                        break;
//                    case TObjectId.eEmbeddedBoardObject:
//                        System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
//                        break;
//                    //case TObjectId.eSplitPlaneObject:
//                    //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
//                    //    break;
//                    case TObjectId.eTraceObject:
//                        System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
//                        break;
//                    case TObjectId.eSpareViaObject:
//                        System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
//                        break;
//                    //case TObjectId.eBoardObject:
//                    //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
//                    //    break;
//                    //case TObjectId.eBoardOutlineObject:
//                    //    System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
//                    //    break;
//                    default:
//                        System.Diagnostics.Debug.WriteLine(Primitive.GetState_DescriptorString());
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

public class clsSelectedObjects
{

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

    internal IPCB_Component GetComponent(string RefDes)
    {
        foreach (IPCB_Component item in componentObjects)
        {
            if (item.GetState_Name().GetState_Text() == RefDes)
                return item;
        }
        return null;
    }
}

[XmlRoot("Dictionary")]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
{
    #region IXmlSerializable Members
    public System.Xml.Schema.XmlSchema GetSchema()
    {
        return null;
    }
    public void ReadXml(System.Xml.XmlReader reader)
    {
        XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
        XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
        bool wasEmpty = reader.IsEmptyElement;
        reader.Read();
        if (wasEmpty)
            return;
        while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
        {
            reader.ReadStartElement("item");
            reader.ReadStartElement("key");
            TKey key = (TKey)keySerializer.Deserialize(reader);
            reader.ReadEndElement();
            reader.ReadStartElement("value");
            TValue value = (TValue)valueSerializer.Deserialize(reader);
            reader.ReadEndElement();
            this.Add(key, value);
            reader.ReadEndElement();
            reader.MoveToContent();
        }
        reader.ReadEndElement();
    }
    public void WriteXml(System.Xml.XmlWriter writer)
    {
        XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
        XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
        foreach (TKey key in this.Keys)
        {
            writer.WriteStartElement("item");
            writer.WriteStartElement("key");
            keySerializer.Serialize(writer, key);
            writer.WriteEndElement();
            writer.WriteStartElement("value");
            TValue value = this[key];
            valueSerializer.Serialize(writer, value);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }
    #endregion
}

