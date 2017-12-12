//app.js

App({
  onLaunch: function () {
    //调用API从本地缓存中获取数据
    var logs = wx.getStorageSync('logs') || []
    logs.unshift(Date.now())
    wx.setStorageSync('logs', logs)
  },
  getUserInfo:function(cb){
    var that = this
    if(this.globalData.userInfo){
      typeof cb == "function" && cb(this.globalData.userInfo)
    }else{
      //调用登录接口
      wx.login({
        success: function (loginRes) {
          that.globalData.systemCode=loginRes.code;
          //wx.setStorageSync("systemCode",loginRes.code);
          
          //获取openid、sessionkey
          wx.request({
            url: 'https://wx.bjzztz.com/api/WeChatRequest/GetOpenId?code='+loginRes.code,
            data: {},
            method: 'GET',
            success: function(res){
              var data=JSON.parse(res.data);
              that.globalData.sessionkey=data.session_key;
              that.globalData.openId=data.openid;
            },
            fail: function() {
              that.globalData.openId="";
              that.globalData.sessionkey="";
            }
          });

          //获取用户个人信息
          wx.getUserInfo({
            success: function (res) {
              that.globalData.userInfo = res.userInfo
              typeof cb == "function" && cb(that.globalData.userInfo)
            }
          });

        }
      })
    }
  },
  globalData:{
    userInfo:null,
    channelId:"",
    systemCode:"",
    openId:"",
    sessionkey:""
  }
})