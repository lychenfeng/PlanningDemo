var fileTimeStrNew = "";

ThreeDModel = {};




$(function () {
    $("#btn_import").click(function () {
        $("#myModal").modal();
    });
    $("#btn_import1").click(function () {
        $("#myModal1").modal();
    });
    var allfilelength = 0;
    var progress = 0;
    var i = 0;
    //上传
    $("#txt_file").fileinput({
        language: 'zh',
        uploadUrl: "../TestData/Home/UploadFile?type=1",
        uploadAsync: false,
        maxFileCount: 50,
        showZoom: false,
        showDrag: false,
        enctype: 'multipart/form-data',
        validateInitialCount: true
    }).on("filebatchuploadsuccess", function (event, data) {
        var newData = data.response.ResultData.item;
        if (data.response.ResultData.success)
        {
            clearinputFile();
            chultureTip.alert("上传成功", 1)
            $("#myModal").modal("hide");
            //数据赋值
            $("#GKNum").text(newData.length);
            var html = "";
            var cdnum = 0;
            $.each(newData, function (i, v) {
                var gkname = v.name.split(".")[0];
                cdnum += v.data.length;
                $("#CDNum").text(cdnum);
                $.each(v.data, function (i1, v1) {
                    var name = MeasuringPointByName1(v1.name, gkname)
                   html += " <tr><td>" + name + "</td><td>" + v1.length + "</td></tr>";
                })
            })
            $("#DataType").html(html);

            //数据加载到table时候渲染，表头样式会对齐
            $('#dyntable33').dataTable({
                "scrollY": "300",
                "scrollCollapse": "true",
                "paging": "false",
                "bSort": false,
                "info": false,
                'bFilter': false,
                'bLengthChange': false,
            });
        }
        else {
            chultureTip.alert("上传失败", 2)
            $(".bc-upload-progress").show();
            $(".progress-bar").attr("aria-valuenow", progress).css("width", progress + "%").html(progress + "%");
        }
        //MeasuringPointByName(newData);
        //if (i == 0) {
        //    for (var x = 0; x < data.files.length; x++) {
        //        allfilelength = allfilelength + cutFile(data.files[x], 1024 * 1024 * 4).length;
        //    }
        //}
        //i++;
        //if (islast) {
        //    progress = parseFloat((progress + 100 / allfilelength).toFixed(2));
        //    if (i == allfilelength) {
        //        clearinputFile();
        //        chultureTip.alert("上传成功", 1)
        //        $("#myModal").modal("hide");
        //        //数据赋值
        //    } else {
        //        chultureTip.alert("上传失败", 2)
        //        $(".bc-upload-progress").show();
        //        $(".progress-bar").attr("aria-valuenow", progress).css("width", progress + "%").html(progress + "%");
        //    }
        //}
    }).on('fileremoved', function (event, data, msg) {
        $('#txt_file').fileinput('unlock');
    }).on("fileclear", function () {
        clearinputFile();
    }).on('filecleared', function () {
        clearinputFile();
    });
    function clearinputFile() {
        $('#txt_file').fileinput('refresh');
        $('#txt_file').fileinput('unlock');
        i = 0;
        progress = 0;
        allfilelength = 0;
        $(".bc-upload-progress").hide();
    }

});

function MeasuringPointByName1(v, gkname) {
    var i = v.substring(0, 1)
    var name = "";
    switch (i) {
        case "A":
            name =gkname+ "加速度计"+v;
            break;
        case "S":
            name = gkname + "应变片" + v;
            break;
        case "D":
            name = gkname + "位移计" + v;
            break;
        case "P":
            name = gkname + "土压力计" + v;
            break;
        case "T":
            name = gkname + "拉力计" + v;
            break;
        default:
            name = gkname + "未知" + v;
    }
    return name;
}

//说明
//A：加速度计
//S：应变片
//D：位移计
//P：土压力计
//T：拉力计
function MeasuringPointByName(newData)
{
    //$.each(newData, function (i, v) {
    //    console.log(v)

    //});
    var strarr = new Array();
    var jsonarray = [];
    for (var i=1; i < newData.length; i++)
    {
        if (newData[i] != "") {
            var str = newData[i].substring(0, 1)
            strarr.push(str)
        }
    }
   var result= strarr.reduce((o, k) => {
        k in o ? o[k]++ : (o[k] = 1);
        return o;
   }, {});

   for (i in result) {
       console.log(i)
       var name="";
       switch (i)
       {
           case "A":
               name = "加速度计";
               break;
           case "S":
               name = "应变片";
               break;
           case "D":
               name = "位移计";
               break;
           case "P":
               name = "土压力计";
               break;
           case "T":
               name = "拉力计";
               break;
           default:
               name = "未知";
       }
       console.log(result[i])
       var jsonstr = { "name": name, "value": result[i] };
       jsonarray.push(jsonstr)
   }
   console.log(jsonarray)
   //console.log(jsonstr)
   //var jsonstr = result.A
   //console.log(jsonstr)
}

function res(arr) {
    var tmp = [];
    var str = arr.join(',');
    arr.forEach(function (item) {
        var i = 0;
        if (str.replace(item, "").indexOf(item) > -1 && tmp.indexOf(item) === -1) {
            alert("s"+item)
            tmp.push(item);
            i += 1;
        }
        else
        {
            i += 1;
            alert("e" + item)
            console.log()
        }
    })
    return tmp
}


//方案导入
$("#txt_file1").fileinput({
    language: 'zh',
    uploadUrl: "../TestData/Home/UploadFile?type=2",
    uploadAsync: false,
    maxFileCount: 50,
    showZoom: false,
    showDrag: false,
    enctype: 'multipart/form-data',
    validateInitialCount: true
}).on("filebatchuploadsuccess", function (event, data) {
    var newData = data.response.ResultData.item;
    if (data.response.ResultData.success) {
        clearinputFile();
        chultureTip.alert("上传成功", 1)
        $("#myModal1").modal("hide");
        //数据赋值
        $("#GKNum").text(newData.length);
        var html = "";
        var cdnum = 0;
        $.each(newData, function (i, v) {
            var gkname = v.name.split(".")[0];
            cdnum += v.data.length;
            $("#CDNum").text(cdnum);
            $.each(v.data, function (i1, v1) {
                var name = MeasuringPointByName1(v1.name, gkname)
                html += " <tr><td>" + name + "</td><td>" + v1.length + "</td></tr>";
            })
        })
        $("#DataType").html(html);
    }
    else {
        chultureTip.alert("上传失败", 2)
        $(".bc-upload-progress").show();
        $(".progress-bar").attr("aria-valuenow", progress).css("width", progress + "%").html(progress + "%");
    }
    //MeasuringPointByName(newData);
    //if (i == 0) {
    //    for (var x = 0; x < data.files.length; x++) {
    //        allfilelength = allfilelength + cutFile(data.files[x], 1024 * 1024 * 4).length;
    //    }
    //}
    //i++;
    //if (islast) {
    //    progress = parseFloat((progress + 100 / allfilelength).toFixed(2));
    //    if (i == allfilelength) {
    //        clearinputFile();
    //        chultureTip.alert("上传成功", 1)
    //        $("#myModal").modal("hide");
    //        //数据赋值
    //    } else {
    //        chultureTip.alert("上传失败", 2)
    //        $(".bc-upload-progress").show();
    //        $(".progress-bar").attr("aria-valuenow", progress).css("width", progress + "%").html(progress + "%");
    //    }
    //}
}).on('fileremoved', function (event, data, msg) {
    $('#txt_file').fileinput('unlock');
}).on("fileclear", function () {
    clearinputFile();
}).on('filecleared', function () {
    clearinputFile();
});
function clearinputFile() {
    $('#txt_file').fileinput('refresh');
    $('#txt_file').fileinput('unlock');
    i = 0;
    progress = 0;
    allfilelength = 0;
    $(".bc-upload-progress").hide();
}

//数据导出
$("#SJLoad").on('click', function () {
    $("#sjdc").show();
    $("#fadc").hide();
})
//方案导出
$("#FALoad").on('click', function () {
    $("#sjdc").hide();
    $("#fadc").show();
    http.request("/TestData/Home/GetAllTestFilesDoc/", {}, function (e) {
        if (e.Status == 200) {
            console.log(e)
            console.log(e.Data)
            var html = "";
            $.each(e.Data, function (i, v) {
                var url = v.url
                html += " <tr><td>" + v.name + "</td><td><a href=\"#\" onclick=\"xz('" + url + "')\" class=\"btn btn-secondary btn-sm btn-icon icon-left\">下载 </a></td></tr>";
            })
            $("#tbfadc").html(html)
            $('#dyntable55').dataTable({
                "destroy": true,  //datatables允许多次初始化某一表格 (多次初始化会有一个列表数据不刷新的问题，需要手动刷新当前页面)
                "scrollY": "500",
                "scrollCollapse": "true",
                "paging": "false",
                "bSort": false,
                "info": false,
                'bFilter': true,
                'bLengthChange': false,
            });
        } else {
            chultureTip.alert(e.Message, 2);
        }
    });
})

function xz(url)
{
    window.open(url);
}

//根据工况和测点查询
$("#SelectByGKAndCD").on('click', function ()
{
    var numbers = $("#CDNumber").val();
    console.log(numbers)
    var numbersstr = numbers != null && numbers.length > 0 ? numbers.join(",") : "";
    console.log(numbersstr)
})

//点击模型交互Tab
$("#ModelinteractionTab").on('click', function () {
    GetGKNameByProjectID();
})

//查询工况下拉列表
function GetGKNameByProjectID()
{
    //项目id
    var projectid = 1;//暂时写死为1
    http.request('/TestData/Home/GetGKNameByProjectID?ProjectId=' + projectid, {}, function (e)
    {
        var data = e.Data;
        console.log(e)
        if (e.Status == 200)
        {
            var html = "";
            $.each(data, function (i, v) {
                html += "<option value=\"0\">请选择工况</option>";
                html += "<option value=\"" + v.ID + "\">" + v.WorkCondName + "</option>";
            })
            $("#GKName").html(html);
        }
    })
}

select2_replace(["#GKName"], null, function () {
    //change 回掉
    var gkid = $("#GKName").val();
    http.request('/TestData/Home/GetCDNameByGKID?GKId=' + gkid, {}, function (e) {
        var data = e.Data;
        console.log(e)
        if (e.Status == 200) {
            var html = "";
            $.each(data, function (i, v) {
                html += "<option value=\"" + v+ "\">" + v + "</option>";
            })
            $("#CDNumber").html(html);
        }
    })
});
