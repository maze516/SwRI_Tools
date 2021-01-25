using DXP;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public static class UI
{
    //UI.ApplyADUITheme(this);
    public static void ApplyADUITheme(Form form)
    {
        form.BackColor = DxpThemeManager.ColorPanelBody;
        form.ForeColor = DxpThemeManager.ColorDefaultFont;

        foreach (Control control in FlattenControlsTree(form))
        {
            control.BackColor = DxpThemeManager.ColorPanelBody;
            control.ForeColor = DxpThemeManager.ColorPanelFont;

            var buttonControl = control as Button;
            if (buttonControl != null)
            {
                buttonControl.FlatStyle = FlatStyle.Flat;
                buttonControl.FlatAppearance.BorderColor = DxpThemeManager.ColorBorder;
                buttonControl.FlatAppearance.BorderSize = 1;
                buttonControl.BackColor = GetThemeColor("Button");
                buttonControl.ForeColor = DxpThemeManager.ColorPanelFont;
            }

            var checkboxControl = control as CheckBox;
            if (checkboxControl != null)
            {
                checkboxControl.FlatStyle = FlatStyle.Flat;
                checkboxControl.FlatAppearance.BorderColor = DxpThemeManager.ColorBorder;
                checkboxControl.BackColor = DxpThemeManager.ColorMenuBackground;
                if (checkboxControl.Enabled)
                    checkboxControl.ForeColor = DxpThemeManager.ColorPanelFont;
                else
                    checkboxControl.ForeColor = DxpThemeManager.ColorDisabledFont;
            }

            //var txtControl = control as TextBox;
            //if (txtControl != null)
            //{
            //    txtControl.BackColor = DxpThemeManager.ColorMenuBackground;
            //    txtControl.ForeColor = DxpThemeManager.ColorPanelFont;

            //}

            //var containerControl = control as ContainerControl;
            //if (containerControl != null)
            //{
            //    containerControl.BackColor = DxpThemeManager.ColorMenuBackground;
            //    containerControl.ForeColor = DxpThemeManager.ColorPanelFont;

            //}

            var dataGrid = control as DataGridView;
            if (dataGrid != null)
            {
                dataGrid.BackColor = GetThemeColor("PlainArea");
                dataGrid.BackgroundColor = GetThemeColor("PlainArea");
                dataGrid.ForeColor = DxpThemeManager.ColorPanelFont;
                dataGrid.GridColor = GetThemeColor("GridLine");
                dataGrid.RowsDefaultCellStyle.BackColor = GetThemeColor("PlainArea");
                dataGrid.AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.Single;
                dataGrid.RowsDefaultCellStyle.SelectionBackColor = GetThemeColor("SelectedBack");
                dataGrid.RowsDefaultCellStyle.SelectionForeColor = GetThemeColor("SelectedFont");
                dataGrid.ColumnHeadersDefaultCellStyle.BackColor = GetThemeColor("Header");
                dataGrid.ColumnHeadersDefaultCellStyle.ForeColor = DxpThemeManager.ColorMenuFont;
            }

            var toolStrip = control as ToolStrip;
            //if (toolStrip != null)
            //{
            //    toolStrip.RenderMode = ToolStripRenderMode.Professional;
            //    toolStrip.Renderer = new PanelToolStripRender();
            //    toolStrip.BackColor = DxpThemeManager.ColorMenuBackground;

            //    foreach (var toolStripItem in toolStrip.Items)
            //    {
            //        if (toolStripItem.GetType() == typeof(ToolStripSeparator))
            //        {
            //            var toolStripSeparateor = toolStripItem as ToolStripSeparator;
            //            toolStripSeparateor.BackColor = DxpThemeManager.ColorMenuBackground;
            //            toolStripSeparateor.ForeColor = GetThemeColor("Border");
            //        }

            //        if (toolStripItem.GetType() == typeof(ToolStripTextBox))
            //        {
            //            var toolStripTextBox = toolStripItem as ToolStripTextBox;
            //            toolStripTextBox.BackColor = GetThemeColor("EditBoxBack");
            //        }
            //    }
            //}
        }
    }

    static IEnumerable<Control> FlattenControlsTree(Control c)
    {
        yield return c;

        foreach (Control o in c.Controls)
        {
            foreach (var oo in FlattenControlsTree(o))
                yield return oo;
        }
    }

    public static Color GetThemeColor(string colorName)
    {
        int argColor;
        GlobalVars.UIThemeManager.CurrentUITheme().GetThemeColor(colorName, out argColor);
        return IntToColor(argColor);
    }

    private static Color IntToColor(int color)
    {
        byte[] bytes = BitConverter.GetBytes(color);
        return Color.FromArgb((int)bytes[0], (int)bytes[1], (int)bytes[2]);
    }
}