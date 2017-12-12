//logs.js
var util = require('../../utils/util.js');
var app = getApp();
var fid;
//var sysCode = wx.getStorageSync('systemCode');

Page({
  data: {
    txtCustNameBorder:"BorderGray",
    txtMobileBorder:"BorderGray",
    txtIDNumberBorder:"BorderGray",
    txtContactBorder:"BorderGray",
    txtApplyMobileBorder:"BorderGray",
    ProductKindColor:"colorGray",
    userInfo: {}
  },

  //初始化事件
  onLoad: function (data) {
    //加载期望贷款类型
    loadProductKind(this);

    var that = this;
    //调用应用实例的方法获取全局数据
    app.getUserInfo(function(userInfo){
        //更新数据
        that.setData({
          userInfo:userInfo
        })
    })
  },


  //输入事件
  txtCustNameInput: function(e){
    if(e.detail.value!="")
    {
      this.setData({
        txtCustNameBorder:"BorderGray"
        ,message:""
      })
    }
  },
  //输入事件
  txtMobileInput: function(e){
    if(e.detail.value!="")
    {
      this.setData({
        txtMobileBorder:"BorderGray"
        ,message:""
      })
    }
  },
  //输入事件
  txtIDNumberInput: function(e){
    if(e.detail.value!="")
    {
      this.setData({
        txtIDNumberBorder:"BorderGray"
        ,message:""
      })
    }
  },
  //输入事件
  txtContactInput: function(e){
    if(e.detail.value!="")
    {
      this.setData({
        txtContactBorder:"BorderGray"
        ,message:""
      })
    }
  },
  //输入事件
  txtApplyMobileInput: function(e){
    if(e.detail.value!="")
    {
      this.setData({
        txtApplyMobileBorder:"BorderGray"
        ,message:""
      })
    }
  },
  //复选框更改事件
  cbgProductKindChange: function(e){
    if(e.detail.value!="")
    {
      this.setData({
        ProductKindColor:"colorGray"
        ,message:""
      })
    }
  },
  

  //提交事件
  formSubmit: function(e) {

    this.setData({message:""});
    //验证数据
    if(util.Trim(e.detail.value.txtCustName).length<2)//用户姓名
    {
      this.setData({
        txtCustNameBorder:"BorderRed"
      })
      return;
    }
    if(e.detail.value.txtMobile!="")//用户手机号
    {
       if(!util.CheckMobile(e.detail.value.txtMobile))
        {
          this.setData({
            txtMobileBorder:"BorderRed"
          })
          return;
        }
    }
    if(util.Trim(e.detail.value.txtIDNumber)=="")//用户身份证号码
    {
      this.setData({
        txtIDNumberBorder:"BorderRed"
      })
      return;
    }
    else if(!util.CheckIdCard(e.detail.value.txtIDNumber))
    {
      this.setData({
        txtIDNumberBorder:"BorderRed"
      })
      return;
    }
    if(util.Trim(e.detail.value.txtContact).length<2)//提交人姓名
    {
      this.setData({
        txtContactBorder:"BorderRed"
      })
      return;
    }
    if(util.Trim(e.detail.value.txtApplyMobile)=="")//提交人电话
    {
      this.setData({
        txtApplyMobileBorder:"BorderRed"
      })
      return;
    }
    else if(!util.CheckMobile(e.detail.value.txtApplyMobile))
    {
      this.setData({
        txtApplyMobileBorder:"BorderRed"
      })
      return;
    }
    if(e.detail.value.cbgProductKind=="")//贷款类型
    {
      this.setData({
        ProductKindColor:"colorRed"
      })
      return;
    }

    //获取formId
    fid = e.detail.formId;

    var that=this;
    //验证身份证号是否存在
    wx.request({
      url: 'https://wx.bjzztz.com/api/CRMCustomerApply/HasIDNumber?idnumber='+e.detail.value.txtIDNumber,
      data: {
      },
      method: 'GET',
      success: function(res){
        if(res.data=="0")
        {
          setMessage(that,"身份证号不能为空！");
        }
        else if(res.data=="-1")
        {
          setMessage(that,"当前客户身份证号已经存在！");;
        }
        else if(res.data=="-2")
        {
          setMessage(that,"身份证号验证异常，请重新再试！");
        }
        else
        {
          //POST提交数据
          PostData(that, e.detail.value, fid);
        }
        
      },
      fail: function() {
        setMessage(that,"身份证号验证失败，网络异常或服务器已关闭！");
      }
    });
    
  },

  //重置事件
  formReset: function() {
    this.setData({
      txtCustNameBorder:"BorderGray",
      txtMobileBorder:"BorderGray",
      txtIDNumberBorder:"BorderGray",
      txtContactBorder:"BorderGray",
      txtApplyMobileBorder:"BorderGray",
      ProductKindColor:"colorGray",
      message:""
    });
  },

  tapBtn:function(){
    wx.redirectTo({
      url: '/pages/CustomerApplyList/CustomerApplyList'
    });
  }

})


//加载期望贷款类型
function loadProductKind(that)
{
  wx.request({
  url: 'https://wx.bjzztz.com/api/DataItemDetail?ItemId=1361d274-c125-49a2-9f13-94eb2cfdb338&isFirstItem=1',
  data: {},
  method: 'GET',
  success: function(res){
    that.setData({ProductKindItems:JSON.parse(res.data)})
  }
  });
}

//设置错误消息函数
function setMessage(that,msg)
{
  that.setData({
        message:msg
      })
}


//提交函数
function PostData(that, value, fid)
{
    wx.request({
      url: 'https://wx.bjzztz.com/api/CRMCustomerApply/AddCRMCustomerApply',
      data: {
        ChannelId: app.globalData.channelId,
        CustName: value.txtCustName,
        Mobile: value.txtMobile,
        IDNumber: value.txtIDNumber,
        Contact: value.txtContact,
        ApplyMobile: value.txtApplyMobile,
        ProductKind: value.cbgProductKind,
        OpenId: app.globalData.openId,
        FormId: fid
      },
      method: 'POST',
      header: {'content-type': 'application/x-www-form-urlencoded'},
      success: function(res){
        var data=JSON.parse(res.data);
        if(data.Code>0)
        {
          //setMessage(that,"操作成功！");
          wx.showToast({
            title:"操作成功！",
            icon:"success",
            duration:2000,
            success:function(){
              setTimeout(
              function(){ 
                wx.redirectTo({
                url: '/pages/CustomerApplyList/CustomerApplyList'
              })
              }, 2000);
            }
          });
        }
        else if(data.Code==0)
        {
          setMessage(that,"录入失败，请重新再试！");
        }
        else if(data.Code<0)
        {
          setMessage(that,"录入失败，操作出现异常！");
        }
      },
      fail: function() {
        setMessage(that,"提交失败，网络异常或服务器已关闭！");
      }
    })
}