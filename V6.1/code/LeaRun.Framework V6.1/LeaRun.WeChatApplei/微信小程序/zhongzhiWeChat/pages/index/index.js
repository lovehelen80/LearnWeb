//index.js
//获取应用实例
var app = getApp();

Page({
  data: {
    motto: '欢迎使用中致微信小程序',
    userInfo: {}
  },
  //事件处理函数
  bindViewTap: function() {
    wx.navigateTo({
      url: '/pages/CustomerApply/CustomerApply'
    })
  },
  onLoad: function (data) {
    //wx.setStorageSync("ChannelId",data.channelId);
    app.globalData.channelId=data.channelId;
    
    var that = this;
    //调用应用实例的方法获取全局数据
    app.getUserInfo(function(userInfo){
      //更新数据
      that.setData({
        userInfo:userInfo
      })
    })
  }
})
