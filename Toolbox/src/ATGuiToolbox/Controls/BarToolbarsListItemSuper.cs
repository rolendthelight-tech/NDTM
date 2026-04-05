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
    public class BarToolbarsListItemSuper : BarLinkContainerItem
    {
        //private BarButtonItem item;
        //private bool showCustomizationItem;
        //private bool showDockPanels;
        //private bool showToolbars;

        public BarToolbarsListItemSuper()
        {
            //this.showDockPanels = false;
            //this.showToolbars = true;
            //this.showCustomizationItem = true;
            //this.item = new BarButtonItem(null, true);
            //this.item.ItemClick += new ItemClickEventHandler(this.onItemClick);
        }

        internal BarToolbarsListItemSuper(bool isPrivateItem, BarManager manager)
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
                    if (((/*this.ShowToolbars &&*/ !bar.IsMainMenu) /*&& !bar.OptionsBar.DisableClose*/) /*&& (!bar.OptionsBar.Hidden || base.Manager.IsDesignMode)*/)
                    {
                        text = bar.Text;
                        BarButtonItem item = new BarButtonItem(base.Manager, text/*, true*/)
                        {
                            //Caption = text,
                            Tag = bar,
                            ButtonStyle = BarButtonStyle.Check,
                            Down = ((IDockableObject) bar).IsVisible
                        };
                        item.ItemClick += new ItemClickEventHandler(this.onItemClick);
                        this.AddItem(item);
                    }
                }
                if ((base.Manager.DockManager != null) /*&& this.ShowDockPanels*/)
                {
                    foreach (DockPanel panel in base.Manager.DockManager.Panels)
                    {
                        if (((panel.Count <= 0) /*&& (panel.Visibility != DockVisibility.AutoHide)*/) /*&& ((panel.RootPanel == null) || (panel.RootPanel.Visibility != DockVisibility.AutoHide))*/)
                        {
                          BarButtonItem item2 = new BarButtonItem(base.Manager, (panel.TabText != "") ? panel.TabText : panel.Text /*, true*/)
                            {
                              //Caption = (panel.TabText != "") ? panel.TabText : panel.Text,
                              Tag = panel,
                              ButtonStyle = BarButtonStyle.Check,
                              Down = (panel.Visibility != DockVisibility.Hidden) && ((panel.RootPanel != null) && (panel.RootPanel.Visibility != DockVisibility.Hidden))
                            };
                            item2.ItemClick += new ItemClickEventHandler(this.onItemClick);
                            this.AddItem(item2);
                        }
                    }
                }
                //if (base.Manager.AllowCustomization && this.ShowCustomizationItem)
                //{
                //    BarItemLink link = this.AddItem(this.item);
                //    link.UserCaption = base.Manager.GetString(BarString.CustomizeButton);
                //    link.BeginGroup = true;
                //}
            }
            finally
            {
                base.CancelUpdate();
            }
            base.OnGetItemData();
        }

        private void onItemClick(object sender, ItemClickEventArgs e)
        {
            //if (e.Item == this.item)
            //{
            //    base.Manager.Customize();
            //}
            //else
            {
                bool down = ((BarButtonItem) e.Item).Down;
                DockPanel tag = e.Item.Tag as DockPanel;
                if (tag != null)
                {
                    if (!down)
                    {
                      tag.Hide();
                      tag.Visibility = DockVisibility.Hidden;
                    }
                    else
                    {
                      tag.Show();
                      tag.Visibility = DockVisibility.Visible;
                    }
                }
                else
                {
                    IDockableObject obj2 = e.Item.Tag as IDockableObject;
                    if (obj2 != null)
                    {
                        obj2.IsVisible = down;
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
    }

}

