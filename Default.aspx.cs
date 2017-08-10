using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Diagnostics;

namespace GradeDiagramTest
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ////insert a datum to ScoreDetailTB table      
            //string ID = "112";
            //string grade = "\"100/2/50/50\"";
            //Response.Write("<script>console.log(" + CsDBOp.InsertScore(ID, grade) + ");</script>");


            //取得資料
            List<ScoreAnalysisM> ScoreAnalysisList = new List<ScoreAnalysisM>();


            //下方3個變數的值之後會由DB取得
            //get all the data from ScoreDetailTB table
            DataTable dt = CsDBOp.GetAllTBData();


            //get the retrieved data from each row of the retrieved data table.
           







            ////update a student's grade
            //string aID = "113";
            //string NewGrade = "\"78/5/0/20/20/20/18\"";
            //Response.Write("<script>console.log(" + CsDBOp.UpdateScore(aID, NewGrade) + ");</script>");
            ///////////////////////////////////////





            ////delete a student's grade      
            //Response.Write("<script>console.log(" + CsDBOp.DeleteScore(ID) + ");</script>");
        }
    }
}