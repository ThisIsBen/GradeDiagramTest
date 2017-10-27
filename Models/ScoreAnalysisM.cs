using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
/// <summary>
/// Summary description for ScoreAnalysisM
/// </summary>
public class ScoreAnalysisM
{
    public string StuCouHWDe_ID;
    public List<string[]> Grade=new List<string[]>();
    public int QuestionNum;
    public int[] MemberQuestionNum;
    public int QueIndex_start=1;
    public bool XMLerror = false;

    public ScoreAnalysisM(string id, string GradeStr)
    {
        StuCouHWDe_ID = id;
        string []QuestionGroup= GradeStr.Remove(GradeStr.Length-1).Split(':');
        QuestionNum = QuestionGroup.Length;
        
        foreach (string temp_str in QuestionGroup)
        {
            string []str_add;
            string [] temp_str_arr=temp_str.Split(',');
           
            str_add = new string[Convert.ToInt16(temp_str_arr[2])+1];
            str_add[0] = temp_str_arr[1];
            for (int i = 3; i < temp_str_arr.Length; i++)
                str_add[i - 2] = temp_str_arr[i];
            Grade.Add(str_add);
        }
        MemberQuestionNum = new int[QuestionNum];
        for (int i = 0; i < QuestionNum; i++)
            MemberQuestionNum[i] = Grade[i].Length-1;
    }
    
    public ScoreAnalysisM(string id, string AnswerStr,string QuesOdrStr,List<string> xmlFile ,Hashtable[] correctAnswerHT)
    {


        //string[] temp_str_ar;
        StuCouHWDe_ID = id;
        string[] AnswerStr_Question= AnswerStr.Remove(AnswerStr.Length - 1).Split(':');
        string[] QuesOdrStr_Question = QuesOdrStr.Remove(QuesOdrStr.Length - 1).Split(':');
        MemberQuestionNum = new int[AnswerStr_Question.Length];
        QuestionNum = AnswerStr_Question.Length;
        for (int i = 0; i < AnswerStr_Question.Length; i++)
        {
            
            string[] AnswerStr_MemQuestion = AnswerStr_Question[i].Split(',');
            string[] QuesOdrStr_MemQuestion = QuesOdrStr_Question[i].Split(',');
            if (xmlFile[i] != AnswerStr_MemQuestion[0])
                XMLerror = true;
            MemberQuestionNum[i] = QuesOdrStr_MemQuestion.Length-QueIndex_start;
            Grade.Add(new string[QuesOdrStr_MemQuestion.Length]);
            int grade_perQuestion = 100 / QuesOdrStr_MemQuestion.Length;
            int correct_num = 0;
            for (int index = 1; index < QuesOdrStr_MemQuestion.Length; index++)
            {
                
                string compare_valstr=correctAnswerHT[i][QuesOdrStr_MemQuestion[index]].ToString();
                if (compare_valstr == AnswerStr_MemQuestion[index])
                {
                    Grade[i][index] = grade_perQuestion.ToString();
                    correct_num++;
                }
                else
                    Grade[i][index] = 0.ToString();
            }
            Grade[i][0] = (correct_num * grade_perQuestion).ToString();
        }

        CsDBOp.UpdateScore("StuCouHWDe_IPC", StuCouHWDe_ID, SQLGradeStr_Upgrade(Grade));
    }
    string SQLGradeStr_Upgrade(List<string[]> grade)
    {
        StringBuilder sb = new StringBuilder() ;
        for (int i = 0; i < grade.Count; i++)
        {
            sb.Append((i + 1).ToString());
            sb.Append("," + grade[i][0]);
            sb.Append("," + (grade[i].Length-1).ToString());
            for (int index = 0; index < grade[i].Length - 1; index++)
                sb.Append("," + grade[i][index + 1]);
            sb.Append(':');
        }
        return sb.ToString();
    }

}