layui.use(['form', 'table', 'layer', 'jquery'], function () {
    NProgress.start();
    var form = layui.form;
    var table = layui.table;
    var layer = layui.layer;
    var $ = layui.jquery;
    var where = "";
    var tableindex;
    getlist(where);
    function getlist(where) {
        //var tableindex
        tableindex = table.render({
            elem: "#departmentTable",
            url: '/Department/GetPaginatedListAsync',
            where: {
                departmentName: where
            },
            request: {
                page: 'page',
                limit: 'size'
            },
            method: 'post',
            toolbar: '#toolbarDemo',
            height: 'full-210',
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
                { field: 'parentId', title: 'ParentId', sort: false, hide: true },
                { field: 'departmentName', title: '部门名称' },
                { field: 'departmentCode', title: '部门编号' },
                { field: 'departmentManager', title: '部门负责人' },
                { field: 'contactNumber', title: '联系电话' },
                { field: 'introduction', title: '简介' },
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
        table.on('tool(departmentTable)', function (obj) {
            debugger;
            var data = obj.data;
            if (obj.event === 'delete') {
                // 暂时不允许删除
            }
            else if (obj.event === 'edit') {
                var temindex = layer.open({
                    type: 1,
                    area: ['700px', '460px'],
                    title: '编辑',
                    content: $('#department').html(),
                    success: function (layero, index) {
                        form.render();
                        layero.addClass('layui-form');
                        layero.find('.layui-layer-btn0').attr('lay-filter', 'formVerify').attr('lay-submit', '');
                      
                        $.ajax({
                            url: '/Department/GetAllDepartments',
                            type: 'Post',
                            async: false,
                            dataType: 'json',
                            success: function (res) {
                                if (res.code === 0) {
                                    //console.log(res.data);
                                    $.each(res.data, function (index, item) {
                                        console.log(item);
                                        $('#parentId').append(new Option(item.departmentName, item.id));
                                    });
                                }
                                else {
                                    layer.close(temindex);
                                    layer.alert(res.msg);
                                }
                                form.render();
                            }
                        });
                        $("#departmentName").val(data.departmentName);
                        $("#departmentCode").val(data.departmentCode);
                        $("#departmentManager").val(data.departmentManager);
                        $("#contactNumber").val(data.contactNumber);
                        $("#introduction").val(data.introduction);
                        $("#parentId").val(data.parentId);
                        form.render();

                    },
                    btn: ['保存', '取消'],
                    yes: function (index, layero) {
                        form.on("submit(formVerify)", function () {
                            var departmentName = $("#departmentName").val();
                            var introduction = $("#introduction").val();
                            var departmentCode = $("#departmentCode").val();
                            var departmentManager = $("#departmentManager").val();
                            var contactNumber = $("#contactNumber").val();
                            var parentId = $("#parentId").val();
                            var id = data.id;
                            var json = {};
                            json.id = id;
                            json.departmentName = departmentName;
                            json.departmentCode = departmentCode;
                            json.departmentManager = departmentManager;
                            json.contactNumber = contactNumber;
                            json.introduction = introduction;
                            json.parentId = parentId;
                            $.ajax({
                                url: '/Department/AddOrEditAsync',
                                data: {
                                    param: json
                                },
                                type: 'Post',
                                success: function (res) {
                                    if (res.code === 0) {
                                        layer.close(temindex);
                                        tableindex.reload();
                                        //console.log(res);
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
        table.on('toolbar(departmentTable)', function (obj) {
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
                    //暂时不允许删除
                    //if (data.length === 0) {
                    //    layer.msg('请选择一行');
                    //}
                    //else {
                    //    layer.confirm("确定要删除吗？", { title: "提示" }, function (index) {

                    //        for (var i = 0; i < data.length; i++) {
                    //            // alert(data[i].id);
                    //            console.log("删除的ID是：" + data.id);
                    //            $.ajax({
                    //                url: '/HospitalLevel/DeleteHospitalLevel',
                    //                data: {
                    //                    Id: data[i].id,
                    //                    type: 3
                    //                },
                    //                type: 'Post',
                    //                success: function (res) {

                    //                    if (res.data === true) {
                    //                        tableindex.reload();
                    //                        console.log(res);
                    //                    }
                    //                    else {
                    //                        layer.alert(res.msg);
                    //                    }
                    //                }
                    //            });
                    //        }
                    //        layer.msg('删除成功');
                    //        layer.close(index);

                    //    });

                    //    // layer.msg('删除');
                    //}
                    break;
            };
        });
    }

    $("#btnQuery").click(function () {
        var keyWord = $("#keyWordName").val();
        getlist(keyWord);
    });

    $("#btnRefresh").click(function () {
        getlist(where);
        tableindex.reload();
    });

    $("#btnadd").click(function () {
        var temindex = layer.open({
            type: 1,
            area: ['680px', '430px'],
            title: '新增',
            content: $('#department').html(),
            btn: ['保存', '取消'],
            success: function (layero, index) {
                form.render();
                layero.addClass('layui-form');
                layero.find('.layui-layer-btn0').attr('lay-filter', 'formVerify').attr('lay-submit', '');
                $.ajax({
                    url: '/Department/GetAllDepartments',
                    type: 'Post',
                    async: false,
                    dataType: 'json',
                    success: function (res) {
                        //console.log(res);
                        if (res.code === 0) {
                            $.each(res.data, function (index, item) {
                                $('#parentId').append(new Option(item.departmentName, item.id));
                            });
                        }
                        else {
                            layer.close(temindex);
                            layer.alert(res.msg);
                        }
                        form.render();
                    }
                });
                form.render();
            },
            yes: function (index, layero) {
                let locked = false;
                if (!locked) {
                    form.on('submit(formVerify)', function () {
                        var departmentName = $("#departmentName").val();
                        var departmentCode = $("#departmentCode").val();
                        var departmentManager = $("#departmentManager").val();
                        var contactNumber = $("#contactNumber").val();
                        var introduction = $("#introduction").val();
                        var parentId = $("#parentId").val();
                        var json = {};
                        json.departmentName = departmentName;
                        json.departmentCode = departmentCode;
                        json.departmentManager = departmentManager;
                        json.contactNumber = contactNumber;
                        json.introduction = introduction;
                        json.parentId = parentId;
                        $.ajax({
                            url: '/Department/AddOrEditAsync',
                            data: {
                                param: json
                            },
                            type: 'Post',
                            async: false,
                            success: function (res) {
                                //alert("ok");
                                //console.log(res);
                                if (res.code === 0) {
                                    locked = true;
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
                        })
                    })
                }
            },

        })
    })
    
});