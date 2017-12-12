<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="LeaRun.WeChatApplei.API.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script src="Scripts/jquery-1.7.1.js"></script>
    <style>
        .cs {
    
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>
<script type="text/javascript">
    
    $.ajax({
        url: 'http://localhost:10580/api/CRMCustomerApply/AddCRMCustomerApply',
        type: 'POST', //GET
        async: false,    //或false,是否异步
        data: {
            ChannelId:'渠道1',
            CustName: '测试1',
            Mobile: "123",
            IDNumber: "0987654321",
            Contact: "联系人001",
            ApplyMobile: "联系人电话",
            ProductKind: "1,2,3,4,5,6,7,8,9",
            OpenId: "ouFQL0X_222An9co6_4A9GuSrC50",
            FormId: "ouFQL0X_222An9co6_4A9GuSrC50"
        },
        timeout: 5000,    //超时时间
        dataType: 'json',    //返回的数据格式：json/xml/html/script/jsonp/text
        success: function (data, textStatus, jqXHR) {
            alert(data)
            alert(textStatus)
            alert(jqXHR)
        },
        error: function (xhr, textStatus) {
            alert(textStatus);
            alert(JSON.stringify(xhr));
        }

    })
    
    /*
    $.ajax({
        url: 'http://localhost:10580/api/CRMCustomerApply/HasIDNumber?idnumber=123456',
        type: 'GET', //GET
        async: false,    //或false,是否异步
        data: {
        },
        timeout: 5000,    //超时时间
        dataType: 'json',    //返回的数据格式：json/xml/html/script/jsonp/text
        success: function (data, textStatus, jqXHR) {
            alert(data)
            alert(textStatus)
            alert(jqXHR)
        },
        error: function (xhr, textStatus) {
            alert(textStatus);
            alert(JSON.stringify(xhr));
        }

    })*/
</script>