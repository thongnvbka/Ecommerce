using System;
using System.Drawing;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;

namespace Common.Helper
{
    public class ExcelHelper
    {
        public static void CreateHeaderTable(ExcelWorksheet sheet, int fromRow, int fromCol, int toRow, int toCol, object value)
        {
            CreateCellTable(sheet.Cells[fromRow, fromCol, toRow, toCol], value, ExcelHorizontalAlignment.Center, false, null, null, null, true);
        }

        public static void CreateHeaderTable(ExcelWorksheet sheet, int fromRow, int fromCol, int toRow, int toCol, object value, ExcelHorizontalAlignment align)
        {
            CreateCellTable(sheet.Cells[fromRow, fromCol, toRow, toCol], value, align, false, null, null, null, true);
        }

        public static void CreateHeaderTable(ExcelWorksheet sheet, int fromRow, int fromCol, int toRow, int toCol, object value,
            ExcelHorizontalAlignment align, bool merge)
        {
            CreateCellTable(sheet.Cells[fromRow, fromCol, toRow, toCol], value, align, merge, null, null, null, true);
        }

        public static void CreateHeaderTable(ExcelWorksheet sheet, int fromRow, int fromCol, int toRow, int toCol, object value,
            ExcelHorizontalAlignment align, bool merge, Color background)
        {
            CreateCellTable(sheet.Cells[fromRow, fromCol, toRow, toCol], value, align, merge, background, null, null, true);
        }

        public static void CreateHeaderTable(ExcelWorksheet sheet, int fromRow, int fromCol, int toRow, int toCol, object value,
            ExcelHorizontalAlignment align, bool merge, Color background, Color color)
        {
            CreateCellTable(sheet.Cells[fromRow, fromCol, toRow, toCol], value, align, merge, background, color, null, true);
        }

        public static void CreateHeaderTable(ExcelWorksheet sheet, int fromRow, int fromCol, int toRow, int toCol, object value,
            ExcelHorizontalAlignment align, bool merge, Color background, Color color, float size)
        {
            CreateCellTable(sheet.Cells[fromRow, fromCol, toRow, toCol], value, align, merge, background, color, size, true);
        }

        public static void CreateHeaderTable(ExcelWorksheet sheet, int fromRow, int fromCol, int toRow, int toCol, object value,
            ExcelHorizontalAlignment align, bool merge, Color background, Color color, float size, bool bold)
        {
            CreateCellTable(sheet.Cells[fromRow, fromCol, toRow, toCol], value, align, merge, background, color, size, bold);
        }

        public static void CreateHeaderTable(ExcelWorksheet sheet, int row, int col, object value)
        {
            CreateCellTable(sheet.Cells[row, col], value, ExcelHorizontalAlignment.Center, false, null, null, null, true);
        }

        public static void CreateHeaderTable(ExcelWorksheet sheet, int row, int col, object value, ExcelHorizontalAlignment align)
        {
            CreateCellTable(sheet.Cells[row, col], value, align, false, null, null, null, true);
        }

        public static void CreateHeaderTable(ExcelWorksheet sheet, int row, int col, object value, ExcelHorizontalAlignment align, bool merge)
        {
            CreateCellTable(sheet.Cells[row, col], value, align, merge, null, null, null, true);
        }

        public static void CreateHeaderTable(ExcelWorksheet sheet, int row, int col, object value, ExcelHorizontalAlignment align, bool merge,
            Color background)
        {
            CreateCellTable(sheet.Cells[row, col], value, align, merge, background, null, null, true);
        }

        public static void CreateHeaderTable(ExcelWorksheet sheet, int row, int col, object value, ExcelHorizontalAlignment align, bool merge,
            Color background, Color color)
        {
            CreateCellTable(sheet.Cells[row, col], value, align, merge, background, color, null, true);
        }

        public static void CreateHeaderTable(ExcelWorksheet sheet, int row, int col, object value, ExcelHorizontalAlignment align, bool merge,
            Color background, Color color, float size, bool bold)
        {
            CreateCellTable(sheet.Cells[row, col], value, align, merge, background, color, size, bold);
        }

        public static void CreateHeaderTable(ExcelWorksheet sheet, string range, object value)
        {
            CreateCellTable(sheet.Cells[range], value, ExcelHorizontalAlignment.Center, false, null, null, null, true);
        }

        public static void CreateHeaderTable(ExcelWorksheet sheet, string range, object value, ExcelHorizontalAlignment align, bool merge)
        {
            CreateCellTable(sheet.Cells[range], value, align, merge, null, null, null, true);
        }

        public static void CreateHeaderTable(ExcelWorksheet sheet, string range, object value, ExcelHorizontalAlignment align, bool merge,
            Color background)
        {
            CreateCellTable(sheet.Cells[range], value, align, merge, background, null, null, true);
        }

        public static void CreateHeaderTable(ExcelWorksheet sheet, string range, object value, ExcelHorizontalAlignment align, bool merge,
            Color background, Color color)
        {
            CreateCellTable(sheet.Cells[range], value, align, merge, background, color, null, true);
        }

        public static void CreateHeaderTable(ExcelWorksheet sheet, string range, object value, ExcelHorizontalAlignment align, bool merge,
            Color background, Color color, float size)
        {
            CreateCellTable(sheet.Cells[range], value, align, merge, background, color, size, true);
        }

        public static void CreateHeaderTable(ExcelWorksheet sheet, string range, object value, ExcelHorizontalAlignment align, bool merge,
            Color background, Color color, float size, bool bold)
        {
            CreateCellTable(sheet.Cells[range], value, align, merge, background, color, size, bold);
        }

        // Cell table
        public static void CreateCellTable(ExcelWorksheet sheet, int fromRow, int fromCol, int toRow, int toCol, object value)
        {
            CreateCellTable(sheet.Cells[fromRow, fromCol, toRow, toCol], value, ExcelHorizontalAlignment.Left, false, null, null, null);
        }

        public static void CreateCellTable(ExcelWorksheet sheet, int fromRow, int fromCol, int toRow, int toCol, object value, ExcelHorizontalAlignment align)
        {
            CreateCellTable(sheet.Cells[fromRow, fromCol, toRow, toCol], value, align, false, null, null, null);
        }

        public static void CreateCellTable(ExcelWorksheet sheet, int fromRow, int fromCol, int toRow, int toCol, object value,
            ExcelHorizontalAlignment align, bool merge)
        {
            CreateCellTable(sheet.Cells[fromRow, fromCol, toRow, toCol], value, align, merge, null, null, null);
        }

        public static void CreateCellTable(ExcelWorksheet sheet, int fromRow, int fromCol, int toRow, int toCol, object value,
            ExcelHorizontalAlignment align, bool merge, Color background)
        {
            CreateCellTable(sheet.Cells[fromRow, fromCol, toRow, toCol], value, align, merge, background, null, null);
        }

        public static void CreateCellTable(ExcelWorksheet sheet, int fromRow, int fromCol, int toRow, int toCol, object value,
            ExcelHorizontalAlignment align, bool merge, Color background, Color color)
        {
            CreateCellTable(sheet.Cells[fromRow, fromCol, toRow, toCol], value, align, merge, background, color, null);
        }

        public static void CreateCellTable(ExcelWorksheet sheet, int fromRow, int fromCol, int toRow, int toCol, object value,
            ExcelHorizontalAlignment align, bool merge, Color background, Color color, float size)
        {
            CreateCellTable(sheet.Cells[fromRow, fromCol, toRow, toCol], value, align, merge, background, color, size);
        }

        public static void CreateCellTable(ExcelWorksheet sheet, int fromRow, int fromCol, int toRow, int toCol, object value,
            ExcelHorizontalAlignment align, bool merge, Color background, Color color, float size, bool bold)
        {
            CreateCellTable(sheet.Cells[fromRow, fromCol, toRow, toCol], value, align, merge, background, color, size, bold);
        }

        public static void CreateCellTable(ExcelWorksheet sheet, int fromRow, int fromCol, int toRow, int toCol, object value,
            ExcelHorizontalAlignment align, bool merge, Color? background, Color? color, float? size, bool bold, bool wrapText)
        {
            CreateCellTable(sheet.Cells[fromRow, fromCol, toRow, toCol], value, align, merge, background, color, size, bold, wrapText);
        }

        public static void CreateCellTable(ExcelWorksheet sheet, int row, int col, object value)
        {
            CreateCellTable(sheet.Cells[row, col], value, ExcelHorizontalAlignment.Left, false, null, null, null);
        }

        public static void CreateCellTable(ExcelWorksheet sheet, int row, int col, object value, bool wrapText)
        {
            CreateCellTable(sheet.Cells[row, col], value, ExcelHorizontalAlignment.Left, false, null, null, null, false, wrapText);
        }

        public static void CreateCellTable(ExcelWorksheet sheet, int row, int col, object value, ExcelHorizontalAlignment align)
        {
            CreateCellTable(sheet.Cells[row, col], value, align, false, null, null, null);
        }

        public static void CreateCellTable(ExcelWorksheet sheet, int row, int col, object value, ExcelHorizontalAlignment align, bool merge)
        {
            CreateCellTable(sheet.Cells[row, col], value, align, merge, null, null, null);
        }

        public static void CreateCellTable(ExcelWorksheet sheet, int row, int col, object value, ExcelHorizontalAlignment align, bool merge,
            Color background)
        {
            CreateCellTable(sheet.Cells[row, col], value, align, merge, background, null, null);
        }

        public static void CreateCellTable(ExcelWorksheet sheet, int row, int col, object value, ExcelHorizontalAlignment align, bool merge,
            Color background, Color color)
        {
            CreateCellTable(sheet.Cells[row, col], value, align, merge, background, color, null);
        }

        public static void CreateCellTable(ExcelWorksheet sheet, int row, int col, object value, ExcelHorizontalAlignment align, bool merge,
            Color background, Color color, float size, bool bold)
        {
            CreateCellTable(sheet.Cells[row, col], value, align, merge, background, color, size, bold);
        }

        public static void CreateCellTable(ExcelWorksheet sheet, int row, int col, object value, ExcelHorizontalAlignment align, bool merge,
            Color? background, Color? color, float? size, bool bold, bool wrapText)
        {
            CreateCellTable(sheet.Cells[row, col], value, align, merge, background, color, size, bold, wrapText);
        }

        public static void CreateCellTable(ExcelWorksheet sheet, string range, object value)
        {
            CreateCellTable(sheet.Cells[range], value, ExcelHorizontalAlignment.Left, false, null, null, null);
        }

        public static void CreateCellTable(ExcelWorksheet sheet, string range, object value, ExcelHorizontalAlignment align, bool merge)
        {
            CreateCellTable(sheet.Cells[range], value, align, merge, null, null, null);
        }

        public static void CreateCellTable(ExcelWorksheet sheet, string range, object value, ExcelHorizontalAlignment align, bool merge,
            Color background)
        {
            CreateCellTable(sheet.Cells[range], value, align, merge, background, null, null);
        }

        public static void CreateCellTable(ExcelWorksheet sheet, string range, object value, ExcelHorizontalAlignment align, bool merge,
            Color background, Color color)
        {
            CreateCellTable(sheet.Cells[range], value, align, merge, background, color, null);
        }

        public static void CreateCellTable(ExcelWorksheet sheet, string range, object value, ExcelHorizontalAlignment align, bool merge,
            Color background, Color color, float size)
        {
            CreateCellTable(sheet.Cells[range], value, align, merge, background, color, size);
        }

        public static void CreateCellTable(ExcelWorksheet sheet, string range, object value, ExcelHorizontalAlignment align, bool merge,
            Color background, Color color, float size, bool bold)
        {
            CreateCellTable(sheet.Cells[range], value, align, merge, background, color, size, bold);
        }

        public static void CreateCellTable(ExcelWorksheet sheet, string range, object value, ExcelHorizontalAlignment align, bool merge,
            Color? background, Color? color, float? size, bool bold, bool wrapText)
        {
            CreateCellTable(sheet.Cells[range], value, align, merge, background, color, size, bold, wrapText);
        }

        public static void CreateCell(ExcelWorksheet sheet, int fromRow, int fromCol, int toRow, int toCol, object value, ExcelHorizontalAlignment align, bool merge,
            Color? background, Color? color, float? size, bool bold = false)
        {
            CreateCell(sheet.Cells[fromRow, fromCol, toRow, toCol], value, align, merge, background, color, size, bold);
        }

        public static void CreateCell(ExcelWorksheet sheet, int row, int col, object value, ExcelHorizontalAlignment align, bool merge,
            Color? background, Color? color, float? size, bool bold = false)
        {
            CreateCell(sheet.Cells[row, col], value, align, merge, background, color, size, bold);
        }

        public static void CreateCell(ExcelWorksheet sheet, string range, object value, ExcelHorizontalAlignment align, bool merge,
            Color? background, Color? color, float? size, bool bold = false)
        {
            CreateCell(sheet.Cells[range], value, align, merge, background, color, size, bold);
        }

        public static void AddImage(ExcelWorksheet sheet, string name, int rowIndex, int colIndex, Stream stream, int width, int height)
        {
            var image = new Bitmap(stream);
            var excelImage = sheet.Drawings.AddPicture(name, image);
            excelImage.From.Column = colIndex;
            excelImage.From.Row = rowIndex;
            excelImage.SetSize(width, height);
            excelImage.From.ColumnOff = Pixel2Mtu(2);
            excelImage.From.RowOff = Pixel2Mtu(2);
            excelImage.Border.LineStyle = eLineStyle.Solid;
            excelImage.Border.Fill.Color = Color.Black;
        }

        private static int Pixel2Mtu(int pixels)
        {
            return pixels * 9525;
        }

        public static void AddHeaderAndFooter(ExcelWorksheet sheet, eOrientation orientation = eOrientation.Landscape)
        {
            var headerPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Template\Images\HeaderVTI.jpg");
            var footerPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Template\Images\FooterVTI.jpg");

            var evenHeader = sheet.HeaderFooter.EvenHeader.InsertPicture(new FileInfo(headerPath), PictureAlignment.Left);
            if (orientation == eOrientation.Portrait)
            {
                evenHeader.Width = evenHeader.Width - (5 * evenHeader.Width / evenHeader.Height);
                evenHeader.Height = evenHeader.Height - 5;
            }
            else
            {
                evenHeader.Width = evenHeader.Width + (5 * evenHeader.Width / evenHeader.Height);
                evenHeader.Height = evenHeader.Height + 5;
            }

            var oddHeader = sheet.HeaderFooter.OddHeader.InsertPicture(new FileInfo(headerPath), PictureAlignment.Left);
            if (orientation == eOrientation.Portrait)
            {
                oddHeader.Width = oddHeader.Width - (5 * oddHeader.Width / oddHeader.Height);
                oddHeader.Height = oddHeader.Height - 5;
            }
            else
            {
                oddHeader.Width = oddHeader.Width + (5 * oddHeader.Width / oddHeader.Height);
                oddHeader.Height = oddHeader.Height + 5;
            }

            var evenFooter = sheet.HeaderFooter.EvenFooter.InsertPicture(new FileInfo(footerPath), PictureAlignment.Centered);
            if (orientation == eOrientation.Portrait)
            {
                evenFooter.Width = evenFooter.Width - (53 * evenFooter.Width / evenFooter.Height);
                evenFooter.Height = evenFooter.Height - 53;
            }
            else
            {
                evenFooter.Width = evenFooter.Width - (50 * evenFooter.Width / evenFooter.Height);
                evenFooter.Height = evenFooter.Height - 50;
            }

            var oddFooter = sheet.HeaderFooter.OddFooter.InsertPicture(new FileInfo(footerPath), PictureAlignment.Centered);
            if (orientation == eOrientation.Portrait)
            {
                oddFooter.Width = oddFooter.Width - (53 * oddFooter.Width / oddFooter.Height);
                oddFooter.Height = oddFooter.Height - 53;
            }
            else
            {
                oddFooter.Width = oddFooter.Width - (50 * oddFooter.Width / oddFooter.Height);
                oddFooter.Height = oddFooter.Height - 50;
            }

            sheet.View.PageLayoutView = true;
            sheet.PrinterSettings.TopMargin = 2.4M / 2.54M;
            sheet.PrinterSettings.Orientation = orientation;
        }

        private static void CreateCell(ExcelRange cell, object value, ExcelHorizontalAlignment align, bool merge, Color? background,
            Color? color, float? size, bool bold = false, bool wrapText = false)
        {
            cell.Merge = merge;
            cell.Value = value;
            cell.Style.HorizontalAlignment = align;
            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cell.Style.Font.Bold = bold;
            cell.Style.WrapText = wrapText;
            if (background != null)
            {
                var fill = cell.Style.Fill;
                fill.PatternType = ExcelFillStyle.Solid;
                fill.BackgroundColor.SetColor(background.Value);
            }
            if (color != null)
            {
                cell.Style.Font.Color.SetColor(color.Value);
            }
            if (size.HasValue)
            {
                cell.Style.Font.Size = size.Value;
            }
            cell.AutoFitColumns();
        }

        private static void CreateCellTable(ExcelRange cell, object value, ExcelHorizontalAlignment align, bool merge, Color? background,
            Color? color, float? size, bool bold = false, bool wrapText = false)
        {
            CreateCell(cell, value, align, merge, background, color, size, bold, wrapText);
            SetBorder(cell);
        }

        private static void SetBorder(ExcelRange range)
        {
            var border = range.Style.Border;
            border.Top.Style = border.Left.Style = border.Bottom.Style = border.Right.Style = ExcelBorderStyle.Thin;
        }

        #region Henry

        private static void SetBorder(CustomExcelStyle excelStyle, ExcelRange range)
        {
            var border = range.Style.Border;

            if (excelStyle.Border.HasValue)
            {
                border.Top.Style = excelStyle.Border.Value;
                border.Bottom.Style = excelStyle.Border.Value;
                border.Left.Style = excelStyle.Border.Value;
                border.Right.Style = excelStyle.Border.Value;
            }
            else
            {
                border.Top.Style = excelStyle.BorderTop.HasValue ? excelStyle.BorderTop.Value : ExcelBorderStyle.None;
                border.Bottom.Style = excelStyle.BorderBottom.HasValue ? excelStyle.BorderBottom.Value : ExcelBorderStyle.None;
                border.Left.Style = excelStyle.BorderLeft.HasValue ? excelStyle.BorderLeft.Value : ExcelBorderStyle.None;
                border.Right.Style = excelStyle.BorderRight.HasValue ? excelStyle.BorderRight.Value : ExcelBorderStyle.None;
            }
        }

        private static void CreateCell(ExcelRange cell, object value, CustomExcelStyle excelStyle)
        {
            cell.Merge = excelStyle.IsMerge;
            cell.Value = value;
            cell.Style.Font.Bold = excelStyle.IsBold;
            cell.Style.Font.Size = excelStyle.FontSize.HasValue ? excelStyle.FontSize.Value : 11;

            if (excelStyle.Color.HasValue)
                cell.Style.Font.Color.SetColor(excelStyle.Color.Value);

            if (excelStyle.BackgroundColor.HasValue)
            {
                var fill = cell.Style.Fill;
                fill.PatternType = ExcelFillStyle.Solid;
                fill.BackgroundColor.SetColor(excelStyle.BackgroundColor.Value);
            }

            cell.Style.HorizontalAlignment = excelStyle.HorizontalAlign.HasValue ? excelStyle.HorizontalAlign.Value : ExcelHorizontalAlignment.Center;
            cell.Style.VerticalAlignment = excelStyle.VerticleAlign.HasValue ? excelStyle.VerticleAlign.Value : ExcelVerticalAlignment.Center;

            if (excelStyle.IsWrapText.HasValue)
                cell.Style.WrapText = excelStyle.IsWrapText.Value;
            else
                cell.AutoFitColumns();            

            if(!string.IsNullOrEmpty(excelStyle.NumberFormat))
                cell.Style.Numberformat.Format = excelStyle.NumberFormat;
            cell.Calculate();
            SetBorder(excelStyle, cell);
        }


        public static void CreateCellTable(ExcelWorksheet sheet, string range, object value, CustomExcelStyle excelStyle)
        {
            ExcelRange cellRange = sheet.Cells[range];
            CreateCell(cellRange, value, excelStyle);
        }

        public static void CreateCellTable(ExcelWorksheet sheet, int fromRow, int fromCol, int toRow, int toCol, object value, CustomExcelStyle excelStyle)
        {
            CreateCell(sheet.Cells[fromRow, fromCol, toRow, toCol], value, excelStyle);
        }

        public static void CreateColumnAutofit(ExcelWorksheet sheet, int[] columnFitArray)
        {
            foreach (int item in columnFitArray)
            {
                sheet.Column(item).AutoFit();
            }
        }

        public static void CreateColumnAutofit(ExcelWorksheet sheet, int beginCol, int toCol)
        {
            for (int i = beginCol; i <= toCol; i++)
            {
                sheet.Column(i).AutoFit();
            }
        }
        #endregion
    }

    public class CustomExcelStyle
    {
        public ExcelBorderStyle? Border { get; set; }
        public ExcelBorderStyle? BorderTop { get; set; }
        public ExcelBorderStyle? BorderBottom { get; set; }
        public ExcelBorderStyle? BorderLeft { get; set; }
        public ExcelBorderStyle? BorderRight { get; set; }
        public Color? Color { get; set; }
        public float? FontSize { get; set; }
        public ExcelHorizontalAlignment? HorizontalAlign { get; set; }
        public ExcelVerticalAlignment? VerticleAlign;
        public bool IsMerge { get; set; }
        public Color? BackgroundColor { get; set; }
        public bool IsBold { get; set; }
        public bool? IsWrapText { get; set; }
        public string NumberFormat { get; set; }
    }
}