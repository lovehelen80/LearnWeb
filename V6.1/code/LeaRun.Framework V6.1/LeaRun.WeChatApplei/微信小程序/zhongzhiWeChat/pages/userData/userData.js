//logs.js
var app = getApp();
var fid="";

function setMessage(that,msg)
{
  that.setData({
        message:msg
      })
}

Page({
  data: {
    paraId:"",
    sexMen:true,
    sexWomen:false,
    txtNameColor:"solid 1px #aaa",
    txtBirthDayColor:"color:#000",
    systemType: wx.getStorageSync('systemType'),
    systemCode: wx.getStorageSync('systemCode'),
    userInfo: {}
  },

  //初始化事件
  onLoad: function (data) {
    var that = this;
    //调用应用实例的方法获取全局数据
    app.getUserInfo(function(userInfo){
        //更新数据
        that.setData({
          userInfo:userInfo
          //,paraId:data.id
        })
    })
  },

  //日期更改事件
  bindDateChange: function(e) {
    this.setData({
      date: e.detail.value
    })

    if(e.detail.value!="")
    {
      this.setData({
        txtBirthDayColor:"color:#000"
      })
    }
  },

  //文本（姓名），输入事件
  txtNameInput: function(e){
    if(e.detail.value!="")
    {
      this.setData({
        txtNameColor:"solid 1px #aaa"
      })
    }
  },

  //提交事件
  formSubmit: function(e) {
    //验证数据
    if(e.detail.value.txtName=="")
    {
      this.setData({
        txtNameColor:"solid 1px red"
      })
      return;
    }
    if(e.detail.value.txtBirthDay=="")
    {
      this.setData({
        txtBirthDayColor:"color:red"
      })
      return;
    }

    //获取formId
    fid = e.detail.formId;

    //POST提交数据
    var that=this;
    wx.request({
      url: 'https://wx.bjzztz.com/api/userdata',
      data: {
        txtName: e.detail.value.txtName,
        rgSex: e.detail.value.rgSex,
        txtBirthDay: e.detail.value.txtBirthDay,
        txtRemark: e.detail.value.txtRemark
      },
      method: 'POST',
      header: {'content-type': 'application/x-www-form-urlencoded'},
      success: function(res){
        setMessage(that, JSON.stringify(res));
      },
      fail: function() {
        setMessage(that,"fail");
      }
    })
  },

  //重置事件
  formReset: function() {
    this.setData({
      sexMen:true,
      sexWomen:false,
      date:"",
      txtNameColor:"solid 1px #aaa",
      txtBirthDayColor:"color:#000"
    })
  }

})
