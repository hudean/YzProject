var editor;
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

    var tableindex;
    getquery(titleStr);
    
    function getquery(titleStr) {
        tableindex = table.render({
            elem: "#userTable",
            url: '/User/GetPaginatedListAsync',
            where: {
                userName: titleStr
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
                { field: 'userName', title: '用户名称' },
                { field: 'hardImgUrl', title: '用户头像' },
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
        table.on('tool(userTable)', function (obj) {
            var data = obj.data;
            if (obj.event === 'delete') {
                layer.confirm("确定要删除吗？", { title: "提示" }, function (index) {
                    $.ajax({
                        url: '/User/DeleteAsync',
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
                    content: $('#userEdit').html(),
                    success: function (layero, index) {


                        //常规使用 - 普通图片上传
                        var uploadInst = upload.render({
                            elem: '#imgUpload'           //绑定点击按钮
                            , url: '/User/UploadImgAsync'      //上传接口
                            , accept: 'images'                //图片格式
                            , auto: false                     //取消自动上传
                            , bindAction: '#hideUpload'       //绑定真正的上传按钮
                            , data: { action: "edit" }
                            , choose: function (obj) {
                                //预读本地文件示例，不支持ie8
                                obj.preview(function (index, file, result) {
                                    $('#hardImgUrl').attr('src', result); //图片链接（base64）
                                });
                            }
                            , before: function (obj) {
                            }
                            , done: function (res) {
                                //如果上传失败
                                if (res.code > 0) {
                                    return layer.alert(res.msg);
                                }
                                //上传成功的一些操作
                                //……
                                $('#demoText').html(''); //置空上传失败的状态
                                tableindex.reload();
                            }
                            , error: function () {
                                //演示失败状态，并实现重传
                                var demoText = $('#demoText');
                                demoText.html('<span style="color: #FF5722;">上传失败</span> <a class="layui-btn layui-btn-xs demo-reload">重试</a>');
                                demoText.find('.demo-reload').on('click', function () {
                                    uploadInst.upload();
                                });
                            }
                        });

                        layero.addClass('layui-form');
                        layero.find('.layui-layer-btn0').attr('lay-filter', 'formVerify').attr('lay-submit', '');


                        $("#userName").val(data.userName);
                        $("#hardImgUrl").attr("src", data.hardImgUrl);
                        form.render();
                    },
                    btn: ['保存', '取消'],
                    yes: function (index, layero) {
                        form.on("submit(formVerify)", function () {
                            var userName = $("#userName").val();
                            //var passWord = $("#passWord").val();
                            //var confirmPassword = $("#confirmPassword").val();

                            //if (passWord != confirmPassword) {
                            //    layer.alert("输入的密码不一致");
                            //    return;
                            //}
                            var id = data.id;
                            var json = {};
                            json.id = id;
                            json.hardImgUrl = data.hardImgUrl;
                            json.userName = userName;
                            //json.passWord = passWord;
                            //json.confirmPassword = confirmPassword;

                            $.ajax({
                                url: '/User/AddOrEditAsync',
                                data: {
                                    param: json//JSON.stringify(json)
                                },
                                type: 'Post',
                                success: function (res) {
                                    if (res.code === 0) {
                                        // 触发隐藏按的上传按钮
                                        $('#hideUpload').trigger('click');
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
            else if (obj.event === 'editPassWord') {
                var temindex = layer.open({
                    type: 1,
                    area: ['700px', '600px'],
                    title: '编辑',
                    content: $('#userEditPassWord').html(),
                    success: function (layero, index) {

                        layero.addClass('layui-form');
                        layero.find('.layui-layer-btn0').attr('lay-filter', 'formVerify').attr('lay-submit', '');

                        form.render();
                    },
                    btn: ['保存', '取消'],
                    yes: function (index, layero) {
                        form.on("submit(formVerify)", function () {
                            var passWord = $("#passWord").val();
                            var confirmPassword = $("#confirmPassword").val();

                            if (passWord != confirmPassword) {
                                layer.alert("输入的密码不一致");
                                return;
                            }
                            var id = data.id;
                            var json = {};
                            json.id = id;
                            json.passWord = passWord;
                            json.confirmPassword = confirmPassword;

                            $.ajax({
                                url: '/User/EditPassWordAsync',
                                data: {
                                    param: json//JSON.stringify(json)
                                },
                                type: 'Post',
                                success: function (res) {
                                    if (res.code === 0) {
                                        layer.alert("密码修改成功");
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
            } else if (obj.event === 'setRole') {
                var temindex = layer.open({
                    type: 1,
                    area: ['700px', '600px'],
                    title: '设置角色',
                    content: $('#setRoleHtml').html(),
                    success: function (layero, index) {

                        layero.addClass('layui-form');
                        layero.find('.layui-layer-btn0').attr('lay-filter', 'formVerify').attr('lay-submit', '');
                        let id = data.id;
                        $.ajax({
                            url: '/Role/GetConfigurationRoleListAsync',
                            data: {
                                userId: id//JSON.stringify(json)
                            },
                            type: 'Post',
                            async: true,
                            success: function (res) {
                               
                                if (res.code === 0) {
                                    if (res.data && res.data.length > 0) {
                                        $("#setRoleFrom").children().remove();
                                        //$("#setRoleFrom").empty();
                                        let str = "";
                                        let rowItem = res.data.length / 8;
                                       
                                        for (var i = 0; i <= rowItem; i++) {
                                            str += '<div class="layui-form-item">'
                                            
                                            for (var j = 0; j < 8; j++) {
                                                let index = i * 8 + j;
                                                if (index >= res.data.length) {
                                                    break;
                                                }
                                                let item = res.data[index];
                                                //console.log(item);
                                                if (item.isChecked) {
                                                    str += '<input type="checkbox" value="' + item.id + '" name="roleChecck" title="' + item.roleName + '" lay-skin="primary" checked>';
                                                } else {
                                                    str += '<input type="checkbox" value="' + item.id + '" name="roleChecck" title="' + item.roleName + '" lay-skin="primary">';
                                                }
                                            }
                                            str += ' </div>';
                                        }
                                        $("#setRoleFrom").append(str);
                                        form.render();
                                    }

                                 
                                }
                            }
                        });

                        form.render();
                    },
                    btn: ['保存', '取消'],
                    yes: function (index, layero) {
                        let userId = data.id; 
                        //var spCodesTemp = "";
                        let ids = [];
                        $('input:checkbox[name=roleChecck]:checked').each(function (i) {
                            //if (0 == i) {
                            //    spCodesTemp = $(this).val();
                            //} else {
                            //    spCodesTemp += ("," + $(this).val());
                            //}
                            ids.push($(this).val());
                        });
                        //$("#txt_spCodes").val(spCodesTemp);
                        //console.log("ids", ids);
                        $.ajax({
                            url: '/Role/SetRolesByUserIdAsync',
                            data: {
                                //param: obj,//JSON.stringify(json)
                                userId: userId,
                                roleIds: ids
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
        table.on('toolbar(userTable)', function (obj) {
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

                            for (var i = 0; i < data.length; i++) {
                                $.ajax({
                                    url: '/User/DeleteAsync',
                                    data: {
                                        Id: data[i].id
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
                            }
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
        getquery(titleStr);
        tableindex.reload();
    });

    $("#btnadd").click(function () {
        var temindex = layer.open({
            type: 1,
            area: ['700px', '600px'],
            title: '新增',
            content: $('#user').html(),
            btn: ['保存', '取消'],
            success: function (layero, index) {

                //常规使用 - 普通图片上传
                var uploadInst = upload.render({
                    elem: '#imgUpload'           //绑定点击按钮
                    , url: '/User/UploadImgAsync'      //上传接口
                    , accept: 'images'                //图片格式
                    , auto: false                     //取消自动上传
                    , bindAction: '#hideUpload'       //绑定真正的上传按钮
                    , data: { action: "add" }
                    , choose: function (obj) {
                        //预读本地文件示例，不支持ie8
                        obj.preview(function (index, file, result) {
                            $('#hardImgUrl').attr('src', result); //图片链接（base64）
                        });
                    }
                    , before: function (obj) {
                    }
                    , done: function (res) {
                        //如果上传失败
                        if (res.code > 0) {
                            return layer.alert(res.msg);
                        }
                        //上传成功的一些操作
                        //……
                        $('#demoText').html(''); //置空上传失败的状态
                        tableindex.reload();
                    }
                    , error: function () {
                        //演示失败状态，并实现重传
                        var demoText = $('#demoText');
                        demoText.html('<span style="color: #FF5722;">上传失败</span> <a class="layui-btn layui-btn-xs demo-reload">重试</a>');
                        demoText.find('.demo-reload').on('click', function () {
                            uploadInst.upload();
                        });
                    }
                });


                layero.addClass('layui-form');
                layero.find('.layui-layer-btn0').attr('lay-filter', 'formVerify').attr('lay-submit', '');

            },
            yes: function (index, layero) {
                let locked = false;
                if (!locked) {
                    form.on('submit(formVerify)', function () {

                        var userName = $("#userName").val();
                        var passWord = $("#passWord").val();
                        var confirmPassword = $("#confirmPassword").val();
                        var json = {};
                        json.userName = userName;
                        json.passWord = passWord;
                        json.confirmPassword = confirmPassword;
                       

                        $.ajax({
                            url: '/User/AddOrEditAsync',
                            data: {
                                param: json//JSON.stringify(json)
                            },
                            type: 'Post',
                            success: function (res) {
                                if (res.code == 0) {
                                    // 触发隐藏按的上传按钮
                                    $('#hideUpload').trigger('click');
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