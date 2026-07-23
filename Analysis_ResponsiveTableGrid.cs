using System;
using System.Linq;
using System.Windows.Forms;

namespace c2flux
{
    // Responsive AntdUI table for the Analysis view.
    // Layout, row height, header height, colors, font and cell spacing are applied centrally.
    public class Analysis_ResponsiveTableGrid : AntdUI.Table
    {
        public Analysis_ResponsiveTableGrid()
        {
            Dock = DockStyle.Fill;
            FixedHeader = true;
            VisibleHeader = true;
            EnableHeaderResizing = true;
            ColumnDragSort = false;
            MultipleRows = false;
            LostFocusClearSelection = false;
            MouseClickPenetration = true;
            ScrollBarAvoidHeader = true;
            AutoSizeColumnsMode = AntdUI.ColumnsMode.Fill;
            ShowTip = true;
            EmptyHeader = true;
            EmptyText = string.Empty;

            ApplyAntdUIStyle();
        }

        public void SetResponsiveColumns(
            params (string ColumnName, int Percentage)[] responsiveColumns)
        {
            if (responsiveColumns == null || Columns == null)
                return;

            foreach ((string ColumnName, int Percentage) definition in responsiveColumns)
            {
                AntdUI.Column column = Columns.FirstOrDefault(
                    currentColumn => string.Equals(
                        currentColumn.Key,
                        definition.ColumnName,
                        StringComparison.Ordinal));

                if (column == null)
                    continue;

                column.Width = $"{Math.Max(0, definition.Percentage)}%";
            }

            LoadLayout();
            Invalidate();
        }

        public void ApplyAntdUIStyle()
        {
            AntdThemeService.ConfigureAnalysisTable(this);
            LoadLayout();
            Invalidate();
        }
    }
}
