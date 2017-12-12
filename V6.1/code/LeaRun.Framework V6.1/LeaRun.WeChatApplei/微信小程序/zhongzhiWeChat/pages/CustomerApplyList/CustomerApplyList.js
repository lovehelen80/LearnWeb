//logs.js
var util = require('../../utils/util.js');
var app = getApp();
var txtName="";
var pagesize=10;
var pageIndex=1;
var pageCount=0;
var dataArr=[];
var dictArr=null;

Page({
  data: {
      listData:{}
  },

  //初始化事件
  onLoad: function (data) {
    //加载字典
    wx.request({
        url: 'https://wx.bjzztz.com/api/DataItemDetail?ItemId=1361d274-c125-49a2-9f13-94eb2cfdb338&isFirstItem=1',
        data: {},
        method: 'GET',
        success: function(res){
            dictArr=JSON.parse(res.data);
        }
    });

    //加载数据
    SearchData(this,"");
  },
  
  //提交事件
  formSubmit: function(e) {
    txtName=e.detail.value.txtName;
    //加载数据
    SearchData(this, txtName);
  },

  //跳转事件
  tapBtn: function() {
    wx.redirectTo({
      url: '/pages/CustomerApply/CustomerApply'
    });
  },
  
  //滚动事件
  scrollLower: function(e){
    if(pageIndex < pageCount)
    {
        var that=this;
        pageIndex++;
        wx.showLoading({
            title:"正在加载数据..."
        });

        wx.request({
            url: 'https://wx.bjzztz.com/api/CRMCustomerApply/GetListByOpenId?openid='+app.globalData.openId+'&name='+txtName+'&pageSize='+pagesize+'&pageIndex='+pageIndex,
            data: {
            },
            method: 'GET',
            success: function(res){
                var data=JSON.parse(res.data);
                
                if(data!=null)
                {
                    var arr=ExpandData(data.Data);
                    dataArr=dataArr.concat(arr);
                    //ExpandData();//扩展数据
                    that.setData({
                        listData: dataArr
                    });
                    pageCount=data.pageCount;
                    setMessage(that,"");
                }
            },
            complete:function(){
                wx.hideLoading();
            }
        });
    }
  }

})


//搜索数据
function SearchData(that, name)
{
    wx.showLoading({
        title:"正在加载数据..."
    });

    wx.request({
      url: 'https://wx.bjzztz.com/api/CRMCustomerApply/GetListByOpenId?openid='+app.globalData.openId+'&name='+name+'&pageSize='+(pagesize)+'&pageIndex=1',
      data: {
      },
      method: 'GET',
      success: function(res){
        var data=JSON.parse(res.data);
        if(data!=null)
        {
            dataArr=data.Data;
            dataArr=ExpandData(dataArr);//扩展数据
            that.setData({
                listData: dataArr
            });
            pageCount=data.pageCount;
            pageIndex=1;
            setMessage(that,"");
        }
        else
        {
            that.setData({
                listData: {}
            });
            setMessage(that,"暂无数据！");
        }
      },
      fail: function() {
        setMessage(that,"加载出现异常！");
      },
      complete:function(){
          wx.hideLoading();
      }
    });
}


//扩展数据
function ExpandData(dataArr)
{
    if(dictArr==null || dataArr==null || dataArr.length==0 || dictArr.length==0)
        return;

    var str;
    for(var i=0;i<dataArr.length;i++)
    {
        str="";
        var arr=dataArr[i].ProductKind.split(',');
        for(var x=0;x<arr.length;x++)
        {
            for(var y=0; y<dictArr.length;y++)
            {
                if(arr[x]==dictArr[y].ItemValue)
                {
                    if(str!="")
                    {
                        str+=",";
                    }
                    str+=dictArr[y].ItemName;
                }
            }
        }

        dataArr[i].ProductKind=str;
    }
    
    return dataArr;
}


//设置错误消息函数
function setMessage(that,msg)
{
  that.setData({
        message:msg
      })
}
