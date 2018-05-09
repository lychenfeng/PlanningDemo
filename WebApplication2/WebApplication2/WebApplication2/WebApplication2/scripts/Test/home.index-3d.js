
//获取用户选择的区间id
var MetroID = $("#MetroList").val();
//与模型交互操作
var bimEngine = null;
var isModelLoaded = false;
var isColorLoaded = false;
var modelColorData = null;
var modelColorDic = new Array();

var timeChanged = true;
//json
var diseaseJsonData = null;
var _diseaseData = null;
var ringJsonData = null;

//mm
var m_thickness = 1220;
var m_radius = 2750;

//病害贴图
var isDiseaseShow = false;
var isDiseaseLoaded = false;
var currentOBJ = null;
var diseaseObjs = new Array();
var diseaseObjsDic = new Array();

var projectCenter = null;

var currentCamera = {};

var isCheckHighth = false;

function HighConfim() {
    var btn = document.getElementById("HighConfim");
    isCheckHighth = !isCheckHighth;
    if (isCheckHighth) {

        btn.innerHTML = "取消检查高度";
      
    }
    else {
        btn.innerHTML = '开始检查高度';
    }
}

function GetDiseaseJsonData() {
    var url = "/Project/ThreeDModel/GetBGJson?sectionId=" + MetroID + "&datetime=" + fileTimeStrNew;
    http.request(url, {}, function (e) {
        chultureTip.closeloading();
        if (e.Status == 200) {
            var data = e.Data
            // console.log("diseaseData")
            //console.log(data)
            _diseaseData = data;
            diseaseJsonData = JSON.parse(_diseaseData);
            console.log(21212312);
            console.log(diseaseJsonData);
        }
        else {
            chultureTip.alert(e.Message, 2);
        }
    });
}




//重置相机
function resetCameraLook() {
    isDiseaseShow = true;
    addDiseaseImg();
    setCameraRotateMode(1);
    bimEngine.resetCameraLook();
}


//改变相机模式，点击列表时可以调用
function setCameraRotateMode(mode) {
    //1 or 2
    //2漫游
    bimEngine.setCameraRotateMode(mode);
}

var gloabid = "c8c91bce-f8d5-4326-9145-98bbf81cdd5e-00087fe6";

function Test() {
   
    // 获取构件的Transform对象
    var transform = bimEngine.getElementTransform(gloabid);

    originScale = transform.scale;
    console.log(1111);
    console.log(originScale);
    // 放大2倍
    transform.scale.x = 2.0;
    transform.scale.y = 2.0;
    transform.scale.z = 2.0;

    // 赋值完成之后，开始更新
    bimEngine.transformElement(gloabid, transform);
}

function onPickElement(evt) {
    var element = evt.args;

    var transform = bimEngine.getElementTransform(element.GlobalId);
    console.log(transform);
    //console.log("pick point : " + JSON.stringify(evt.args.point));
    //console.log("camera position:" + JSON.stringify(bimEngine.getCameraPosition()));
}

var originScale = null;



function ScaleElement(type) {
    //scaleElement(id,x,y,z)
    
    var transform = bimEngine.getElementTransform(gloabid);
    switch (type) {
        //x+
        case 1:
            
            transform.scale.x = transform.scale.x + 0.1;
            bimEngine.transformElement(gloabid, transform);
            break;
        //x-
        case 2:
           
            transform.scale.x = transform.scale.x - 0.1;
            bimEngine.transformElement(gloabid, transform);
            break;
        //y+
        case 3:
           
            transform.scale.y = transform.scale.y + 0.1;
            bimEngine.transformElement(gloabid, transform);
            break;
        //y-
        case 4:
           
            transform.scale.y = transform.scale.y + 0.1;
            bimEngine.transformElement(gloabid, transform);
            break;
        default:
    }
    var newtr = bimEngine.getElementTransform(gloabid);
    var newscale = newtr.scale;
   
    if (newtr.scale.x >= 1.0 && newtr.scale.y >= 1.0) {
        alert("在合理范围内");
    }
    else {
        alert("不在合理范围内");
    }
}

function JudgeIsOK(box1, box2) {

}


function Cliping() {
    var clip = bimEngine.getClipManager();
    
}

function BackToCenter() {
    if (currentCamera.Position != undefined && currentCamera.Target != undefined) {
        bimEngine.flyFromTo(currentCamera.Position, currentCamera.Target);
        $("#backToCenter").hide();
    }
    else {
            console.log("未声明位置")
    }
}

//obj点击事件
function onExternal3dModelNodeSelected(evt) {
    //记录当前相机位置
    currentCamera.Position = bimEngine.getCameraPosition();
    currentCamera.Target = bimEngine.getCameraTarget();
    //之后返回这个位置，可以调用bimEngine.flyFromTo(currentCamera.Position, currentCamera.Target);
    //注意判断是否为undefined


    var obj = evt.args.node;
    flyToCenterWithDisease(obj.ringNum, obj.position);
    //console.log("obj");
    //console.log(obj);
    showOriginPic(MetroID, obj.ringNum, obj.diseaseCode);
    $("#backToCenter").show();
    //alert("pick disease : " + obj.diseaseCode);
}


function AddGround()
{
    var transform = bimEngine.getElementTransform(gloabid);
    console.log(transform.position.x);
    var path = "/scripts/BIMViz/sdk/viz/data/models/uh60/";
    var objname = "ground.obj";
    var mtlname = "ground.mtl";
    bimEngine.add3dModelObj(path, objname, mtlname, false, function (model) {
        console.log(transform.position.x);
        model.position.x = transform.position.x;
        model.position.y = transform.position.y;
        model.position.z = transform.position.z;
        model.scale.x = 100;
        model.scale.y = 100; 
        model.scale.z = 1; 
        bimEngine.requestOneUpdate(); // 参数改变后，请求刷新场景
    });
}


////病害编号
//"diseaseCode":"blablabla"  
////对应的环编号
//"ringNum": 1,
////初始位置，123456那个
//"originalPosition":2,
////初始角度
//"originalAngle":100,
////转换之后的病害三维坐标
//"translatedPosition": {
//    "x": 0,
//    "y": 0,
//    "z": 0 
//},
////obj文件夹位置
//"folderPath":"xxxx/xxxx/xxx",
////obj文件名，不带后缀
//"objName":"xxx"
function addDisease(disease) {
    //转毫米
    var d_center = new THREE.Vector3(disease.center.x * 304.8, disease.center.y * 304.8, disease.center.z * 304.8);
    var d_face = new THREE.Vector3(disease.face.x, disease.face.y, disease.face.z);
    var p = tryCalculationPosition(d_center.clone(), d_face.clone(), disease.originalPosition, disease.originalAngle);
    bimEngine.add3dModelObj(disease.folderPath, disease.objName + ".obj", disease.objName + ".mtl", false, function (model) {
        if (projectCenter == null) {
            projectCenter = bimEngine.projectData.center;
        }

        var tpx = p.x - projectCenter.X;
        var tpy = p.y - projectCenter.Y;
        var tpz = p.z - projectCenter.Z;

        //d_center.sub(p);
        var vectorToHeart = tryCalculationNormal(p, d_center, d_face);
        vectorToHeart = vectorToHeart.normalize();

        //position
        var offset = 100; //why?
        var offsetV = vectorToHeart.clone().setLength(offset);
        var oP = new THREE.Vector3(tpx, tpy, tpz);
        var nP = new THREE.Vector3(oP.x + offsetV.x, oP.y + offsetV.y, oP.z + offsetV.z);
        model.position.x = nP.x;
        model.position.y = nP.y;
        model.position.z = nP.z;

        //rotation
        var normal = null;
        var quaternion = new THREE.Quaternion();
        var quaternion2 = new THREE.Quaternion();
        var rotation =null;
        if (disease.originalAngle < 0) {
            normal = new THREE.Vector3(0, 0, 1);
            quaternion.setFromUnitVectors(normal, vectorToHeart);
            rotation = (new THREE.Euler()).setFromQuaternion(quaternion.normalize());
            model.rotation.x = rotation.x
            model.rotation.y = rotation.y;
        } else {
            normal = new THREE.Vector3(0, 0, -1);
            quaternion.setFromUnitVectors(normal, vectorToHeart);
            rotation = (new THREE.Euler()).setFromQuaternion(quaternion.normalize());
            model.rotation.x = rotation.x
            model.rotation.y = rotation.y + Math.PI;
        }
        model.traverse(function (child) {
            if (child instanceof THREE.Mesh) {
                child.material.transparent = true;
            }
        });
        var scale = 0.3937;
        model.scale.x *= scale*0.4;
        model.scale.y *= scale;
        model.scale.z *= scale;
        //currentOBJ = model;

        model.diseaseCode = disease.diseaseCode;
        model.ringNum = disease.ringNum;
        diseaseObjsDic[model.diseaseCode] = model;
        //diseaseObjs.push(model);
        bimEngine.requestOneUpdate(); // 参数改变后，请求刷新场景
    });
}

//标志 是否是重新选择采集时间
function ChangeTime() {
    timeChanged = true;
}

function addDiseaseByJson() {
    console.log("324234234232323")
    console.log(diseaseJsonData);
    if (timeChanged) {
        timeChanged = false;
        for (var i = 0; i < diseaseJsonData.data.length; i++) {
            var dis = diseaseJsonData.data[i];
            dis.center = ringJsonData[dis.ringNum - 1].center;
            dis.face = ringJsonData[dis.ringNum - 1].face;
            addDisease(dis);
        }
        isDiseaseLoaded = true;
        isDiseaseShow = true;
    }
   else if (isModelLoaded && !isDiseaseLoaded) {
        timeChanged = false;
        for (var i = 0; i < diseaseJsonData.data.length; i++) {
            var dis = diseaseJsonData.data[i];
            dis.center = ringJsonData[dis.ringNum - 1].center;
            dis.face = ringJsonData[dis.ringNum - 1].face;
            addDisease(dis);
        }
        isDiseaseLoaded = true;
        isDiseaseShow = true;
    }
    else {
        if (diseaseObjsDic.length > 0) {
            if (isDiseaseShow) {
                for (var i = 0; i < diseaseObjsDic.length; i++) {
                    var model = diseaseObjsDic[(i + 1).toString()];
                    if (model != undefined) {
                        model.scale.x = 0.0001;
                        model.scale.y = 0.0001;
                        model.scale.z = 0.0001;
                    }                   
                }
                isDiseaseShow = false;
            }
            else {
                for (var i = 0; i < diseaseObjsDic.length; i++) {
                    var model = diseaseObjsDic[(i + 1).toString()];
                    if (model != undefined) {
                        model.scale.x = 0.3937 * 0.4;
                        model.scale.y = 0.4;
                        model.scale.z = 0.4;
                    }
                }
                isDiseaseShow = true;
            }
            bimEngine.requestOneUpdate();
        }
    }
}

//英尺，英尺，毫米，毫米
function tryCalculationPosition(center, face, originalPosition, originalAngle) {
    //console.log("center：" + JSON.stringify(center));
    var _face = face.normalize();
    var _z = new THREE.Vector3(0, 0, 1);
    var _angle = originalAngle * Math.PI / 180.0;
    var _pv = _z.applyAxisAngle(_face, _angle);
    _pv = _pv.setLength(m_radius);
    //var t = originalPosition > 5 ? 1 : -1;
    if (originalPosition > 5) {
        originalPosition = originalPosition - 5;
    } else {
        originalPosition = -(5 - originalPosition);
    }
    _face = _face.setLength(m_thickness * (originalPosition / 10.0));
    return center.add(_pv).add(_face);
    //这里返回的就是毫米
}

var isCheckBox = false;

function MultipleChoice() {
    isCheckBox = !isCheckBox;
    if (isCheckBox) {
        bimEngine.getRectSelectManager().start();
    }
    
}

function onRectSelectExecuted(evt) {
    console.log(111);
    console.log(evt);
}


//       y
//    *******
//    *     * 
//    *     * 
//    *  n  * -> x
//    *     * 
//    *     * 
//    *******
//    n -> toward outside z
//
//now we need position of this disease and which ring this disease belong to.
//please give me position and ring num.
function tryCalculationNormal(orient, bboxCenter, face) {
    //取自revit
    //is face not hand
    //var face = new THREE.Vector3(1, 0, 0);
    //ring center, now use bimviz getElementAxisAlignedBox(id) to get bbox
    //-51894.7972980617,72277.5758173681,-14.0267146131846
    //var rua = 304.8;
    var center = new THREE.Vector3(bboxCenter.x - orient.x, bboxCenter.y - orient.y, bboxCenter.z - orient.z);

    //get real center
    var plane = new THREE.Plane(face, 0);
    var three = new THREE.Vector3(0, 0, 0);
    plane.projectPoint(center, three);

    //console.log("center : x:" + center.x + " " + "y:" + center.y + " " + "z:" + center.z)
    //console.log("three : x:" + three.x + " " + "y:" + three.y + " " + "z:" + three.z)

    return three;

}



function onShowElementPropertyList(evt) {
   
    var element = evt.args;
    console.log(1111);
    console.log(element);
    
    var propertySets = element.PropertySets;
    if (isCheckHighth) {
        var highth = $('#buildingInfo').val();
        var data = element.Floor;
        console.log(data);
        if (data > highth) {
            alert("当前所选择的高度为：" + data + "米,建筑高度超过规定的" + highth+"米");
        }
        else {
            alert("当前所选择的高度为：" + data + "米,建筑高度未超过规定的" + highth + "米");
        }
    }
    else {

    }

    
}

function flyToCenter(ringNum) {
   

    if (projectCenter == null) {
        projectCenter = bimEngine.projectData.center;
    }
   
    var ringN = parseInt(ringNum);
    if (ringN > 9) {
        var ringData = ringJsonData[ringN - 10];
        var _cneter = ringData.center;
        _cneter = new THREE.Vector3(_cneter.x * 304.8 - projectCenter.X, _cneter.y * 304.8 - projectCenter.Y - 200, _cneter.z * 304.8 - projectCenter.Z);
        var _target = new THREE.Vector3(ringData.face.x, ringData.face.y, ringData.face.z);
        _target = _target.setLength(100);
        var target = _cneter.clone().add(_target);
    }
    else {
        var ringData = ringJsonData[ringN - 1];
        var _cneter = ringData.center;
        _cneter = new THREE.Vector3(_cneter.x * 304.8 - projectCenter.X, _cneter.y * 304.8 - projectCenter.Y - 200, _cneter.z * 304.8 - projectCenter.Z);
        var _target = new THREE.Vector3(ringData.face.x, ringData.face.y, ringData.face.z);
        _target = _target.setLength(100);
        var target = _cneter.clone().add(_target);
        ringData = ringJsonData[ringN + 11];
        _cneter = ringData.center;
        _cneter = new THREE.Vector3(_cneter.x * 304.8 - projectCenter.X, _cneter.y * 304.8 - projectCenter.Y - 200, _cneter.z * 304.8 - projectCenter.Z);
    }
    bimEngine.flyFromTo(_cneter, target);
    setCameraRotateMode(2);
}

function flyToCenterWithDisease(ringNum,target) {
    var ringN = parseInt(ringNum);
    var ringData = ringJsonData[ringN-1];
    var _cneter = ringData.center;
    _cneter = new THREE.Vector3(_cneter.x * 304.8 - projectCenter.X, _cneter.y * 304.8 - projectCenter.Y, _cneter.z * 304.8 - projectCenter.Z);

    bimEngine.flyFromTo(_cneter, target);
    setCameraRotateMode(2);
}

function selectSame(result, text) {
    var vm = bimEngine.getHighlightManager();
    var len = result.list.length;
    if (len == 0)
        return;
    for (var i = 0; i < len; i++) {
        var item = result.list[i];
        var key = item.Element.GlobalId;
        vm.highlightElement(key);
    }
}

function onModelLoaded(evt) {
    isModelLoaded = true;
    //模型加载完毕，显示三维界面底部按钮div
    $("#bottomDiv").show();
}
var _isColored = false;

function resetModelColor() {
    console.log(111111111111);
    console.log(modelColorData);
    for (var i = 0; i < modelColorData.length; i++) {
        var ringnum = modelColorData[i].RingNum;
        var color = null;
        color = [0.7215686274, 0.7215686274, 0.7215686274, 1.0];
        ringnum = parseInt(ringnum);
        var ringData = ringJsonData[ringnum - 1];
        bimEngine.changeElementRGBA(ringData.uniqueID, color);
    }
}




function initModelColor() {
    if (_isColored) {
        resetModelColor();
    }
    else {
        for (var i = 0; i < modelColorData.length; i++) {
            var ringnum = modelColorData[i].RingNum;
            var color = null;
            switch (modelColorData[i].ServiceLevel) {
                case 1:
                    color = [0.0, 0.5, 0.0, 1.0];//绿色
                    break;
                case 2:
                    color = [0.0, 0.0, 1.0, 1.0];//蓝色
                    break;
                case 3:
                    color = [1.0, 1.0, 0.0, 1.0];//黄色
                    break;
                case 4:
                    color = [1.0, 0.6, 0.0, 1.0];//橙色
                    break;
                case 5:
                    color = [1.0, 0.0, 0.0, 1.0];//红色
                    break;

            }
            ringnum = parseInt(ringnum);
            var ringData = ringJsonData[ringnum - 1];
            bimEngine.changeElementRGBA(ringData.uniqueID, color);
        }
    }
    _isColored = !_isColored;
}

function changeColor(result, text) {
    var len = result.list.length;
    if (len == 0)
        return;
    for (var i = 0; i < len; i++) {
        var item = result.list[i];
        var key = item.Element.GlobalId;
        bimEngine.changeElementRGBA(key, modelColorDic[text]);
    }
}

var isFull = false;
var wangtobefull = false;
var current3DdivStyle = {};
function fullpage() {
    //console.log("fullpage now isfull? = " + isFull);
    if (!isFull) {
        var elem = document.getElementById("bimviewer");
        if (!current3DdivStyle.height)
            current3DdivStyle.height = elem.style.height;
        if (!current3DdivStyle.width)
            current3DdivStyle.width = elem.style.width;
        if (!current3DdivStyle.left)
            current3DdivStyle.left = elem.style.left;
        elem.style.height = '100%';
        elem.style.width = '100%';
        elem.style.left = '0%';
        var requestMethod = elem.requestFullScreen || elem.webkitRequestFullScreen || elem.mozRequestFullScreen || elem.msRequestFullScreen;

        if (requestMethod) {
            requestMethod.call(elem);
        } else if (typeof window.ActiveXObject !== "undefined") {
            var wscript = new ActiveXObject("WScript.Shell");
            if (wscript !== null) {
                wscript.SendKeys("{F11}");
            }
        }


        //isFull = true;
        wangtobefull = true;

        setTimeout("isFull = true;", 200);
    }
    else {
        var requestMethod = document.exitFullscreen || document.mozCancelFullScreen || document.webkitCancelFullScreen || document.msExitFullscreen;

        if (requestMethod) {
            requestMethod.call(document);
            //setTimeout("isFull = false;", 200);
        }
    }
}



window.onresize = function () {
    //console.log("onresize now wangtobefull? = " + wangtobefull);
    //console.log("onresize now isFull? = " + isFull);
    if (wangtobefull) {
        var _ratio = 1;
        if (window.devicePixelRatio)
            _ratio = window.devicePixelRatio;
        bimEngine.resize(screen.width * _ratio, screen.height * _ratio);

        wangtobefull = false;
    } else {
        if (isFull) {
            var elem = document.getElementById("bimviewer");
            elem.style.height = current3DdivStyle.height;
            elem.style.width = current3DdivStyle.width;
            elem.style.left = current3DdivStyle.left;
            bimEngine.resize(elem.clientWidth, elem.clientHeight);
            isFull = false;
        }
    }
}



//点击模型只显示附近的几环模型，隐藏其他模型
function hideOtherModels(ringNum, num) {
    var hideList = [];
    var showList = [];
    for (var i = 0; i < num/2; i++) {
        var ringData01 = ringJsonData[ringNum - 1 + i];
        showList.push(ringData01.uniqueID);
        var ringData02 = ringJsonData[ringNum - 1 - i];
        showList.push(ringData02.uniqueID);
    }
    console.log(showList);
    for (var i = 0; i < ringJsonData.length; i++) {
        hideList.push(ringJsonData[i].uniqueID);
    }
    console.log(hideList);
    for (var i = 0; i < showList.length; i++) {
        hideList.remove(showList[i]);
    }
    console.log(hideList);
    bimEngine.setElementListVisible(hideList, false);
}
//显示全部模型
function showAllModel() {
    var list = [];
    for (var i = 0; i < ringJsonData.length; i++) {
        list.push(ringJsonData[i].uniqueID);
    }
    bimEngine.setElementListVisible(hideList, true);
}

//ringNum 为数字环号
function selectModelByRingNum(ringNum) {
    
    console.log(ringJsonData);
    var vm = bimEngine.getHighlightManager();
    vm.clearHighlightElementList()
    var ringData = ringJsonData[ringNum - 1];
    var vm = bimEngine.getHighlightManager();
    vm.highlightElement(ringData.uniqueID);
    selectRingToShowInfo(ringNum);
    flyToCenter(ringNum);
    //hideOtherModels(ringNum, 5);
}

//点击模型的回掉函数
function onModelClickedCallback(ringNum) {
    console.log(ringNum)
    //selectRingToShowInfo(ringNum);
   
}

function selectRingToShowInfo(ringNum) {
    showRingInfo(MetroID, ringNum);
}

function addDiseaseImg() {
    addDiseaseByJson();
}

function closeDeaDiv() {
    var elem = document.getElementById("diseaseDiv");
    elem.style.display = "none";
}

//多选功能开始
