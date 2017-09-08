using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using System.Diagnostics;


namespace GradeDiagramTest
{
    public partial class GradeDiagramPlot : System.Web.UI.Page
    {
        // Student Scroe and Question from Database
        public string[] QuestionName = {"Knee","Hand" };
        public List<string[]> MemberQuestionName=new List<string[]>();
        public string []FirstColDefault = {"學生","總分"};
        public string CPaperID_Selector = "001";  //This is CPaperID test variable

        //
       
        List<ScoreAnalysisM> ScoreAnalysisList = new List<ScoreAnalysisM>();
        private int table_rows;
        private int table_cols;

        private List<int[]> QuestionAvg=new List<int[]>();

        private int MemberQuestionNum;
        private int studentNum;
        private int currentQuestion=0;
        private int data_implicit_num = 1;//小題的題數，因為不會呈現，後面資料往前一格
        private List<string> AddRowsName = new List<string>();
        private List<string> AddColsName = new List<string>();
        
        

        protected void Page_Load(object sender, EventArgs e)
        {

            

            //下方3個變數的值之後會由DB取得
            //get all the data from ScoreDetailTB table

            //Test for CPaperID with input '001'
            DataTable dt = CsDBOp.GetAllTBData(CPaperID_Selector);
            MemberQuestionName.Add(new string[] { "adducor", "fadf", "asdf", "dfefa","adsfas" });
            MemberQuestionName.Add(new string[]{ "adducor", "fadf", "asdf", "dfefa" } );


            //Get the retrieved data from each row of the retrieved data table.
            foreach (DataRow dr in dt.Rows)
            {

               Debug.WriteLine(dr.Field<string>("StuCouHWDe_ID") + dr.Field<string>("CPaperID"));
                string StudentIDTemp = dr.Field<string>("StuCouHWDe_ID");
                string GradeStrTemp = dr.Field<string>("Grade");
                if (GradeStrTemp == null)
                    continue;

                ScoreAnalysisM log = new ScoreAnalysisM(StudentIDTemp,GradeStrTemp);
                ScoreAnalysisList.Add(log);

            }
            
            
            //// Add a row named Avg
            AddRow("Avg");
            
            //// calculate average
            MemberQueAvgCal();

            // Add table html for front-end dynamically
            AddDynamicTableHtml(QuestionName.Length, QuestionName);
            

            /// plot the chart
            /// 
            MemberQueAvgSort();

            tab_content_chart_control.InnerHtml = AddChartTabPaneHtml();

            chartPlotJs.InnerHtml = AddChartPlotJs();
            
            onclick_add.InnerHtml = AddOnclickJS();
            
        }

        private string AddChartTabPaneHtml()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < QuestionName.Length; i++)
            {
                if (i == 0)
                    sb.Append("<div class='tab-pane fade in active " + QuestionName[i] + "Table " + QuestionName[i] + "Chart'></div>");
                else
                    sb.Append("<div class='tab-pane fade " + QuestionName[i] + "Table " + QuestionName[i] + "Chart'></div>");
            }
            return sb.ToString();
        }
        
        private string AddChartPlotJs()
        {  
            StringBuilder sb=new StringBuilder();
            sb.Append("<script> var chart={");
            for (int i = 0; i < QuestionName.Length; i++)
                sb.Append(ChartPlotStr("bar", QuestionName[i],i));
            sb.Append("}</script>");
            return sb.ToString();
        }


        private string AddOnclickJS()
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("<script> question='" + QuestionName[0]+"';");

            sb.Append("$(document).ready(function(){");
            foreach (string question in QuestionName)
            {
                sb.Append( "$(\"."+question+"\").click(function () { question=\""+question+"\";chart[question].transform(type); });");

            }
            sb.Append("})</script>");
            return sb.ToString();

        }


        private void AddDynamicTableHtml(int tab_num, string[] question_name)
        {
            Table[] temp_table;
            temp_table = new Table[question_name.Length];

            Panel temp_div;
            StringBuilder sb=new StringBuilder();
            for (int i = 0; i < tab_num; i++)
            {

                temp_table[i] = new Table();

                
                temp_div = new Panel();
                tab_content_control.Controls.Add(temp_div);
                temp_div.ID = question_name[i] + "Table";
                temp_div.Controls.Add(temp_table[i]);
                MemberVariableSet(i);
                GenerateTable(table_cols, table_rows, temp_table[i]);
                
                if (i == 0)
                {
                    temp_div.Attributes.Add("class", "tab-pane fade in active "+question_name[i]+"Table");                   
                    
                    // nav_control innerHtml add string 
                    sb.Append("<li class=\"active "+ question_name[i]+ "\"><a data-toggle=\"tab\" href=\"." + question_name[i] + "Table" + "\">" + question_name[i] +"</a></li>");
                }
                else
                {
                    temp_div.Attributes.Add("class", "tab-pane fade "+ question_name[i] + "Table");
                    
                    // nav_control innerHtml add string 
                    sb.Append("<li class=\"" + question_name[i] + "\"><a data-toggle=\"tab\" href=\"." + question_name[i] + "Table" + "\">" + question_name[i] + "</a></li>");
                }
            }
            nav_control.InnerHtml = sb.ToString();

        }

        private string ChartPlotStr(string type,string question,int question_number)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < QuestionAvg[question_number].Length; i++)
            {
                string temp2;
                temp2 = "['" + MemberQuestionName[question_number][i]+ "'," + QuestionAvg[question_number][i] + "],";
                sb.Append(temp2);

            }
            return question + " : c3.generate({bindto: '." + question + "Chart" + "',data: {columns: [" + sb.ToString() + "],type : '" + type 
                + "'},pie:{label: {format: function (value, ratio, id) {return d3.format()(value);}}},donut:{label: {format: function (value, ratio, id) {return d3.format()(value);}}},bar: {width: {ratio: 0.2}},axis: {x: {type: 'category', categories: ['avg']}}}),";

        }

        private void MemberQueAvgCal()
        {
            int Question_start=ScoreAnalysisList[0].QueIndex_start;
            int QuestionNum= ScoreAnalysisList[0].QuestionNum;
            int[] QuestionAvgTemp; 
            for (int QueIndex = 0; QueIndex < QuestionNum; QueIndex++)
            {
                int MemberQuestionNum = ScoreAnalysisList[0].MemberQuestionNum[QueIndex];
                QuestionAvgTemp = new int[MemberQuestionNum];
                for (int index = 0; index < MemberQuestionNum; index++)
                {
                    int sum = 0;
                    for (int student = 0; student < ScoreAnalysisList.Count; student++)
                        sum = Convert.ToInt16(ScoreAnalysisList[student].Grade[QueIndex][Question_start+index]) + sum;
                    sum = sum / ScoreAnalysisList.Count;
                    QuestionAvgTemp[index] = sum;
                }
                QuestionAvg.Add(QuestionAvgTemp);
            }

           
        }

        private void MemberQueAvgSort()
        {
            for (int question = 0; question < QuestionName.Length; question++)
            {
                for (int i = QuestionAvg[question].Length - 1; i > 0; i--)
                    for (int j = 0; j < i; j++)
                        if (QuestionAvg[question][j] > QuestionAvg[question][j + 1])
                        {
                            Swap(QuestionAvg[question],MemberQuestionName[question], j, j + 1);
                        }

            }
        }
        

        private void Swap(int[] arrayInt, string[] arrayStr, int indexA, int indexB)
        {
            int tmpInt = arrayInt[indexA];
            arrayInt[indexA] = arrayInt[indexB];
            arrayInt[indexB] = tmpInt;

            string tmpStr = arrayStr[indexA];
            arrayStr[indexA] = arrayStr[indexB];
            arrayStr[indexB] = tmpStr;

        }

        private void MemberVariableSet(int Question_num)
        {
            currentQuestion = Question_num;
            table_cols = ScoreAnalysisList[0].Grade[currentQuestion].Length - data_implicit_num;
            table_rows = ScoreAnalysisList.Count;
            studentNum = table_rows;
            MemberQuestionNum = ScoreAnalysisList[0].MemberQuestionNum[currentQuestion];
            
        }

        protected void DownLoadToExl_Click(object sender, EventArgs e)
        {
            Debug.WriteLine(currentQuestion);
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
            tab_content_control.RenderControl(hw);
            this.EnableViewState = false;
            Response.Write(tw.ToString());
            Response.End();

        }

        private void InsertTableStr(int row, int col, string str_insert)
        {
            TableCell tc_temp;
            tc_temp = (TableCell)FindControl("TextBoxRow_"+row+"Col_" + col+"_"+currentQuestion);
            tc_temp.Text = str_insert;
        }

        private string GetTableStr(int row, int col)
        {
            TableCell tc_temp;
            tc_temp = (TableCell)FindControl("TextBoxRow_" + row + "Col_" + col + "_" + currentQuestion);
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

        private void GenerateTable(int colsCount, int rowsCount,Table table_select)
        {

            //Creat the Table and Add it to the Page
            table_select.Attributes.Add("border", "1");
            table_select.Attributes.Add("cellpadding", "1");
            table_select.Attributes.Add("cellspacing", "1");
            table_select.Style.Add("width", "100%");
       
            // Now iterate through the table and add your controls 
            for (int i = 0; i <= rowsCount; i++)
            {
                TableRow row = new TableRow();
                for (int j = 0; j < colsCount; j++)
                {
                    TableCell cell = new TableCell();
                    cell.ID = "TextBoxRow_" + i + "Col_" + j + "_" + currentQuestion;
                    row.Controls.Add(cell);
                    row.Cells.Add(cell);
                }
                table_select.Rows.Add(row);
            }

            for (int i = 1; i <= rowsCount - AddRowsName.Count; i++)
            {
                for (int j = ScoreAnalysisList[i - 1].QueIndex_start; j <ScoreAnalysisList[i - 1].Grade[currentQuestion].Length; j++)
                    InsertTableStr(i, j - data_implicit_num, ScoreAnalysisList[i - 1].Grade[currentQuestion][j]);
           }
            
            // Row 0 set
            for (int i = 0; i < FirstColDefault.Length; i++)
                InsertTableStr(0, i, FirstColDefault[i]);
            
            for (int i = 1; i <=MemberQuestionNum; i++)
                InsertTableStr(0, i+FirstColDefault.Length-1, MemberQuestionName[currentQuestion][i-1]);

            // Column 0 set
            for (int i = 1; i <= studentNum; i++)
                InsertTableStr(i, 0, ScoreAnalysisList[i - 1].StuCouHWDe_ID);
            
            for (int i = studentNum + 1; i <= rowsCount; i++)
                InsertTableStr(i, 0, AddRowsName[i - rowsCount]);

            // Add total column
           for (int row = 1; row <= studentNum; row++)
               InsertTableStr(row, 1, ScoreAnalysisList[row - 1].Grade[currentQuestion][1]);
            
                
            //Average Calculage
            for (int index = 0; index <MemberQuestionNum; index++)
            {
                string sum = QuestionAvg[currentQuestion][index].ToString();
                InsertTableStr(rowsCount, index + FirstColDefault.Length,sum);
            }
            
            
        }

    }
}