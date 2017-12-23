<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DisplayProgramMarkResult.aspx.cs" Inherits="GradeDiagramTest.DisplayProgramMarkResult" %>

<!DOCTYPE html>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="./d3-3.5.0/d3.min.js" charset="utf-8"></script>
    <script src="./c3-0.4.15/c3.min.js"></script>
    <script src="./Scripts/jquery-1.10.2.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
    <script>
        var question;
        var ChartArgArr;
        var chart;
        var colors = ['#1f77b4', '#aec7e8', '#ff7f0e', '#ffbb78', '#2ca02c', '#98df8a', '#d62728', '#ff9896', '#9467bd', '#c5b0d5', '#8c564b', '#c49c94', '#e377c2', '#f7b6d2', '#7f7f7f', '#c7c7c7', '#bcbd22', '#dbdb8d', '#17becf', '#9edae5'];

        function FindArrays() {
            return [$(".Average" + question).map(function () { return parseInt(this.innerText) }).toArray(), $(".MQ" + question).map(function () { return this.innerText }).toArray()]
        }

        function SortByAvg(type) {
            var i, j, tmp, tmp_1;
            switch (type) {

                case 0:
                    for (i = 1; i < ChartArgArr[0].length; i++) {
                        tmp = ChartArgArr[0][i];
                        tmp_1 = ChartArgArr[1][i];
                        for (j = i; j > 0 && tmp < ChartArgArr[0][j - 1]; j--) {
                            ChartArgArr[0][j] = ChartArgArr[0][j - 1];
                            ChartArgArr[1][j] = ChartArgArr[1][j - 1];
                        }
                        ChartArgArr[0][j] = tmp;
                        ChartArgArr[1][j] = tmp_1;

                    }
                    break;
                case 1:
                    for (i = 1; i < ChartArgArr[0].length; i++) {
                        tmp = ChartArgArr[0][i];
                        tmp_1 = ChartArgArr[1][i];
                        for (j = i; j > 0 && tmp > ChartArgArr[0][j - 1]; j--) {
                            ChartArgArr[0][j] = ChartArgArr[0][j - 1];
                            ChartArgArr[1][j] = ChartArgArr[1][j - 1];
                        }
                        ChartArgArr[0][j] = tmp;
                        ChartArgArr[1][j] = tmp_1;
                    }
                    break;
            }
        }

        function ChartGenerate(AvgArr, CatArr) {
            chart = c3.generate({
                bindto: '.' + question + 'Chart',
                data: {
                    columns: [
                        ['Average'].concat(AvgArr)
                    ],
                    type: 'bar',
                    labels: true,

                    color: function (color, d) {
                        return colors[d.index];
                    }
                },
                axis: {
                    x: {
                        type: 'category',
                        categories: CatArr
                    }
                },
                bar: {
                    width: {
                        ratio: 0.5,
                        max: 30
                    }
                },
                grid: {
                    focus: {
                        show: false
                    }
                }
            });
        }
        $(document).ready(function () {
            $(".Diagram1").click(function () {
                ChartArgArr = FindArrays();
                ChartGenerate(ChartArgArr[0], ChartArgArr[1])
            });
            $(".Diagram2").click(function () {
                SortByAvg(0);
                ChartGenerate(ChartArgArr[0], ChartArgArr[1])
            });
            $(".Diagram3").click(function () {
                SortByAvg(1);
                ChartGenerate(ChartArgArr[0], ChartArgArr[1])
            });

        })
    </script>

    <link rel="stylesheet" href="./c3-0.4.15/c3.css" />
    <link rel="stylesheet" href=".Content/DisplayProgram.css" />
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css"/>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>

<body >
    <form id="form1" runat="server">

        <%-- Dynamic add javascript for auto type with qusetion change --%>
        
        <div class="container" style="width:50%;align-self:center;text-align:center">
        <%-- Dynamic add tab for question --%>
        <ul id="nav_control" class="nav nav-tabs" runat="server">
        </ul>


        <%-- Dynamic add tab-pane table --%>
        <div id="tab_content_control" class="tab-content" runat="server">
        </div>


        <%-- type tab and download --%>
        <ul id="nav_control_button" class="nav nav-tabs" runat="server">
            <li class="Diagram1 active"><a data-toggle="tab" href="#" >原始平均</a></li>
            <li class="Diagram2"><a data-toggle="tab" href="#" >由低到高</a></li>
            <li class="Diagram3"><a data-toggle="tab" href="#" >由高到低</a></li>
            <li>
                <asp:LinkButton ID="DownLoadToExl" class="btn" OnClick="DownLoadToExl_Click" runat="server">Download Excel</asp:LinkButton></li>
        </ul>


        <%-- Dynamic add tab-pane chart --%>
        <div id="tab_content_chart_control" class="tab-content" runat="server">

        </div>

        </div>
            <script>
                var question_arr = $(".Question a").map(function () { return this.innerText }).toArray();
                question = question_arr[0];
                ChartArgArr = FindArrays();
                ChartGenerate(ChartArgArr[0], ChartArgArr[1])
                $(document).ready(function () {
                    for (var index in question_arr) {
                        console.log(question_arr[index])
                        $("." + question_arr[index] + " a").click(function () {
                            question = this.innerText;
                            ChartArgArr = FindArrays();
                            $(".Diagram1 a").click()
                            ChartGenerate(ChartArgArr[0], ChartArgArr[1])
                        });
                    }

                })
            </script>


    </form>

</body>
</html>