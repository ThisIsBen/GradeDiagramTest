
using System;
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
    public int QueIndex_start=3;

    public ScoreAnalysisM(string id,string grade_str)
    {
        

        string[] temp_str_arr = MemberQueSpilt(grade_str);
        StuCouHWDe_ID = id;
        for (int i = 0; i < QuestionNum; i++)
        {
            MemberQueGradeSpilt(temp_str_arr[i], i);
        }

    }

    private string[] MemberQueSpilt(string grade_str)
    {
        string[] temp_str_arr = grade_str.Remove(grade_str.Length-1).Split(':');
        QuestionNum = temp_str_arr.Length;
        MemberQuestionNum = new int[QuestionNum];
        return temp_str_arr;
    }

    private string[] MemberQueGradeSpilt(string grade_member_str,int questionNum)
    {
        string[] temp_str_arr = grade_member_str.Split(',');
        MemberQuestionNum[questionNum] = temp_str_arr.Length - QueIndex_start;
        Grade.Add(temp_str_arr);
        return temp_str_arr;
    }


}