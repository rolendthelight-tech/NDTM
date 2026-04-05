using DevExpress.XtraBars;

namespace AT.Toolbox.Controls
{
    using DevExpress.Utils.Serializing;
    using DevExpress.XtraBars.Docking;
    using DevExpress.XtraBars.Localization;
    using DevExpress.XtraBars.Utils;
    using System;
    using System.ComponentModel;

    [DesignTimeVisible(false), ToolboxItem(false)]
    public class BarToolbarsListItemPuper : BarLinkContainerItem
    {
        //private BarButtonItem item;
        //private bool showCustomizationItem;
        //private bool showDockPanels;
        //private bool showToolbars;

        public BarToolbarsListItemPuper()
        {
            //this.showDockPanels = false;
            //this.showToolbars = true;
            //this.showCustomizationItem = true;
            //this.item = new BarButtonItem(null, true);
            //this.item.ItemClick += new ItemClickEventHandler(this.onItemClick);
        }

        internal BarToolbarsListItemPuper(bool isPrivateItem, BarManager manager)
          : this()
        {
            base.fIsPrivateItem = true;
            base.Manager = manager;
            this.OnGetItemData();
        }

        protected override void Dispose(bool disposing)
        {
            //if (disposing)
            //{
            //    if (this.item != null)
            //    {
            //        this.item.ItemClick -= new ItemClickEventHandler(this.onItemClick);
            //        this.item.Dispose();
            //    }
            //    this.item = null;
            //}
            base.Dispose(disposing);
        }

        protected override void OnGetItemData()
        {
            this.BeginUpdate();
            try
            {
              this.ClearLinks();
              foreach (Bar bar in base.Manager.Bars)
              {
                string text = string.Empty;
                if (!bar.IsMainMenu)
                {
                  text = bar.Text;
                  BarListItem item = new BarListItem()
                    {
                      Caption = text,
                      Tag = bar,
                      ShowChecks = true,
                      Manager = this.Manager
                    };
                  item.Strings.AddRange(new object[]
                    {
                      "Скрыть",
                      "Автоматически скрывать",
                      "Закрепляемое",
                      "Плавающее"
                    });
                  item.DataIndex = ((IDockableObject)bar).IsVisible ? (((IDockableObject)bar).IsDragging ? 3 : 2) : 0; //TODO: Проверить
                  item.ListItemClick +=
                    (object sender, ListItemClickEventArgs e) =>
                    this.onListItemClick(sender, e, ((e.Index >= 0) && (e.Index < 4) //TODO: Проверить
                                                       ? ((e.Index >= 2)
                                                            ? ((e.Index >= 3) ? Action.Float : Action.Dock)
                                                            : ((e.Index >= 1) ? Action.AutoHide : Action.Hide))
                                                       : Action.None));
                  BarSubItem sub_item = new BarSubItem(this.Manager, text, new BarItem[] { item });
                  this.AddItem(sub_item);
                }
              }
              if (base.Manager.DockManager != null)
              {
                foreach (DockPanel panel in base.Manager.DockManager.Panels)
                {
                  if (panel.Count <= 0)
                  {
                    string text = (panel.TabText != "") ? panel.TabText : panel.Text;
                    BarListItem item2 = new BarListItem
                      {
                        Caption = text,
                        Tag = panel,
                        ShowChecks = true,
                        Manager = this.Manager
                      };
                    item2.Strings.AddRange(new object[]
                      {
                        "Скрыть",
                        "Автоматически скрывать",
                        "Закрепляемое",
                        "Плавающее"
                      });
                    switch (panel.Visibility)
                    {
                      case DockVisibility.Hidden:
                        item2.DataIndex = 0;
                        break;
                      case DockVisibility.AutoHide:
                        item2.DataIndex = 1;
                        break;
                      case DockVisibility.Visible:
                        item2.DataIndex = panel.Dock == DockingStyle.Float ? 3 : 2;
                        break;
                      default:
                        item2.DataIndex = -1;
                        break;
                    }
                    item2.ListItemClick +=
                      (object sender, ListItemClickEventArgs e) =>
                      this.onListItemClick(sender, e, ((e.Index >= 0) && (e.Index < 4)
                                                         ? ((e.Index >= 2)
                                                              ? ((e.Index >= 3) ? Action.Float : Action.Dock)
                                                              : ((e.Index >= 1) ? Action.AutoHide : Action.Hide))
                                                         : Action.None));
                    BarSubItem sub_item2 = new BarSubItem(this.Manager, text, new BarItem[] { item2 });
                    this.AddItem(sub_item2);
                  }
                }
              }
            }
            finally
            {
              base.CancelUpdate();
            }
            base.OnGetItemData();
        }

        private void onListItemClick(object sender, ListItemClickEventArgs e, Action action)
        {
          DockPanel tag = e.Item.Tag as DockPanel;
          if (tag != null)
          {
            switch(action)
            {
              case Action.Hide:
                tag.Hide();
                tag.Visibility = DockVisibility.Hidden;
                break;
              case Action.AutoHide:
                if (tag.Dock == DockingStyle.Float)
                {
                  tag.Restore();
                  if (tag.Dock == DockingStyle.Float)
                  {
                    if (tag.Parent == null)
                      tag.Parent = tag.RootPanel;
                    tag.Dock = DockingStyle.Fill; // Это не работает
                  }
                }
                //tag.Show();
                tag.Visibility = DockVisibility.AutoHide;
                break;
              case Action.Dock:
                if (tag.Dock == DockingStyle.Float)
                {
                  tag.Restore();
                  if (tag.Dock == DockingStyle.Float)
                  {
                    if (tag.Parent == null)
                      tag.Parent = tag.RootPanel;
                    tag.Dock = DockingStyle.Fill; // Это не работает
                  }
                }
                //tag.Show();
                tag.Visibility = DockVisibility.Visible;
                break;
              case Action.Float:
                tag.Visibility = DockVisibility.Visible;
                tag.MakeFloat();
                tag.Show();
                break;
            }
          }
          else
          {
            IDockableObject obj2 = e.Item.Tag as IDockableObject;
            if (obj2 != null)
            {
              switch (action)
              {
                case Action.Hide:
                  obj2.IsVisible = false;
                  break;
                case Action.AutoHide://TODO: Доделать!
                case Action.Dock:
                case Action.Float:
                  obj2.IsVisible = true;
                  break;
              }
            }
          }
        }

      protected override void OnManagerChanged()
        {
            base.OnManagerChanged();
            //if (this.item != null)
            //{
            //    this.item.Manager = base.Manager;
            //    if (base.Manager != null)
            //    {
            //        this.AddItem(this.item);
            //    }
            //}
        }

        [Browsable(false), XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
        public override BarItemLinkCollection ItemLinks
        {
            get
            {
                return base.ItemLinks;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override LinksInfo LinksPersistInfo
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        //[DefaultValue(true), Description("Gets or sets a value indicating whether \"Customize...\" is among the item's subitems list."), Category("Appearance")]
        //public virtual bool ShowCustomizationItem
        //{
        //    get
        //    {
        //        return this.showCustomizationItem;
        //    }
        //    set
        //    {
        //        this.showCustomizationItem = value;
        //    }
        //}

        //[Description("Gets or sets whether a list of the existing dock panels should be displayed."), DefaultValue(false), Category("Appearance")]
        //public virtual bool ShowDockPanels
        //{
        //    get
        //    {
        //        return this.showDockPanels;
        //    }
        //    set
        //    {
        //        this.showDockPanels = value;
        //    }
        //}

        //[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'ShowDockPanels' property instead of property 'ShowDockWindows'"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //public virtual bool ShowDockWindows
        //{
        //    get
        //    {
        //        return false;
        //    }
        //    set
        //    {
        //    }
        //}

        //[Category("Appearance"), Description("Determines whether to display the list of existing toolbars within a specific BarToolbarsListItemSuper."), DefaultValue(true)]
        //public virtual bool ShowToolbars
        //{
        //    get
        //    {
        //        return this.showToolbars;
        //    }
        //    set
        //    {
        //        this.showToolbars = value;
        //    }
        //}

      public enum Action
      {
        None,
        Hide,
        Dock,
        Float,
        AutoHide,
      }
    }

}

