using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
    }


}