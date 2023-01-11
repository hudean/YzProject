layui.use(['form', 'table', 'upload', 'element', 'layer', 'jquery'], function () {
    NProgress.start();
    var form = layui.form;
    var table = layui.table;
    var layer = layui.layer;
    var laydate = layui.laydate;
    var $ = layui.jquery;
    var upload = layui.upload;
    var element = layui.element;
    var titleStr = "";
    var startTimeStr = "";
    var endTimeStr = "";

    //日期范围
    laydate.render({
        elem: '#test6'
        //设置开始日期、日期日期的 input 选择器
        //数组格式为 5.3.0 开始新增，之前版本直接配置 true 或任意分割字符即可
        , range: ['#startTimeSearch', '#endTimeSerch']
    });
    var tableindex;
    getquery(titleStr, startTimeStr, endTimeStr);

    function getquery(titleStr, startTimeStr, endTimeStr) {
        tableindex = table.render({
            elem: "#excelExampleTable",
            url: '/ExcelExample/GetPaginatedListAsync',
            where: {
                webSiteName: titleStr,
                startTime: startTimeStr,
                endTime: endTimeStr
            },
            request: {
                page: 'page',
                limit: 'size'
            },
            method: 'post',
            height: 'full-210',
            toolbar: '#toolbarDemo',
            parseData: function (res) {
                return {
                    "code": res.code,
                    "msg": res.msg,
                    "count": res.data.count,
                    "data": res.data.items
                }
            },
            cols: [[
                { type: 'checkbox', title: '全选', val: 'id' },
                { field: '', type: 'numbers', title: '序号', sort: false },
                { field: 'id', title: 'ID', sort: false, hide: true },
                { field: 'name', title: '姓名' },
                { field: 'identityCardCode', title: '身份证号码' },
                { field: 'age', title: '年龄' },
                { field: 'description', title: '描述' },
                { field: 'createTime', title: '创建日期' },
                { field: '', title: '操作', templet: '#operationTpl' }
            ]],
            done: function (res, curr, count) {
                if (res.code === 1) {
                    layer.alert(res.msg);
                }
                NProgress.done();
            },
            page: true
        });
        table.on('tool(excelExampleTable)', function (obj) {
            var data = obj.data;
            if (obj.event === 'delete') {
                layer.confirm("确定要删除吗？", { title: "提示" }, function (index) {
                    $.ajax({
                        url: '/ExcelExample/DeleteAsync',
                        data: {
                            Id: data.id
                        },
                        type: 'Post',
                        success: function (res) {
                            if (res.code === 0) {
                                tableindex.reload();
                                console.log(res);
                            }
                            else {
                                layer.alert(res.msg);
                            }
                        }
                    });
                    layer.close(index);

                });
            }
            else if (obj.event === 'edit') {
                var temindex = layer.open({
                    type: 1,
                    area: ['700px', '600px'],
                    title: '编辑',
                    content: $('#addOoEditPopUpLayer').html(),
                    success: function (layero, index) {
                       
                        //监听input文本可输入字数
                        $("#name").on("input", function (e) {
                            if (e.delegateTarget.value.length <= 32) {
                                var show = document.getElementById("nameNum");
                                show.innerHTML = Math.floor(e.delegateTarget.value.length);
                            }

                            if (e.delegateTarget.value.length > 32) {
                                $("#nameNum").attr("style", "color:red");
                            } else {
                                $("#nameNum").removeAttr("style", "color:red");
                            }
                        });

                        layero.addClass('layui-form');
                        layero.find('.layui-layer-btn0').attr('lay-filter', 'formVerify').attr('lay-submit', '');


                        $("#name").val(data.name);
                        $("#identityCardCode").val(data.identityCardCode);
                        $("#age").val(data.age);
                        $("#description").val(data.description);
                        form.render();
                    },
                    btn: ['保存', '取消'],
                    yes: function (index, layero) {
                        form.on("submit(formVerify)", function () {
                            var id = data.id;
                            var name = $("#name").val();
                            var identityCardCode = $("#identityCardCode").val();
                            var age = $("#age").val();
                            var description = $("#description").val();
                           
                            var json = {};
                            json.id = id;
                            json.name = name;
                            json.identityCardCode = identityCardCode;
                            json.age = age;
                            json.description = description;

                            $.ajax({
                                url: '/ExcelExample/AddOrEditAsync',
                                data: {
                                    param: json//JSON.stringify(json)
                                },
                                type: 'Post',
                                success: function (res) {
                                    if (res.code === 0) {
                                        layer.close(temindex);
                                        tableindex.reload();
                                    }
                                    else if (res.code === 2) {
                                        window.location = res.redirect;
                                    }
                                    else {
                                        layer.alert(res.msg);
                                    }
                                }
                            });
                        });
                    }

                });
            }

        });

        //监听头工具栏事件
        table.on('toolbar(excelExampleTable)', function (obj) {
            debugger;
            var checkStatus = table.checkStatus(obj.config.id)
                , data = checkStatus.data; //获取选中的数据
            switch (obj.event) {

                case 'add':
                    layer.msg('添加');
                    break;
                case 'update':
                    if (data.length === 0) {
                        layer.msg('请选择一行');
                    } else if (data.length > 1) {
                        layer.msg('只能同时编辑一个');
                    } else {
                        layer.alert('编辑 [id]：' + checkStatus.data[0].id);
                    }
                    break;
                case 'getCheckLength':
                    if (data.length === 0) {
                        layer.msg('请选择一行');
                    }
                    else {
                        layer.confirm("确定要删除吗？", { title: "提示" }, function (index) {
                            //var ids = [];
                            //data.forEach(ids.push(data[i].id));

                            var ids = data.map((item, index) => {
                                return item.id;
                            });
                            //for (var i = 0; i < data.length; i++) {
                            //    $.ajax({
                            //        url: '/ExcelExample/DeleteAsync',
                            //        data: {
                            //            Id: data[i].id
                            //        },
                            //        type: 'Post',
                            //        success: function (res) {
                            //            if (res.code === 0) {
                            //                tableindex.reload();
                            //            }
                            //            else {
                            //                layer.alert(res.msg);
                            //            }
                            //        }
                            //    });
                            //}

                            $.ajax({
                                url: '/ExcelExample/DeleteManyAsync',
                                data: {
                                    ids: ids
                                },
                                type: 'Post',
                                success: function (res) {
                                    if (res.code === 0) {
                                        tableindex.reload();
                                        console.log(res);
                                    }
                                    else {
                                        layer.alert(res.msg);
                                    }
                                }
                            });
                            layer.msg('删除成功');
                            layer.close(index);

                        });

                    }
                    break;
            };
        });

    }

    //点击查询
    $("#btnQuery").click(function () {
        var titleStr = $("#titleSearch").val();
        var startTimeStr = $("#startTimeSearch").val();
        var endTimeStr = $("#endTimeSerch").val();
        getquery(titleStr, startTimeStr, endTimeStr);
    });
    //点击刷新
    $("#btnRefresh").click(function () {
        getquery(titleStr, startTimeStr, endTimeStr);
        tableindex.reload();
    });
    //添加
    $("#btnAdd").click(function () {
        var temindex = layer.open({
            type: 1,
            area: ['700px', '600px'],
            title: '新增',
            content: $('#addOoEditPopUpLayer').html(),
            btn: ['保存', '取消'],
            success: function (layero, index) {

                //监听input文本可输入字数
                $("#name").on("input", function (e) {
                    if (e.delegateTarget.value.length <= 32) {
                        var show = document.getElementById("nameNum");
                        show.innerHTML = Math.floor(e.delegateTarget.value.length);
                    }

                    if (e.delegateTarget.value.length > 32) {
                        $("#nameNum").attr("style", "color:red");
                    } else {
                        $("#nameNum").removeAttr("style", "color:red");
                    }
                });

                layero.addClass('layui-form');
                layero.find('.layui-layer-btn0').attr('lay-filter', 'formVerify').attr('lay-submit', '');

            },
            yes: function (index, layero) {
                let locked = false;
                if (!locked) {
                    form.on('submit(formVerify)', function () {
                        var name = $("#name").val();
                        var identityCardCode = $("#identityCardCode").val();
                        var age = $("#age").val();
                        var description = $("#description").val();
                        var json = {};
                        json.name = name;
                        json.identityCardCode = identityCardCode;
                        json.age = age;
                        json.description = description;

                        $.ajax({
                            url: '/ExcelExample/AddOrEditAsync',
                            data: {
                                param: json//JSON.stringify(json)
                            },
                            type: 'Post',
                            success: function (res) {
                                if (res.code == 0) {
                                    // 触发隐藏按的上传按钮
                                    //$('#hideUpload').trigger('click');
                                    layer.close(temindex);
                                    locked = true;
                                    tableindex.reload();
                                }
                                else if (res.code == 2) {
                                    window.location = res.redirect;
                                }
                                else {
                                    layer.alert(res.msg);
                                }
                            }
                        })
                    })
                }
            },

        })
    });
    //Excel导入
    $("#btnImport").click(function () {

    });
    //Excel导出
    $("#btnExport").click(function () {
        //$.ajax({
        //    url: '/ExcelExample/ExportExecelAsync',
        //    //data: {
        //    //    param: json//JSON.stringify(json)
        //    //},
        //    type: 'Post',
        //    datatype: "json",
        //    contentType: "application/json",
        //    responseType: "blob",
        //    async: false,
        //    success: function (res) {
        //        console.log(res);
        //         var elink = document.createElement('a');
        //         elink.download = "成绩导入结果.xls";
        //         elink.style.display = 'none';
        //        var blob = new Blob([res], { type: 'application/vnd.ms-excel' });

        //         elink.href = URL.createObjectURL(blob);
        //         document.body.appendChild(elink);
        //         elink.click();
        //         document.body.removeChild(elink);

        //        //if (res.code == 0) {
        //        //    // 触发隐藏按的上传按钮
        //        //    //$('#hideUpload').trigger('click');
        //        //    layer.close(temindex);
        //        //    locked = true;
        //        //    tableindex.reload();
        //        //}
        //        //else if (res.code == 2) {
        //        //    window.location = res.redirect;
        //        //}
        //        //else {
        //        //    layer.alert(res.msg);
        //        //}
        //    },
        //    error: function () {
        //        console.log("这是失败后的err")
        //    },
        //})

        var oReq = new XMLHttpRequest();
        oReq.open("GET", "/ExcelExample/ExportExecelAsync", true);
        oReq.responseType = "blob";
        oReq.onload = function (oEvent) {
            var content = oReq.response;

            var elink = document.createElement('a');
            elink.download = "sss.xls";
            elink.style.display = 'none';

            var blob = new Blob([content]);
            elink.href = URL.createObjectURL(blob);

            document.body.appendChild(elink);
            elink.click();

            document.body.removeChild(elink);
        };
        oReq.send();
       

    });


});