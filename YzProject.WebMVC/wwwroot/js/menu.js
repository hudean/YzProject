layui.use(['form', 'table', 'upload', 'element', 'layer', 'jquery'], function () {
    NProgress.start();
    var form = layui.form;
    var table = layui.table;
    var layer = layui.layer;
    var $ = layui.jquery;
    var upload = layui.upload;
    var element = layui.element;
    var serachMenuNameVal = "";
    var serachFunctionTypeVal = "";
    var tableindex;
    getquery(serachMenuNameVal, serachFunctionTypeVal);
   
    function getquery(serachMenuNameStr, serachFunctionTypeStr)
    {
          tableindex = table.render({
            elem: "#menuTable",
              url: '/Menu/GetPaginatedListAsync',
            where: {
                menuName: serachMenuNameStr,
                menuType: serachFunctionTypeStr
            },
            request: {
                pageName: 'pageIndex', // 页码的参数名称，默认：page
                limitName: 'pageSize'  // 每页数据量的参数名，默认：limit
            },
            method: 'post',
            toolbar: '#toolbarDemo',
            height: 'full-210',
            parseData: function (res) {
                return {
                    "code": res.code,
                    "msg": res.msg,
                    "count": res.data.totalCount,
                    "data": res.data.items
                }
            },
            cols: [[
                { type: 'checkbox', title: '全选', val: 'id' },
                { field: '', type: 'numbers', title: '序号', sort: false },
                { field: 'id', title: 'ID', sort: false, hide: true },
                { field: 'parentId', title: '父级ID', sort: false, hide: false },
                { field: 'menuCode', title: '菜单编码' },
                { field: 'menuName', title: '菜单名称' },
                { field: 'serialNumber', title: '排序号', hide: false },
                { field: 'url', title: '菜单地址' },
                { field: 'menuType', title: '菜单类型' },
                { field: 'icon', title: '菜单图标' },
                { field: 'remarks', title: '菜单备注' },
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
        table.on('tool(menuTable)', function (obj) {
            var data = obj.data;
            if (obj.event === 'delete') {
                layer.confirm("确定要删除吗？", { title: "提示" }, function (index) {
                    $.ajax({
                        url: '/Menu/DeleteAsync',
                        data: {
                            Id: data.id
                        },
                        type: 'Post',
                        success: function (res) {
                            if (res.code === 0) {
                                tableindex.reload();
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
                    content: $('#menuAddOrEdidForm').html(),
                    success: function (layero, index) {
                        layero.addClass('layui-form');
                        layero.find('.layui-layer-btn0').attr('lay-filter', 'formVerify').attr('lay-submit', '');
                        $("#menuCode").val(data.menuCode);
                        $("#menuName").val(data.menuName);
                        $("#menuType").val(data.menuType);
                        $("#serialNumber").val(data.serialNumber);
                        $("#url").val(data.url);
                        $("#remarks").val(data.remarks);
                        $.ajax({
                            url: '/Menu/GetOneLevelMenuListAsync',
                            type: 'Post',
                            dataType: 'json',
                            async: false,
                            success: function (res) {
                                if (res.code === 0) {
                                    $.each(res.data, function (index, item) {
                                        $('#parentId').append(new Option(item.menuName, item.id));
                                    });
                                }
                                else {
                                    layer.close(temindex);
                                    layer.alert(res.msg);
                                }
                                form.render();
                            }
                        });
                        $("#parentId").val(data.parentId);
                        form.render();
                    },
                    btn: ['保存', '取消'],
                    yes: function (index, layero) {
                        form.on("submit(formVerify)", function () {
                            var menuCode = $("#menuCode").val();
                            var menuName = $("#menuName").val();
                            var menuType = $("#menuType").val();
                            var serialNumber = $("#serialNumber").val();
                            var url = $("#url").val();
                            var parentId = $("#parentId").val();
                            var remarks = $("#remarks").val();
                            var id = data.id;

                            var json = {};
                            json.id = id;
                            json.menuCode = menuCode;
                            json.menuName = menuName;
                            json.menuType = menuType;
                            json.serialNumber = serialNumber;
                            json.url = url;
                            json.parentId = parentId;
                            json.remarks = remarks;
                          
                            $.ajax({
                                url: '/Menu/AddOrEditAsync',
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
        table.on('toolbar(menuTable)', function (obj) {
            //debugger;
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
                            //循环单个删除
                            //for (var i = 0; i < data.length; i++) {
                            //    $.ajax({
                            //        url: '/Menu/DeleteAsync',
                            //        data: {
                            //            Id: data[i].id,
                            //            type: 3
                            //        },
                            //        type: 'Post',
                            //        success: function (res) {
                            //            if (res.data === true) {
                            //                tableindex.reload();
                            //            }
                            //            else {
                            //                layer.alert(res.msg);
                            //            }
                            //        }
                            //    });
                            //}
                            //批量删除
                            var ids = data.map((item, index) => {
                                return item.id;
                            });
                            $.ajax({
                                url: '/Menu/DeleteManyAsync',
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
    $("#btnQuery").click(function () {
        var serachMenuNameVal = $("#serachMenuName").val();
        var serachFunctionTypeVal = $("#serachFunctionType").val();
        getquery(serachMenuNameVal, serachFunctionTypeVal);
    });
    $("#btnRefresh").click(function () {
        getquery();
        tableindex.reload();
    });

    $("#btnAdd").click(function () {
        var temindex = layer.open({
            type: 1,
            area: ['700px', '600px'],
            title: '新增',
            content: $('#menuAddOrEdidForm').html(),
            btn: ['保存', '取消'],
            success: function (layero, index) {
                layero.addClass('layui-form');
                layero.find('.layui-layer-btn0').attr('lay-filter', 'formVerify').attr('lay-submit', '');

                $.ajax({
                    url: '/Menu/GetOneLevelMenuListAsync',
                    type: 'Post',
                    dataType: 'json',
                    success: function (res) {
                        if (res.code === 0) {
                            $.each(res.data, function (index, item) {
                                $('#parentId').append(new Option(item.menuName, item.id));
                            });
                        }
                        else {
                            layer.close(temindex);
                            layer.alert(res.msg);
                        }
                        form.render();
                    }
                });
            },
            yes: function (index, layero) {
                let locked = false;
                if (!locked) {
                    form.on('submit(formVerify)', function () {
                        var menuCode = $("#menuCode").val();
                        var menuName = $("#menuName").val();
                        var menuType = $("#menuType").val();
                        var serialNumber = $("#serialNumber").val();
                        var url = $("#url").val();
                        var parentId = $("#parentId").val();
                        var remarks = $("#remarks").val();

                        var json = {};
                        json.menuCode = menuCode;
                        json.menuName = menuName;
                        json.menuType = menuType;
                        json.serialNumber = serialNumber;
                        json.url = url;
                        json.parentId = parentId;
                        json.remarks = remarks;

                        $.ajax({
                            url: '/Menu/AddOrEditAsync',
                            data: {
                                param: json//JSON.stringify(json)
                            },
                            type: 'Post',
                            success: function (res) {
                                if (res.code == 0) {
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
    })
});