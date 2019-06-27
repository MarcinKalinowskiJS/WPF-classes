using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes.Charts;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfInterfejsGraficzny
{
    class PDFCreator
    {
        Document document = null;
        static bool unicode = true;
        static PdfFontEmbedding embedding = PdfFontEmbedding.Always;

        public PDFCreator()
        {
        }

        public void NewTable(string tableTitle, string[] columnNames, string[][] tableData, int colorizingEvery, Color[] colorizingColors)
        {

            Table table = new Table();
            table.Borders.Width = 0.75;

            document.LastSection.AddParagraph(tableTitle, "Heading1");

            //Dodawanie column
            for (int i = 0; i < columnNames.Count(); i++)
            {
                //Ilość liter / wartość   taka szerokość np: wyszukiwanie najszerszej kolumny i ustalenie szerokości na jej podstawie
                Column column = table.AddColumn(Unit.FromCentimeter(2));
                column.Format.Alignment = ParagraphAlignment.Center;
            }

            //Dodawanie wiersza z nazwami kolumn
            Row columnNamesRow = table.AddRow();
            for (int i = 0; i < columnNames.Count(); i++)
            {
                Cell cell = columnNamesRow.Cells[i];
                cell.AddParagraph(columnNames[i]);
            }

            int j = 0;
            //Dodawanie wierszy z tekstem
            for (int y = 0; y < tableData.Count(); y++)
            {
                Row row = table.AddRow();
                row.Shading.Color = colorizingColors[j];
                ++j;
                j %= colorizingEvery;
                for (int x = 0; x < tableData[y].Count(); x++)
                {
                    Cell cell = row.Cells[x];
                    cell.AddParagraph(tableData[y][x]);
                }
            }
            table.SetEdge(0, 0, tableData[0].Count(), tableData.Count()+1, Edge.Box, BorderStyle.Single, 1.5, Colors.Black);

            document.LastSection.Add(table);
        }

        public void PostCreateDocument(string filename, int autostart)
        {
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(unicode, embedding);

            pdfRenderer.Document = document;

            pdfRenderer.RenderDocument();

            pdfRenderer.PdfDocument.Save(filename);

            if (autostart == 1)
            {
                Process.Start(filename);
            }

        }
        public void PreCreateDocument(string title)
        {
            //Create a new MigraDoc document
            document = new Document();

            //Add a section to the document
            PDFCreator.DefineStyles(document);
            document.UseCmykColor = false;

            Section section = document.AddSection();
            document.Info.Title = title;
            document.Info.Subject = title;
            document.Info.Author = "Najlepsza grupa z zajęć WPF";
        }



        /// <summary>
        /// Defines the styles used in the document.
        /// </summary>
        public static void DefineStyles(Document document)
        {
            // Get the predefined style Normal.
            Style style = document.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Times New Roman";

            // Heading1 to Heading9 are predefined styles with an outline level. An outline level
            // other than OutlineLevel.BodyText automatically creates the outline (or bookmarks) 
            // in PDF.

            style = document.Styles["Heading1"];
            style.Font.Name = "Tahoma";
            style.Font.Size = 14;
            style.Font.Bold = true;
            style.Font.Color = Colors.DarkBlue;
            style.ParagraphFormat.PageBreakBefore = true;
            style.ParagraphFormat.SpaceAfter = 6;

            style = document.Styles["Heading2"];
            style.Font.Size = 12;
            style.Font.Bold = true;
            style.ParagraphFormat.PageBreakBefore = false;
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 6;

            style = document.Styles["Heading3"];
            style.Font.Size = 10;
            style.Font.Bold = true;
            style.Font.Italic = true;
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 3;

            style = document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called TextBox based on style Normal
            style = document.Styles.AddStyle("TextBox", "Normal");
            style.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
            style.ParagraphFormat.Borders.Width = 2.5;
            style.ParagraphFormat.Borders.Distance = "3pt";
            style.ParagraphFormat.Shading.Color = Colors.SkyBlue;

            // Create a new style called TOC based on style Normal
            style = document.Styles.AddStyle("TOC", "Normal");
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right, TabLeader.Dots);
            style.ParagraphFormat.Font.Color = Colors.Blue;
        }


        public static void DefineCharts(Document document)
        {
            Paragraph paragraph = document.LastSection.AddParagraph("Chart Overview", "Heading1");
            paragraph.AddBookmark("Charts");

            document.LastSection.AddParagraph("Sample Chart", "Heading2");

            Chart chart = new Chart();
            chart.Left = 0;

            chart.Width = Unit.FromCentimeter(16);
            chart.Height = Unit.FromCentimeter(12);
            Series series = chart.SeriesCollection.AddSeries();
            series.ChartType = ChartType.Column2D;
            series.Add(new double[] { 1, 17, 45, 5, 3, 20, 11, 23, 8, 19 });
            series.HasDataLabel = true;

            series = chart.SeriesCollection.AddSeries();
            series.ChartType = ChartType.Line;
            series.Add(new double[] { 41, 7, 5, 45, 13, 10, 21, 13, 18, 9 });

            XSeries xseries = chart.XValues.AddXSeries();
            xseries.Add("A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N");

            chart.XAxis.MajorTickMark = TickMarkType.Outside;
            chart.XAxis.Title.Caption = "X-Axis";

            chart.YAxis.MajorTickMark = TickMarkType.Outside;
            chart.YAxis.HasMajorGridlines = true;

            chart.PlotArea.LineFormat.Color = Colors.DarkGray;
            chart.PlotArea.LineFormat.Width = 1;

            document.LastSection.Add(chart);
        }
    }
}
