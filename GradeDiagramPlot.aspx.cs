using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Diagnostics;

namespace GradeDiagramTest
{
    public partial class GradeDiagramPlot : System.Web.UI.Page
    {
        // Student Scroe and Question from Database
        
        
        public string []FirstColDefault = {"學生","總分"};
       

        //
       
        List<ScoreAnalysisM> ScoreAnalysisList = new List<ScoreAnalysisM>();
        private int table_rows;
        private int table_cols;

        private List<int[]> QuestionAvg=new List<int[]>();
        private List<string> correctXML = new List<string>();
        private int MemberQuestionNum;
        private int studentNum;
        private int currentQuestion=0;
        private List<string> AddRowsName = new List<string>();
        private List<string> AddColsName = new List<string>();
        public string[] QuestionName;
        public List<string[]> MemberQuestionAnswer=new List<string[]>();
        private Hashtable[] correctAnswerHT ;

        protected void Page_Load(object sender, EventArgs e)
        {

            

            //下方3個變數的值之後會由DB取得
            //get all the data from IPCExamHWCorrectAnswer table



            string cActivityID_Selector = Request.QueryString["cActivityID"];
            DataTable dt = CsDBOp.GetAllTBData("IPCExamHWCorrectAnswer", cActivityID_Selector);
            //Exception for empty table data
            if (dt.Rows.Count==0)
                Response.End();


            //Test for cActivityID with input for IPCExamHWCorrectAnswer
            QuestionName=dt.Rows[0].Field<string>("QuestionBodyPart").Split(',');
            string TempCA=dt.Rows[0].Field<string>("correctAnswer");
            foreach (string TempCA_fullstr in TempCA.Remove(TempCA.Length - 1).Split(':'))
            {
                 
                 MemberQuestionAnswer.Add(TempCA_fullstr.Split(','));
            }

            string temp_str= dt.Rows[0].Field<string>("correctAnswerOrdering");
            string[] tempCAOstr = temp_str.Remove(temp_str.Length-1).Split(':');
            correctAnswerHT = new Hashtable[tempCAOstr.Length];
            int index = 0;
            foreach (string tempCAOstr_split in tempCAOstr)
            {
                string[] tempCAOstr_in_split = tempCAOstr_split.Split(',');
                correctXML.Add(tempCAOstr_in_split[0]);
                correctAnswerHT[index] = new Hashtable();
                for (int i = 1; i < tempCAOstr_in_split.Length; i++)
                {
                    correctAnswerHT[index].Add(tempCAOstr_in_split[i], MemberQuestionAnswer[index][i]);
                }
                index++;
            }


            dt = CsDBOp.GetAllTBData("StuCouHWDe_IPC", cActivityID_Selector);

            //Get the retrieved data from each row of the retrieved data table.
            foreach (DataRow dr in dt.Rows)
            {
                string StudentIDTemp = dr.Field<string>("StuCouHWDe_ID");
                string Gradetemp = dr.Field<string>("Grade");
                if (Gradetemp != null)
                {
                    ScoreAnalysisM log_temp = new ScoreAnalysisM(StudentIDTemp, Gradetemp);
                    ScoreAnalysisList.Add(log_temp);
                    continue;
                }
                    
                
                string AnsewerTemp = dr.Field<string>("StudentAnswer");
                string QuesOrdering = dr.Field<string>("QuesOrdering");
                if (AnsewerTemp == null)
                    continue;
                ScoreAnalysisM log = new ScoreAnalysisM(StudentIDTemp, AnsewerTemp, QuesOrdering,correctXML,correctAnswerHT);
                ScoreAnalysisList.Add(log);

                //比對學生作答內容中的XML檔名
                if (log.XMLerror)
                {
                    Response.Write("alert('Student answer XML file name does not match Correct answer XML file name')");
                    Response.End();
                }

            }
            
            
            //// Add a row named Avg
            AddRow("Avg");
            
            //// calculate average
            MemberQueAvgCal();

            // Add table html for front-end dynamically
            AddDynamicTableHtml(QuestionName.Length, QuestionName);
            

            /// plot the chart
            tab_content_chart_control.InnerHtml = AddChartTabPaneHtml();

            
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
                    sb.Append("<li class='Question active " + question_name[i] + "'><a data-toggle='tab' href='." + question_name[i] + "Table" + "'>" + question_name[i] + "</a></li>");
                }
                else
                {
                    temp_div.Attributes.Add("class", "tab-pane fade "+ question_name[i] + "Table");
                    
                    // nav_control innerHtml add string 
                    sb.Append("<li class='Question " + question_name[i] + "'><a data-toggle='tab' href='." + question_name[i] + "Table" + "'>" + question_name[i] + "</a></li>");
                }
            }
            nav_control.InnerHtml = sb.ToString();

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


        private void MemberVariableSet(int Question_num)
        {
            currentQuestion = Question_num;
            table_cols = ScoreAnalysisList[0].Grade[currentQuestion].Length+1;// +1 mean student column
            studentNum = ScoreAnalysisList.Count;
            table_rows = studentNum+AddRowsName.Count;
            
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
        private void InsertTableStr(int row, int col, string str_insert,string Classname_Insert)
        {
            TableCell tc_temp;    
            tc_temp = (TableCell)FindControl("TextBoxRow_" + row + "Col_" + col + "_" + currentQuestion);
            tc_temp.Attributes.Add("class", Classname_Insert);
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
            
        }

        private void AddCol(string ColName)
        {
            AddColsName.Add(ColName);
            
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

            for (int i = 1; i <= studentNum; i++)
            {
                for (int j = 0; j <ScoreAnalysisList[i - 1].Grade[currentQuestion].Length; j++)
                    InsertTableStr(i, j+1, ScoreAnalysisList[i - 1].Grade[currentQuestion][j]);
           }
            
            // Row 0 set
            for (int i = 0; i < FirstColDefault.Length; i++)
                InsertTableStr(0, i, FirstColDefault[i]);
            
            for (int i = 1; i <=MemberQuestionNum; i++)
                InsertTableStr(0, i+FirstColDefault.Length-1, MemberQuestionAnswer[currentQuestion][i],"MQ"+QuestionName[currentQuestion]);

            // Column 0 set
            for (int i = 1; i <= studentNum; i++)
                InsertTableStr(i, 0, ScoreAnalysisList[i - 1].StuCouHWDe_ID);
            
            for (int i = studentNum + 1; i <= rowsCount; i++)
                InsertTableStr(i, 0, AddRowsName[i - rowsCount]);

                
            //Average Calculage
            for (int index = 0; index <MemberQuestionNum; index++)
            {
                string sum = QuestionAvg[currentQuestion][index].ToString();
                InsertTableStr(rowsCount, index + FirstColDefault.Length,sum,"Average"+QuestionName[currentQuestion]);
            }
            int avg_total=0;
            for (int i = 0; i < studentNum; i++)
                avg_total += Convert.ToInt16(ScoreAnalysisList[i].Grade[currentQuestion][0]);
            avg_total /= studentNum;
            InsertTableStr(rowsCount, 1, avg_total.ToString());
            
        }

    }
}