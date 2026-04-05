using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AT.Toolbox.Base;
using AT.Toolbox.Dialogs;
using ATMapToolbox;
using ATMapToolbox.Geometry;
using ATMapToolbox.Interfaces;
using ATMapToolbox.Layers;
using ATMapToolbox.Tools;
using DevExpress.XtraBars;
using MapInfo.Windows.Dialogs;

namespace MapToolboxTest
{
  public partial class MapForm1 : LocalizableForm 
  {
    private MapDS m_data_source;

    public MapForm1()
    {
      InitializeComponent();

      CustomPointLayer lr = new CustomPointLayer( null, null );
      CustomPolylineLayer lr2 = new CustomPolylineLayer( null, null );

      MapLayerFactory.LayerTemplates.Add( lr.Type, lr );
      MapLayerFactory.LayerTemplates.Add(lr2.Type, lr2);

      m_data_source = new MapDS();
      m_ctl.DataSource = m_data_source;

      m_ctl.PrepareToolbar = PopulateToolbar;

      AddPointObjectTool tl = new AddPointObjectTool("AddPointObjectTool","Testa");
      tl.CreateObjectHandler = CreatePointObject;
      m_ctl.AddTool(tl);

      MoveSelectedPointObjectTool tl2 = new MoveSelectedPointObjectTool("MoveSelectedPointObjectTool", "Testa");
      m_ctl.AddTool(tl2);

      AddPolylineTool tl3 = new AddPolylineTool("AddPloyLineTool", "Test2");
      tl3.CreateObjectHandler = CreatePolylineObject;
      m_ctl.AddTool(tl3);
    }

    private void CreatePolylineObject( out IMappableObject obj )
    {
      MappableObjectStub2 st = new MappableObjectStub2(counter);
      m_data_source.m_data2.Add(st);
      obj = st;
      counter++;
    }

    private static int counter = 5;

    private void CreatePointObject( out IMappableObject obj )
    {
      MappableObjectStub st = new MappableObjectStub(counter);
      m_data_source.m_data.Add( st );
      obj = st;
      counter++;
    }

    private void PopulateToolbar( Bar tool_bar )
    {
      BarButtonItem itm = new BarButtonItem( tool_bar.Manager, "AddPoint");
      tool_bar.AddItem( itm );
      itm.ItemClick += new ItemClickEventHandler(itm_ItemClick);

      BarButtonItem itm2 = new BarButtonItem(tool_bar.Manager, "Move selected");
      tool_bar.AddItem(itm2);
      itm2.ItemClick += new ItemClickEventHandler(itm2_ItemClick);

      BarButtonItem itm3 = new BarButtonItem(tool_bar.Manager, "Add Polyline");
      tool_bar.AddItem(itm3);
      itm3.ItemClick += new ItemClickEventHandler(itm3_ItemClick);

      BarButtonItem itm5 = new BarButtonItem(tool_bar.Manager, "Connect");
      tool_bar.AddItem(itm5);
      itm5.ItemClick += new ItemClickEventHandler(itm5_ItemClick);

      BarCheckItem itm4 = new BarCheckItem(tool_bar.Manager,false);
      itm4.Caption = "Edit Polyline";
      tool_bar.AddItem(itm4);
      itm4.CheckedChanged += new ItemClickEventHandler(itm4_CheckedChanged);
    }

    private void itm5_ItemClick( object sender, ItemClickEventArgs e )
    {
      m_ctl.ConnectObjects("Test2", CreatePolylineObject);
    }

    void itm4_CheckedChanged(object sender, ItemClickEventArgs e)
    {
      BarCheckItem itm = sender as BarCheckItem;

      if (itm.Checked)
      {
        if (!m_ctl.BeginLayerObjectEdit("Test2"))
          itm.Checked = false;
      }
      else
        m_ctl.EndLayerGeometryEdit();
    }

    private void itm3_ItemClick( object sender, ItemClickEventArgs e )
    {
      m_ctl.SetActiveTool("AddPloyLineTool");
    }

    private void itm2_ItemClick( object sender, ItemClickEventArgs e )
    {
      m_ctl.SetActiveTool("MoveSelectedPointObjectTool");
    }

    void itm_ItemClick(object sender, ItemClickEventArgs e)
    {
      m_ctl.SetActiveTool( "AddPointObjectTool" );
    }

    private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      SettingsForm frm = new SettingsForm(  );
      frm.Show();
    }

    private void MapForm1_Shown(object sender, EventArgs e)
    {
     // MapLayerFactory.Preferences.Layers.Add(new MapLayerFactory.LayerInfo() { DisplayName = "test", Order = 0, Type = "PointLayer", SystemAlias = "Test"});

      m_ctl.LoadData( true );
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
      if( m_data_source.m_data.Count == 0 )
        return;

      //if (!m_ctl.SelectedObjects.Contains(m_data_source.m_data[0]))
      //  m_ctl.SelectedObjects.Add( m_data_source.m_data[0] ); 
      //else 
      //  m_ctl.SelectedObjects.Remove(m_data_source.m_data[0]);

    }

    private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
    {
      LayerControlDlg d = new LayerControlDlg(  );
      d.Map = m_ctl.MapControl.Map;
      d.ShowDialog(this);
      int a = 10;
    }
  }

  public class MapDS : ILayerDataSource
  {
    public  List<IMappableObject> m_data = new List<IMappableObject>();
    public List<IMappableObject> m_data2 = new List<IMappableObject>();

    public List<IMappableObject> QueryObjectsByRect( string object_type, RectangleF rect )
    {
      switch( object_type.ToUpper() )
      {
        case "TEST@A":
          return m_data;
        default:
          return m_data2;
      }
    }

    public IMappableObject FindObject( string object_type, int geo_id )
    {
      foreach( IMappableObject mappable_object in m_data )
      {
        if (geo_id == mappable_object.GeoID)
          return mappable_object;
      }

      foreach (IMappableObject mappable_object in m_data2)
      {
        if (geo_id == mappable_object.GeoID)
          return mappable_object;
      }

      return null; 
    }

    public void SaveObject( IMappableObject mappable_object )
    {

    }
  }

  internal class MappableObjectStub : IMappableObject 
  {
    private int m_id;

    public MappableObjectStub( int counter )
    {
      m_id = counter;
      StyleString = "mi;40;Red;" + m_id;
    }

    public double Longtitude
    {
      get;
      set;
    }

    public double Lattitude
    {
      get;
      set;
    }

    public string StyleString
    {
      get;
      set;
    }

    public int GeoID
    {
      get
      {
        return m_id ;
      }
    }

    public string TableName
    {
      get
      {
        return "Testa";
      }
    }

    public string Description
    {
      get;
      set;
    }

    public GeometryWrapper Geometry
    {
      get;
      set;
    }
  }

  internal class MappableObjectStub2 : IMappableObject
  {
    private int m_id;

    public MappableObjectStub2(int counter)
    {
      m_id = counter;
      StyleString = "2;2;Red;false";
    }

    public double Longtitude
    {
      get;
      set;
    }

    public double Lattitude
    {
      get;
      set;
    }

    public string StyleString
    {
      get;
      set;
    }

    public int GeoID
    {
      get
      {
        return m_id;
      }
    }

    public string TableName
    {
      get
      {
        return "Test2";
      }
    }

    public string Description
    {
      get;
      set;
    }

    public GeometryWrapper Geometry
    {
      get;
      set;
    }
  }
}