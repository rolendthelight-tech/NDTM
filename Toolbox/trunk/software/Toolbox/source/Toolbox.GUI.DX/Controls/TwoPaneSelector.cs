using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.XtraEditors;
using JetBrains.Annotations;

namespace Toolbox.GUI.DX.Controls
{
  /// <summary>
  /// Контрол для выбора набора значений из списка и установки порядка
  /// Оперирует только строковыми значениями
  /// </summary>
  public partial class TwoPaneSelector : XtraUserControl
  {
    //TODO: Локализация

    #region Item Class -----------------------------------------------------------------------------------------------------

    /// <summary>
    /// Класс-заглушка для строки — строка + порядок в списке
    /// </summary>
    internal class Item
    {
      /// <summary>
      /// Порядок в списке
      /// </summary>
      public int Order { get; set; }

      /// <summary>
      /// Строка
      /// </summary>
      public string Caption { get; set; }

      /// <summary>
      /// Функция для сравнения по порядку
      /// </summary>
      /// <param name="i1">Элемент 1</param>
      /// <param name="i2">Элемент 2</param>
      /// <returns>Сравнение порядков следования</returns>
      internal static int CompareItems([NotNull] Item i1, [NotNull] Item i2)
      {
	      if (i1 == null) throw new ArgumentNullException("i1");
	      if (i2 == null) throw new ArgumentNullException("i2");

				return i1.Order.CompareTo(i2.Order);
      }
    }

    #endregion

    #region Private Variables ---------------------------------------------------------------------------------------------

    /// <summary>
    /// Полный список элементов
    /// </summary>
    private readonly List<Item> m_full_items;

    /// <summary>
    /// Выбранй отсортированый список элементов
    /// </summary>
    private readonly List<Item> m_selected_items;

    #endregion

    public TwoPaneSelector()
    {
      InitializeComponent();

      m_full_items = new List<Item>();
      m_selected_items = new List<Item>();

      m_full_list.DataSource = m_full_items;
      m_selected_list.DataSource = m_selected_items;

      m_selected_list.DisplayMember = "Caption";
      m_selected_list.ValueMember = "Caption";

      m_full_list.DisplayMember = "Caption";
      m_full_list.ValueMember = "Caption";
    }

    #region Public Properties ---------------------------------------------------------------------------------------------

    /// <summary>
    /// Название выбраного списка
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public string SelectedGroupName
    {
      get { return m_selected_list_caption.Text; }
      set { m_selected_list_caption.Text = value; }
    }

    /// <summary>
    /// Название полного списка
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public string FullGroupName
    {
      get { return m_full_list_caption.Text; }
      set { m_full_list_caption.Text = value; }
    }

    /// <summary>
    /// Выбраные и упорядоченые пользователем элементы
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public List<string> Selected
    {
      get
      {
        var ret_val = new List<string>();

        m_selected_items.Sort(Item.CompareItems);

        foreach (Item itm in m_selected_items)
          ret_val.Add(itm.Caption);

        return ret_val;
      }
    }

    /// <summary>
    /// Полный список элементов для выборки
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public List<string> Full
    {
      get
      {
        var ret_val = new List<string>();

        foreach (Item itm in m_full_items)
          ret_val.Add(itm.Caption);

        return ret_val;
      }
      set
      {
        m_full_items.Clear();
        m_selected_items.Clear();

        foreach (string s in value)
        {
          Item itm = new Item {Caption = s};
          m_full_items.Add(itm);
          itm.Order = m_full_items.Count;
        }
      }
    }

    #endregion

    #region Private Methods -----------------------------------------------------------------------------------------------

    private void HandleAdd(object sender, EventArgs e)
    {
      MoveItems(m_full_list, m_full_items, m_selected_items, true);
    }

    private void HandleAddAll(object sender, EventArgs e)
    {
      MoveItems(m_full_list, m_full_items, m_selected_items, false);
    }

    private void HandleRemove(object sender, EventArgs e)
    {
      MoveItems(m_selected_list, m_selected_items, m_full_items, true);
    }

    private void HandleRemoveAll(object sender, EventArgs e)
    {
      MoveItems(m_selected_list, m_selected_items, m_full_items, false);
    }

    private void HandleMoveUp(object sender, EventArgs e)
    {
      if (0 == m_selected_list.SelectedItems.Count)
        return;

      var selected = new List<Item>();

      foreach (object o in m_selected_list.SelectedItems)
        selected.Add(o as Item);

      selected.Sort(Item.CompareItems);

      int index = m_selected_items.IndexOf(selected[0]);

      if (index < 1)
        return;

      foreach (Item item in selected)
      {
        index = m_selected_items.IndexOf(item);
        item.Order--;
        m_selected_items[index - 1].Order++;
        m_selected_items.Sort(Item.CompareItems);
      }

      m_selected_list.ResetBindings();

      //Исправляем выделение
      for (int i = 0; i < m_selected_items.Count; i++)
        m_selected_list.SetSelected(i, false);

      foreach (Item item in selected)
        m_selected_list.SetSelected(m_selected_list.FindItem(item), true);

      m_selected_list.Invalidate();
    }

    private void HandleMoveDown(object sender, EventArgs e)
    {
      if (0 == m_selected_list.SelectedItems.Count)
        return;

      var selected = new List<Item>();

      foreach (object o in m_selected_list.SelectedItems)
        selected.Add(o as Item);

      selected.Sort(Item.CompareItems);

      int index = m_selected_items.IndexOf(selected[0]);

      if (index >= m_selected_items.Count)
        return;

      foreach (Item item in selected)
      {
        index = m_selected_items.IndexOf(item);
        item.Order++;
        m_selected_items[index + 1].Order--;
        m_selected_items.Sort(Item.CompareItems);
      }

      m_selected_list.ResetBindings();

      //Исправляем выделение
      for (int i = 0; i < m_selected_items.Count; i++)
        m_selected_list.SetSelected(i, false);

      foreach (Item item in selected)
        m_selected_list.SetSelected(m_selected_list.FindItem(item), true);

      m_selected_list.Invalidate();
    }

    private void MoveItems([NotNull] ListBoxControl src, [NotNull] List<Item> src_ds, [NotNull] List<Item> dst_ds, bool move_selected)
    {
	    if (src == null) throw new ArgumentNullException("src");
	    if (src_ds == null) throw new ArgumentNullException("src_ds");
	    if (dst_ds == null) throw new ArgumentNullException("dst_ds");

	    var selected = new List<Item>();

      if (move_selected)
      {
        if (0 == src.SelectedItems.Count)
          return;

        foreach (object o in src.SelectedItems)
          selected.Add(o as Item);
      }
      else
      {
        foreach (Item o in src_ds)
          selected.Add(o);
      }

      foreach (Item itm in selected)
      {
        src_ds.Remove(itm);
        dst_ds.Add(itm);
        itm.Order = dst_ds.Count;
      }

      m_selected_list.ResetBindings();
      m_full_list.ResetBindings();

      //Исправляем выделение
      for (int i = 0; i < m_selected_items.Count; i++)
        m_selected_list.SetSelected(i, false);

			m_selected_list.Invalidate();
			m_full_list.Invalidate();
    }

    #endregion
  }
}