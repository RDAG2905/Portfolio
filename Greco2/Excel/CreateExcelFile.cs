using System;
using System.Collections.Generic;
using System.Linq;

using System.Data;
using System.Diagnostics;
using System.Globalization;

using System.Text.RegularExpressions;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Greco2.Excel
{
    public class CreateExcelFile
    {
        // This class was created using several articles below, code merged and improved to generate
// a spreadsheet from dataset using autosized columns, auto filter, alternating light blue background color and bold column headers.
// Also improved a lot code performance removing bottlenecks and using Row object cache.
// Daniel Liedke
//
//
//  Autofit Content:
//  https://social.msdn.microsoft.com/Forums/office/en-US/28aae308-55cb-479f-9b58-d1797ed46a73/solution-how-to-autofit-excel-content?forum=oxmlsdk
//
//  Coloring Cells:
//  https://social.msdn.microsoft.com/Forums/office/en-US/a973335c-9f9b-4e70-883a-02a0bcff43d2/coloring-cells-in-excel-sheet-using-openxml-in-c?forum=oxmlsdk
//
//  Date Formats:
//  https://stackoverflow.com/questions/2792304/how-to-insert-a-date-to-an-open-xml-worksheet
// 
//  Auto filter:
//  https://community.dynamics.com/crm/b/crmmitchmilam/archive/2010/11/04/openxml-worksheet-adding-autofilter
//
//  Font Bold:
//  https://stackoverflow.com/questions/29913094/how-to-make-excel-work-sheet-header-row-bold-using-openxml
//
//
//  Generating Spreadsheet from Dataset:
//  http://www.mikesknowledgebase.com/pages/CSharp/ExportToExcel.htm
//
//   (c) www.mikesknowledgebase.com 2014 
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files 
//   (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, 
//   publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
//   subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
//   MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
//   FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
//   WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//   



//namespace SpreadSheetGenerator
//    {
//        public class CreateExcelFile
//        {
            /// <summary>
            /// Create an Excel file, and write it to a file.
            /// </summary>
            /// <param name="ds">DataSet containing the data to be written to the Excel.</param>
            /// <param name="excelFilename">Name of file to be written.</param>
            /// <returns>True if successful, false if something went wrong.</returns>
            public static bool CreateExcelDocument(DataSet ds, string excelFilename, bool includeAutoFilter)
            {
                try
                {
                    using (SpreadsheetDocument document = SpreadsheetDocument.Create(excelFilename, SpreadsheetDocumentType.Workbook))
                    {
                        WriteExcelFile(ds, document, includeAutoFilter);
                    }
                    Trace.WriteLine("Successfully created: " + excelFilename);
                    return true;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Failed, exception thrown: " + ex.Message);
                    return false;
                }
            }

            private static void WriteExcelFile(DataSet ds, SpreadsheetDocument spreadsheet, bool includeAutoFilter)
            {
                // Reset rows cache
                _rowListCache = new List<Row>();

                // Add a WorkbookPart to the document.
                WorkbookPart workbookpart = spreadsheet.AddWorkbookPart();
                workbookpart.Workbook = new Workbook();

                // Add a WorksheetPart to the WorkbookPart.
                WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet();

                // add styles to sheet
                WorkbookStylesPart wbsp = workbookpart.AddNewPart<WorkbookStylesPart>();
                wbsp.Stylesheet = CreateStylesheet();
                wbsp.Stylesheet.Save();

                DataTable dt = ds.Tables[0];
                string worksheetName = dt.TableName;

                // Create columns calculating size of biggest text for the database column
                int numberOfColumns = dt.Columns.Count;
                Columns columns = new Columns();
                for (int colInx = 0; colInx < numberOfColumns; colInx++)
                {
                    DataColumn col = dt.Columns[colInx];

                    string maxText = col.ColumnName;
                    foreach (DataRow dr in dt.Rows)
                    {
                        string value = string.Empty;
                        if (col.DataType.FullName == "System.DateTime")
                        {
                            DateTime dtValue;
                            if (DateTime.TryParse(dr[col].ToString(), out dtValue))
                                value = dtValue.ToShortDateString();
                        }
                        else
                        {
                            value = dr[col].ToString();
                        }

                        if (value.Length > maxText.Length)
                        {
                            maxText = value;
                        }
                    }
                    //double width = GetWidth("Calibri", 11, maxText);
                    //columns.Append(CreateColumnData((uint)colInx + 1, (uint)colInx + 1, width + 2));
                columns.Append(CreateColumnData((uint)colInx + 1, (uint)colInx + 1, 15));
            }
                worksheetPart.Worksheet.Append(columns);

                // Create SheetData and assign to worksheetpart
                SheetData sd = new SheetData();
                worksheetPart.Worksheet.Append(sd);

                // Add Sheets to the Workbook.
                Sheets sheets = spreadsheet.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

                // Append a new worksheet and associate it with the workbook.
                Sheet sheet = new Sheet()
                {
                    Id = spreadsheet.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = worksheetName
                };
                sheets.Append(sheet);

                // Append this worksheet's data to our Workbook, using OpenXmlWriter, to prevent memory problems
                WriteDataTableToExcelWorksheet(dt, ref worksheetPart, includeAutoFilter);

                // Save it and close it
                spreadsheet.WorkbookPart.Workbook.Save();
                spreadsheet.Close();
            }

            private static void WriteDataTableToExcelWorksheet(DataTable dt, ref WorksheetPart worksheetPart, bool includeAutoFilter)
            {
                string cellValue = "";

                //  Create a Header Row in our Excel file, containing one header for each Column of data in our DataTable.
                //
                //  We'll also create an array, showing which type each column of data is (Text or Numeric), so when we come to write the actual
                //  cells of data, we'll know if to write Text values or Numeric cell values.
                int numberOfColumns = dt.Columns.Count;
                bool[] IsNumericColumn = new bool[numberOfColumns];
                bool[] IsDateColumn = new bool[numberOfColumns];

                string[] excelColumnNames = new string[numberOfColumns];
                for (int n = 0; n < numberOfColumns; n++)
                    excelColumnNames[n] = GetExcelColumnName(n);

                //
                //  Create the Header row in our Excel Worksheet
                //
                uint rowIndex = 1;

                for (int colInx = 0; colInx < numberOfColumns; colInx++)
                {
                    DataColumn col = dt.Columns[colInx];

                    // Save the cell data in spreadsheet
                    var cell = CreateSpreadsheetCellIfNotExist(worksheetPart.Worksheet, excelColumnNames[colInx] + rowIndex.ToString());
                    cell.CellValue = new CellValue(col.ColumnName);
                    cell.DataType = CellValues.String;
                    cell.StyleIndex = (UInt32Value)1U;

                    IsNumericColumn[colInx] = (col.DataType.FullName == "System.Decimal") || (col.DataType.FullName == "System.Int32") || (col.DataType.FullName == "System.Int64") || (col.DataType.FullName == "System.Double") || (col.DataType.FullName == "System.Single");
                    IsDateColumn[colInx] = (col.DataType.FullName == "System.DateTime");
                }

                // Set the AutoFilter property to a range that is the size of the data
                // within the worksheet
                if (includeAutoFilter)
                {
                    AutoFilter autoFilter1 = new AutoFilter()
                    { Reference = "A1:" + excelColumnNames[numberOfColumns - 1] + "1" };
                    worksheetPart.Worksheet.Append(autoFilter1);
                }

                //
                //  Now, step through each row of data in our DataTable...
                //
                double cellNumericValue = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    // ...create a new row, and append a set of this row's data to it.
                    ++rowIndex;

                    // Add Data
                    Cell cell;

                    for (int colInx = 0; colInx < numberOfColumns; colInx++)
                    {
                        cellValue = dr.ItemArray[colInx].ToString();
                        cellValue = ReplaceHexadecimalSymbols(cellValue);

                        // Create cell with data
                        if (IsNumericColumn[colInx])
                        {
                            //  For numeric cells, make sure our input data IS a number, then write it out to the Excel file.
                            //  If this numeric value is NULL, then don't write anything to the Excel file.
                            cellNumericValue = 0;
                            if (double.TryParse(cellValue, out cellNumericValue))
                            {
                                cellValue = cellNumericValue.ToString();
                            }
                            else
                            {
                                cellValue = null;
                            }

                            cell = CreateSpreadsheetCellIfNotExist(worksheetPart.Worksheet, excelColumnNames[colInx] + rowIndex.ToString());
                            cell.CellValue = new CellValue(cellValue);
                            cell.DataType = CellValues.Number;
                            cell.StyleIndex = (rowIndex % 2 == 0) ? (UInt32Value)0U : (UInt32Value)3U;
                        }
                        else if (IsDateColumn[colInx])
                        {
                            //  This is a date value.
                            DateTime dtValue;
                            string strValue = "";
                            if (DateTime.TryParse(cellValue, out dtValue))
                                strValue = dtValue.ToOADate().ToString(CultureInfo.InvariantCulture);

                            cell = CreateSpreadsheetCellIfNotExist(worksheetPart.Worksheet, excelColumnNames[colInx] + rowIndex.ToString());
                            cell.CellValue = new CellValue(strValue);
                            cell.DataType = new EnumValue<CellValues>(CellValues.Number);  //Date is only available in Office 2010
                            cell.StyleIndex = (rowIndex % 2 == 0) ? (UInt32Value)2U : (UInt32Value)4U;
                        }
                        else
                        {
                            //  For text cells, just write the input data straight out to the Excel file.
                            cell = CreateSpreadsheetCellIfNotExist(worksheetPart.Worksheet, excelColumnNames[colInx] + rowIndex.ToString());
                            cell.CellValue = new CellValue(cellValue);
                            cell.DataType = CellValues.String;
                            cell.StyleIndex = (rowIndex % 2 == 0) ? (UInt32Value)0U : (UInt32Value)3U;
                        }
                    }
                }

                worksheetPart.Worksheet.Save();
            }

            private static string ReplaceHexadecimalSymbols(string txt)
            {
                string r = "[\x00-\x08\x0B\x0C\x0E-\x1F\x26]";
                return Regex.Replace(txt, r, "", RegexOptions.Compiled);
            }

            //  Convert a zero-based column index into an Excel column reference  (A, B, C.. Y, Y, AA, AB, AC... AY, AZ, B1, B2..)
            public static string GetExcelColumnName(int columnIndex)
            {
                //  eg  (0) should return "A"
                //      (1) should return "B"
                //      (25) should return "Z"
                //      (26) should return "AA"
                //      (27) should return "AB"
                //      ..etc..
                char firstChar;
                char secondChar;
                char thirdChar;

                if (columnIndex < 26)
                {
                    return ((char)('A' + columnIndex)).ToString();
                }

                if (columnIndex < 702)
                {
                    firstChar = (char)('A' + (columnIndex / 26) - 1);
                    secondChar = (char)('A' + (columnIndex % 26));

                    return string.Format("{0}{1}", firstChar, secondChar);
                }

                int firstInt = columnIndex / 26 / 26;
                int secondInt = (columnIndex - firstInt * 26 * 26) / 26;
                if (secondInt == 0)
                {
                    secondInt = 26;
                    firstInt = firstInt - 1;
                }
                int thirdInt = (columnIndex - firstInt * 26 * 26 - secondInt * 26);

                firstChar = (char)('A' + firstInt - 1);
                secondChar = (char)('A' + secondInt - 1);
                thirdChar = (char)('A' + thirdInt);

                return string.Format("{0}{1}{2}", firstChar, secondChar, thirdChar);
            }

            //private static double GetWidth(string font, int fontSize, string text)
            //{
            //    System.Drawing.Font stringFont = new System.Drawing.Font(font, fontSize);
            //    return GetWidth(stringFont, text);
            //}

            //private static double GetWidth(System.Drawing.Font stringFont, string text)
            //{
            //    // This formula is based on this article plus a nudge ( + 0.2M )
            //    // http://msdn.microsoft.com/en-us/library/documentformat.openxml.spreadsheet.column.width.aspx
            //    // Truncate(((256 * Solve_For_This + Truncate(128 / 7)) / 256) * 7) = DeterminePixelsOfString

            //    System.Drawing.Size textSize = System.Windows.Forms.TextRenderer.MeasureText(text, stringFont);
            //    double width = (double)(((textSize.Width / (double)7) * 256) - (128 / 7)) / 256;
            //    width = (double)decimal.Round((decimal)width + 0.2M, 2);

            //    return width;
            //}

            private static Column CreateColumnData(UInt32 StartColumnIndex, UInt32 EndColumnIndex, double ColumnWidth)
            {
                Column column;
                column = new Column
                {
                    Min = StartColumnIndex,
                    Max = EndColumnIndex,
                    Width = ColumnWidth,
                    CustomWidth = true
                };
                return column;
            }

            // Given a cell name, parses the specified cell to get the column name.
            private static string GetColumnName(string cellName)
            {
                // Create a regular expression to match the column name portion of the cell name.
                Regex regex = new Regex("[A-Za-z]+");
                Match match = regex.Match(cellName);

                return match.Value;
            }

            // Given a cell name, parses the specified cell to get the row index.
            private static uint GetRowIndex(string cellName)
            {
                // Create a regular expression to match the row index portion the cell name.
                Regex regex = new Regex(@"\d+");
                Match match = regex.Match(cellName);

                return uint.Parse(match.Value);
            }

            private static IList<Row> _rowListCache = new List<Row>();

            // Given a Worksheet and a cell name, verifies that the specified cell exists.
            // If it does not exist, creates a new cell. 
            private static Cell CreateSpreadsheetCellIfNotExist(Worksheet worksheet, string cellName)
            {
                string columnName = GetColumnName(cellName);
                uint rowIndex = GetRowIndex(cellName);

                Row rowWorkSheet = rowIndex < _rowListCache.Count ? _rowListCache[(int)rowIndex] : null;
                Cell cell;

                // If the Worksheet does not contain the specified row, create the specified row.
                // Create the specified cell in that row, and insert the row into the Worksheet.
                if (rowWorkSheet == null)
                {
                    Row row = new Row() { RowIndex = new UInt32Value(rowIndex) };
                    cell = new Cell() { CellReference = new StringValue(cellName) };
                    row.Append(cell);
                    worksheet.Descendants<SheetData>().First().Append(row);
                    _rowListCache.Add(row);
                }
                else
                {
                    Row row = rowWorkSheet;
                    cell = new Cell() { CellReference = new StringValue(cellName) };
                    row.Append(cell);
                }

                return cell;
            }

            private static Stylesheet CreateStylesheet()
            {
                // Stylesheet declarion and namespace
                Stylesheet stylesheet = new Stylesheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
                stylesheet.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
                stylesheet.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");

                // List of fonts
                Fonts fontsList = new Fonts() { Count = (UInt32Value)2U, KnownFonts = true };

                // FontId=0 - Regular Excel font
                Font font0 = new Font();
                FontSize fontSize0 = new FontSize() { Val = 11D };
                Color color0 = new Color() { Theme = (UInt32Value)1U };
                FontName fontName0 = new FontName() { Val = "Calibri" };
                FontFamilyNumbering fontFamilyNumbering0 = new FontFamilyNumbering() { Val = 2 };
                FontScheme fontScheme0 = new FontScheme() { Val = FontSchemeValues.Minor };
                font0.Append(fontSize0);
                font0.Append(color0);
                font0.Append(fontName0);
                font0.Append(fontFamilyNumbering0);
                font0.Append(fontScheme0);

                // FontId=1 - Bold font for header
                Font font1 = new Font();
                Bold bold = new Bold();
                font1.Append(bold);

                fontsList.Append(font0);
                fontsList.Append(font1);

                // List of fills
                Fills fillList = new Fills() { Count = (UInt32Value)3U };

                // FillId = 0, Normal background

                Fill fill0 = new Fill();
                PatternFill patternFill0 = new PatternFill() { PatternType = PatternValues.None };
                fill0.Append(patternFill0);
            
                // FillId = 00, Normal background
                Fill fill00 = new Fill();
                PatternFill patternFill00 = new PatternFill() { PatternType = PatternValues.None };
                fill00.Append(patternFill00);

                // FillId = 1, Light Blue for alternating cells
                Fill fill1 = new Fill();
                PatternFill patternFill1 = new PatternFill() { PatternType = PatternValues.None };
                //PatternFill patternFill1 = new PatternFill() { PatternType = PatternValues.Solid };
                //ForegroundColor foregroundColor1 = new ForegroundColor() { Rgb = "FFDEEFF7" };
                //patternFill1.Append(foregroundColor1);
                fill1.Append(patternFill1);

                fillList.Append(fill0);
                fillList.Append(fill00);
                fillList.Append(fill1);

                // Borders style
                Borders bordersList = new Borders() { Count = (UInt32Value)1U };
                Border border1 = new Border();
                LeftBorder leftBorder1 = new LeftBorder();
                RightBorder rightBorder1 = new RightBorder();
                TopBorder topBorder1 = new TopBorder();
                BottomBorder bottomBorder1 = new BottomBorder();
                DiagonalBorder diagonalBorder1 = new DiagonalBorder();
                border1.Append(leftBorder1);
                border1.Append(rightBorder1);
                border1.Append(topBorder1);
                border1.Append(bottomBorder1);
                border1.Append(diagonalBorder1);
                bordersList.Append(border1);

                // List of cell styles formats
                CellStyleFormats cellStyleFormatsList = new CellStyleFormats() { Count = (UInt32Value)1U };
                CellFormat cellFormat = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U };
                cellStyleFormatsList.Append(cellFormat);

                // Cells formats 
                CellFormats cellFormatsList = new CellFormats() { Count = (UInt32Value)5U };

                // StyleIndex = 0 - Regular font
                CellFormat cellFormat0 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U };

                // StyleIndex = 1 - Bold font
                CellFormat cellFormat1 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U };

                // StyleIndex = 2 - Date (Short Date)
                CellFormat cellFormat2 = new CellFormat() { NumberFormatId = (UInt32Value)14U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U };

                // StyleIndex = 3 - Light blue background (Text and Numbers)
                CellFormat cellFormat3 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)2U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFill = true };

                // StyleIndex = 4 - Light blue background (Date)
                CellFormat cellFormat4 = new CellFormat() { NumberFormatId = (UInt32Value)14U, FontId = (UInt32Value)0U, FillId = (UInt32Value)2U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFill = true };

                cellFormatsList.Append(cellFormat0);
                cellFormatsList.Append(cellFormat1);
                cellFormatsList.Append(cellFormat2);
                cellFormatsList.Append(cellFormat3);
                cellFormatsList.Append(cellFormat4);

                // Cells styles
                CellStyles cellStyleList = new CellStyles() { Count = (UInt32Value)1U };
                CellStyle cellStyle0 = new CellStyle() { Name = "Normal", FormatId = (UInt32Value)0U, BuiltinId = (UInt32Value)0U };
                cellStyleList.Append(cellStyle0);

                DifferentialFormats differentialFormats0 = new DifferentialFormats() { Count = (UInt32Value)0U };
                TableStyles tableStyles0 = new TableStyles() { Count = (UInt32Value)0U, DefaultTableStyle = "TableStyleMedium2", DefaultPivotStyle = "PivotStyleMedium9" };
                StylesheetExtensionList stylesheetExtensionList = new StylesheetExtensionList();
                StylesheetExtension stylesheetExtension1 = new StylesheetExtension() { Uri = "{EB79DEF2-80B8-43e5-95BD-54CBDDF9020C}" };
                stylesheetExtensionList.Append(stylesheetExtension1);

                stylesheet.Append(fontsList);
                stylesheet.Append(fillList);
                stylesheet.Append(bordersList);
                stylesheet.Append(cellStyleFormatsList);
                stylesheet.Append(cellFormatsList);
                stylesheet.Append(cellStyleList);
                stylesheet.Append(differentialFormats0);
                stylesheet.Append(tableStyles0);
                stylesheet.Append(stylesheetExtensionList);

                return stylesheet;
            }

        }
}