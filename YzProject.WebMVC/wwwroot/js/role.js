var editor;
layui.use(['form', 'table', 'upload', 'element', 'layer', 'jquery', 'tree', 'util'], function () {
    NProgress.start();
    var form = layui.form;
    var table = layui.table;
    var layer = layui.layer;
    var laydate = layui.laydate;
    var $ = layui.jquery;
    var upload = layui.upload;
    var element = layui.element;
    var titleStr = "";
    var tree = layui.tree;
    var util = layui.util;

    var tableindex;
    getquery();
    
    function getquery(titleStr) {
        tableindex = table.render({
            elem: "#roleTable",
            url: '/Role/GetPaginatedListAsync',
            where: {
                roleName: titleStr
            },
            request: {
                pageName: 'pageIndex', // 页码的参数名称，默认：page
                limitName: 'pageSize'  // 每页数据量的参数名，默认：limit
            },
            method: 'post',
            height: 'full-210',
            toolbar: '#toolbarDemo',
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
                { field: 'roleName', title: '角色名称' },
                //{ field: 'roleCode', title: '角色编号' },
                { field: 'roleDescription', title: '角色描述' }, 
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
        table.on('tool(roleTable)', function (obj) {
            var data = obj.data;
            if (obj.event === 'delete') {
                layer.confirm("确定要删除吗？", { title: "提示" }, function (index) {
                    $.ajax({
                        url: '/Role/DeleteAsync',
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
                    area: ['700px', '400px'],
                    title: '编辑',
                    content: $('#addOrEditForm').html(),
                    success: function (layero, index) {
                       
                        //监听input文本可输入字数
                        $("#roleName").on("input", function (e) {
                            if (e.delegateTarget.value.length <= 32) {
                                var show = document.getElementById("roleNameNum");
                                show.innerHTML = Math.floor(e.delegateTarget.value.length);
                            }

                            if (e.delegateTarget.value.length > 32) {
                                $("#roleNameNum").attr("style", "color:red");
                            } else {
                                $("#roleNameNum").removeAttr("style", "color:red");
                            }
                        });

                        layero.addClass('layui-form');
                        layero.find('.layui-layer-btn0').attr('lay-filter', 'formVerify').attr('lay-submit', '');


                        $("#roleName").val(data.roleName);
                        //$("#roleCode").val(data.roleCode);
                        $("#roleDescription").val(data.roleDescription);
                        form.render();
                    },
                    btn: ['保存', '取消'],
                    yes: function (index, layero) {
                        form.on("submit(formVerify)", function () {
                            var roleName = $("#roleName").val();
                            //var roleCode = $("#roleCode").val();
                            var roleDescription = $("#roleDescription").val();
                            var id = data.id;
                            var json = {};
                            json.id = id;
                            json.roleName = roleName;
                            //json.roleCode = roleCode;
                            json.roleDescription = roleDescription;

                            $.ajax({
                                url: '/Role/AddOrEditAsync',
                                data: {
                                    param: json//JSON.stringify(json)
                                },
                                type: 'Post',
                                success: function (res) {
                                    if (res.code === 0) {
                                        // 触发隐藏按的上传按钮
                                        //$('#hideUpload').trigger('click');
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
            else if (obj.event === 'setPermissions') {
                var temindex = layer.open({
                    type: 1,
                    area: ['700px', '800px'],
                    title: '配置权限',
                    content: $('#configurePermissionsForm').html(),
                    success: function (layero, index) {
                        layero.addClass('layui-form');
                        layero.find('.layui-layer-btn0').attr('lay-filter', 'formVerify').attr('lay-submit', '');
                        //模拟数据
                        //data = [{
                        //    title: '一级2'
                        //    , id: 2
                        //    , field: ''
                        //    , spread: true
                        //    , children: [{
                        //        title: '二级2-1'
                        //        , id: 5
                        //        , field: ''
                        //        , spread: true
                        //        , checked: true
                        //    }, {
                        //        title: '二级2-2'
                        //        , id: 6
                        //        , field: ''
                        //    }]
                        //}, {
                        //    title: '一级3'
                        //    , id: 16
                        //    , field: ''
                        //    , children: [{
                        //        title: '二级3-1'
                        //        , id: 17
                        //        , field: ''
                        //        , fixed: true

                        //    }, {
                        //        title: '二级3-2'
                        //        , id: 27
                        //        , field: ''
                        //    }]
                        //}];
                        let id = data.id; 
                        $.ajax({
                            url: '/Menu/GetConfigurationPermissionListAsync',
                            data: {
                                roleId: id//JSON.stringify(json)
                            },
                            type: 'Post',
                            success: function (res) {
                                if (res.code === 0) {
                                    let data = res.data
                                    //基本演示
                                    tree.render({
                                        elem: '#test12'
                                        , data: data
                                        , showCheckbox: true  //是否显示复选框
                                        , id: 'demoId1'
                                        , isJump: false //是否允许点击节点时弹出新窗口跳转
                                        , click: function (obj) {
                                            //var data = obj.data;  //获取当前点击的节点数据
                                            //layer.msg('状态：' + obj.state + '<br>节点数据：' + JSON.stringify(data));
                                        }
                                    });
                                }
                            }
                        });
                       
                        form.render();
                    },
                    btn: ['保存', '取消'],
                    yes: function (index, layero) {

                        var checkedData = tree.getChecked('demoId1'); //获取选中节点的数据
                        //layer.alert(JSON.stringify(checkedData), { shade: 0 });
                        //console.log(checkedData);
                        //let obj = {};
                        //obj.roleId = data.id; 
                        let roleId = data.id; 
                        let nodeIds = getCheckedId(checkedData);
                        //obj.permissionIds = nodeIds;
                        //console.log(nodeIds);
                        $.ajax({
                            url: '/Menu/PermissionAuthorizeByRoleIdAsync',
                            data: {
                                //param: obj,//JSON.stringify(json)
                                roleId: roleId,
                                permissionIds:nodeIds
                            },
                            type: 'Post',
                            success: function (res) {
                                if (res.code === 0) {
                                    layer.close(temindex);
                                    //locked = true;
                                    //tableindex.reload();
                                    //刷新页面
                                    location.reload(true)
                                }
                            }
                        });

                    }

                });
            }
        });

        //监听头工具栏事件
        table.on('toolbar(roleTable)', function (obj) {
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

                            //for (var i = 0; i < data.length; i++) {
                            //    $.ajax({
                            //        url: '/Role/DeleteAsync',
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
                            var ids = data.map((item, index) => {
                                return item.id;
                            });
                            $.ajax({
                                url: '/Role/DeleteManyAsync',
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
        getquery(titleStr);
    });
    //点击刷新
    $("#btnRefresh").click(function () {
        $("#titleSearch").val("");
        getquery();
        tableindex.reload();
    });

    $("#btnadd").click(function () {
        var temindex = layer.open({
            type: 1,
            area: ['700px', '400px'],
            title: '新增',
            content: $('#addOrEditForm').html(),
            btn: ['保存', '取消'],
            success: function (layero, index) {

                //监听input文本可输入字数
                $("#roleName").on("input", function (e) {
                    if (e.delegateTarget.value.length <= 32) {
                        var show = document.getElementById("roleNameNum");
                        show.innerHTML = Math.floor(e.delegateTarget.value.length);
                    }

                    if (e.delegateTarget.value.length > 32) {
                        $("#roleNameNum").attr("style", "color:red");
                    } else {
                        $("#roleNameNum").removeAttr("style", "color:red");
                    }
                });

                layero.addClass('layui-form');
                layero.find('.layui-layer-btn0').attr('lay-filter', 'formVerify').attr('lay-submit', '');

            },
            yes: function (index, layero) {
                let locked = false;
                if (!locked) {
                    form.on('submit(formVerify)', function () {
                        var roleName = $("#roleName").val();
                        //var roleCode = $("#roleCode").val();
                        var roleDescription = $("#roleDescription").val();
                        var json = {};
                        json.roleName = roleName;
                        //json.roleCode = roleCode;
                        json.roleDescription = roleDescription;

                        $.ajax({
                            url: '/Role/AddOrEditAsync',
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

    //获取所有选中的节点id
    function getCheckedId(data) {
        var id = "";
        $.each(data, function (index, item) {
            if (id != "") {
                id = id + "," + item.id;
            }
            else {
                id = item.id;
            }
            //item 没有children属性
            if (item.children != null) {
                var i = getCheckedId(item.children);
                if (i != "") {
                    id = id + "," + i;
                }
            }
        });
        return id;
    }

});