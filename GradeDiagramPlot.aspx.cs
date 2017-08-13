using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;

namespace GradeDiagramTest
{
    public partial class GradeDiagramPlot : System.Web.UI.Page
    {
        // Student Scroe and Question from Database

        public string []FirstColDefault = {"總分"};
        //
       
        List<ScoreAnalysisM> ScoreAnalysisList = new List<ScoreAnalysisM>();
        private int table_rows;
        private int table_cols;
        private int []QuestionAvg;
        private int QuestionNum;
        private int studentNum;
        private List<string> AddRowsName = new List<string>();
        private List<string> AddColsName = new List<string>();
        private TableCell tc_temp;
        

        protected void Page_Load(object sender, EventArgs e)
        {

            

            //下方3個變數的值之後會由DB取得
            //get all the data from ScoreDetailTB table
            DataTable dt = CsDBOp.GetAllTBData();
            

            //Get the retrieved data from each row of the retrieved data table.
            foreach (DataRow dr in dt.Rows)
            {
                ScoreAnalysisM log = new ScoreAnalysisM();

                log.StuCouHWDe_ID = dr.Field<string>("StuCouHWDe_ID");
                log.Grade = dr.Field<string>("Grade").Split(',').ToList<string>();
                log.QuestionNum = Convert.ToInt16(log.Grade[1]);
                log.Grade.RemoveAt(1);
                ScoreAnalysisList.Add(log);

            }


            //Initalize the member variable

            table_cols = FirstColDefault.Length + ScoreAnalysisList[0].Grade.Count-1 ;
            table_rows = ScoreAnalysisList.Count;
            studentNum = table_rows;
            QuestionNum = ScoreAnalysisList[0].Grade.Count - 1;
            // Add a row named Avg
            AddRow("Avg");

            GenerateTable(table_cols, table_rows);

            StringBuilder sb = new StringBuilder();  
            for (int i = 0; i < QuestionNum;i++ )
            {
                string temp2;
                temp2 = "['第" + (i+1) + "題'," + QuestionAvg[i] + "],";
                 sb.Append(temp2);
                
            }
            chart.InnerHtml="<script>var chart = c3.generate({bindto: '#chart',data: {columns: ["+sb.ToString()+"],type : 'pie'}});</script>";
            
        }

        protected void DownLoadToExl_Click(object sender, EventArgs e)
        {
            DownLoadFile();
        }
        private void DownLoadFile() // Convert Table to excel file
        {
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename= " + DateTime.Now.ToString("yyyyMMdd") + "成績清單.xls");
            Response.ContentType = "application/excel";
            Response.Write('\uFEFF');       // Add this, then the saved file will include BOM.
            StringWriter tw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(tw);
            Table_div.RenderControl(hw);
            this.EnableViewState = false;
            Response.Write(tw.ToString());
            Response.End();

        }

        private void InsertTableStr(int row, int col, string str_insert)
        {
            tc_temp = (TableCell)FindControl("TextBoxRow_"+row+"Col_" + col);
            tc_temp.Text = str_insert;
        }

        private string GetTableStr(int row, int col)
        {
            tc_temp = (TableCell)FindControl("TextBoxRow_" + row + "Col_" + col);
            return tc_temp.Text;
        }

        private void AddRow(string RowName)
        {
            AddRowsName.Add(RowName);
            table_rows++;
        }

        private void AddCol(string ColName)
        {
            AddColsName.Add(ColName);
            table_cols++;
        }

        private void GenerateTable(int colsCount, int rowsCount)
        {

            //Creat the Table and Add it to the Page
            
            Table_div.Attributes.Add("border", "1");
            Table_div.Attributes.Add("cellpadding", "1");
            Table_div.Attributes.Add("cellspacing", "1");
            Table_div.Style.Add("width", "100%");
       
            // Now iterate through the table and add your controls 
            for (int i = 0; i <= rowsCount; i++)
            {
                TableRow row = new TableRow();
                for (int j = 0; j <= colsCount; j++)
                {
                    TableCell cell = new TableCell();
                    cell.ID = "TextBoxRow_" + i + "Col_" + j;
                    row.Controls.Add(cell);
                    row.Cells.Add(cell);
                }
                Table_div.Rows.Add(row);
            }

            for (int i = 1; i <= rowsCount - AddRowsName.Count; i++)
            {
                int StudentDataLength = ScoreAnalysisList[i - 1].QuestionNum + FirstColDefault.Length;
                for (int j = 1; j <=StudentDataLength; j++)
                    InsertTableStr(i, j, ScoreAnalysisList[i - 1].Grade[j - 1]);
           }
            
            // Row 0 set
            for (int i = 1; i <= FirstColDefault.Length; i++)
                InsertTableStr(0, i, FirstColDefault[i - 1]);
            
            for (int i = FirstColDefault.Length + 1; i <= colsCount; i++)
                InsertTableStr(0, i, "第" + (i - FirstColDefault.Length) + "題");

            // Column 0 set
            for (int i = 1; i <= studentNum; i++)
                InsertTableStr(i, 0, ScoreAnalysisList[i - 1].StuCouHWDe_ID);
            
            for (int i = studentNum + 1; i <= rowsCount; i++)
                InsertTableStr(i, 0, AddRowsName[i - rowsCount]);
            

            // Average Calculage
            QuestionAvg = new int[QuestionNum];
            for (int j = FirstColDefault.Length+1; j <=colsCount; j++)
            {
                int sum = 0;
                for (int i = 1; i < rowsCount; i++)
                    sum = Convert.ToInt16(GetTableStr(i, j)) + sum;
                sum=sum/(rowsCount-1);
                InsertTableStr(rowsCount, j, sum.ToString());
                QuestionAvg[j-FirstColDefault.Length - 1] = sum;
            }
            
        }


    }
}