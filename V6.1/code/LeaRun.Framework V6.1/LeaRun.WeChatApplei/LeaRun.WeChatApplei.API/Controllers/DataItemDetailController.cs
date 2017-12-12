using LeaRun.Application.Busines.SystemManage;
using LeaRun.Application.Entity.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace LeaRun.WeChatApplei.API.Controllers
{
    public class DataItemDetailController : ApiController
    {
        /// <summary>
        /// 获取通用字典数据
        /// </summary>
        /// <param name="ItemId">通用字典类型</param>
        /// <param name="isFirstItem">是否只获取第一级（1,是； 0,全部；）</param>
        /// <returns></returns>
        public string Get(string ItemId, int isFirstItem)
        {
            IEnumerable<DataItemDetailEntity> list = null;
            try
            {
                DataItemDetailBLL dataItemDetailBLL = new DataItemDetailBLL();
                list = dataItemDetailBLL.GetList(ItemId);
                if (isFirstItem == 1)
                {
                    list = list.Where(p => string.IsNullOrWhiteSpace(p.ParentId) || p.ParentId.Trim() == "0").ToArray();
                }
            }
            catch (Exception ex)
            {
            }
            return new JavaScriptSerializer().Serialize(list);
        }

    }
}
