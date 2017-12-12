using LeaRun.Application.Busines.CustomerManage;
using LeaRun.Application.Entity;
using LeaRun.Application.Entity.CustomerManage;
using LeaRun.Data;
using LeaRun.Data.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace LeaRun.WeChatApplei.API.Controllers
{
    public class CRMCustomerApplyController : ApiController
    {
        /// <summary>
        /// 根据openid，获取列表（分页）
        /// </summary>
        /// <param name="openid">openid</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        [HttpGet]
        public string GetListByOpenId(string openid, string name, int pageSize, int pageIndex)
        {
            CRMCustomerApplyResData resData = null;
            try
            {
                string where = "";
                if (!string.IsNullOrWhiteSpace(name))
                {
                    where = " and (CustName like @Name or IDNumber like @Name)";
                }

                string sqlCount = "select count(0) CC from CRM_CustomerApply C inner join Base_DataItemDetail B on B.ItemId='" + DataItemCodes.AllowStatus + "' and C.AllowStatus=B.ItemValue where OpenId=@OpenId " + where;
                string sqlData = "select * from (select ROW_NUMBER() over(order by C.ModifyDate desc) r_n,CustomerApplyId,CustomerId,CustName,IDNumber,ProductKind,ItemName from CRM_CustomerApply C inner join Base_DataItemDetail B on B.ItemId='" + DataItemCodes.AllowStatus + "' and C.AllowStatus=B.ItemValue where OpenId=@OpenId " + where + ") FF where r_n beTWeen " + (pageSize * (pageIndex - 1) + 1) + " and " + (pageSize * pageIndex) + " ORDER BY r_n";
                SqlParameter[] paras = new SqlParameter[2];
                paras[0] = new SqlParameter();
                paras[0].ParameterName = "@OpenId";
                paras[0].SqlDbType = SqlDbType.VarChar;
                paras[0].Value = openid;

                paras[1] = new SqlParameter();
                paras[1].ParameterName = "@Name";
                paras[1].SqlDbType = SqlDbType.VarChar;
                paras[1].Value = "%" + name + "%";

                object cc = DbFactory.Base().FindObject(sqlCount, paras);
                DataTable dt = DbFactory.Base().FindTable(sqlData, paras);

                if (dt != null && dt.Rows.Count > 0)
                {
                    resData = new CRMCustomerApplyResData();
                    resData.dataCount = cc == null ? 0 : Convert.ToInt32(cc);
                    resData.pageCount = (int)Math.Ceiling((double)resData.dataCount / pageSize);
                    resData.Data = new List<CRMCustomerApplyData>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        CRMCustomerApplyData data = new CRMCustomerApplyData();
                        data.CustomerApplyId = dt.Rows[i]["CustomerApplyId"].ToString();
                        data.CustomerId = dt.Rows[i]["CustomerId"].ToString();
                        data.CustName = dt.Rows[i]["CustName"].ToString();
                        data.IDNumber = dt.Rows[i]["IDNumber"].ToString();
                        data.ProductKind = dt.Rows[i]["ProductKind"].ToString();
                        data.ItemName = dt.Rows[i]["ItemName"].ToString();
                        resData.Data.Add(data);
                    }
                }
            }
            catch (Exception ex)
            { 
            }

            return new JavaScriptSerializer().Serialize(resData);
        }

        /// <summary>
        /// 判断客户身份证号是否存在
        /// </summary>
        /// <param name="idnumber">客户身份证号</param>
        /// <returns>是否存在（1,不存在； 0,参数为空； -1不存在； -2,,异常；）</returns>
        [HttpGet]
        public string HasIDNumber(string idnumber)
        {
            string result = "";
            if (string.IsNullOrWhiteSpace(idnumber))
            {
                return "0";
            }

            try
            {
                string sql = "select top 1 IDNumber from CRM_CustomerApply where IDNumber=@IDNumber";
                SqlParameter[] paras = new SqlParameter[1];
                paras[0] = new SqlParameter();
                paras[0].ParameterName = "@IDNumber";
                paras[0].SqlDbType = SqlDbType.VarChar;
                paras[0].Value = idnumber;

                object cc = DbFactory.Base().FindObject(sql, paras);
                result = cc == null ? "1" : "-1";
            }
            catch(Exception ex)
            {
                result = "-2";
            }

            return result;
        }

        /// <summary>
        /// 修改FormId
        /// </summary>
        /// <param name="CustomerApplyId">主键</param>
        /// <returns></returns>
        [HttpGet]
        public string UpFormId(string id, string formId)
        {
            int result = 0; 
            try
            {
                string sql = "update CRM_CustomerApply set FormId=@formId where CustomerApplyId=@CustomerApplyId";

                SqlParameter[] paras = new SqlParameter[2];
                paras[0] = new SqlParameter();
                paras[0].ParameterName = "@formId";
                paras[0].SqlDbType = SqlDbType.VarChar;
                paras[0].Value = formId;

                paras[1] = new SqlParameter();
                paras[1].ParameterName = "@CustomerApplyId";
                paras[1].SqlDbType = SqlDbType.VarChar;
                paras[1].Value = id;

                result = DbFactory.Base().ExecuteBySql(sql, paras);
            }
            catch (Exception ex)
            {
            }

            return (result > 0 ? 1 : 0).ToString();

        }

        
        [HttpPost]
        public string AddCRMCustomerApply([FromBody]CustomerApplyPostData data)
        {
            AddResData resData = new AddResData();
            try
            {
                #region 简单验证
                bool check = true;
                if (string.IsNullOrWhiteSpace(data.CustName))
                {
                    resData.Code = 0;
                    resData.Message = "客户姓名不能为空";
                    check = false;
                }
                else if (string.IsNullOrWhiteSpace(data.IDNumber))
                {
                    resData.Code = 0;
                    resData.Message = "客户身份证号不能为空";
                    check = false;
                }
                else if (string.IsNullOrWhiteSpace(data.Contact))
                {
                    resData.Code = 0;
                    resData.Message = "提交人姓名不能为空";
                    check = false;
                }
                else if (string.IsNullOrWhiteSpace(data.ApplyMobile))
                {
                    resData.Code = 0;
                    resData.Message = "提交人电话不能为空";
                    check = false;
                }
                else if (string.IsNullOrWhiteSpace(data.ProductKind))
                {
                    resData.Code = 0;
                    resData.Message = "期望贷款类型不能为空";
                    check = false;
                }

                if (!check)
                {
                    return new JavaScriptSerializer().Serialize(resData);
                }
                #endregion

                resData.Id = Guid.NewGuid().ToString();
                string sql = "insert into CRM_CustomerApply(CustomerApplyId,ChannelId,CustName,Mobile,IDNumber,Contact,ApplyMobile,ProductKind,OpenId,FormId,AllowStatus,CreateDate,ModifyDate) ";
                sql += "values('" + resData.Id + "',@ChannelId,@CustName,@Mobile,@IDNumber,@Contact,@ApplyMobile,@ProductKind,@OpenId,@FormId,'Todo',getdate(),getdate())";

                #region 参数化
                SqlParameter[] paras = new SqlParameter[9];
                paras[0] = new SqlParameter();
                paras[0].ParameterName = "@ChannelId";
                paras[0].SqlDbType = SqlDbType.VarChar;
                paras[0].Value = data.ChannelId;

                paras[1] = new SqlParameter();
                paras[1].ParameterName = "@CustName";
                paras[1].SqlDbType = SqlDbType.VarChar;
                paras[1].Value = data.CustName;

                paras[2] = new SqlParameter();
                paras[2].ParameterName = "@Mobile";
                paras[2].SqlDbType = SqlDbType.VarChar;
                if (data.Mobile == null)
                    paras[2].Value = DBNull.Value;
                else
                    paras[2].Value = data.Mobile;

                paras[3] = new SqlParameter();
                paras[3].ParameterName = "@IDNumber";
                paras[3].SqlDbType = SqlDbType.VarChar;
                paras[3].Value = data.IDNumber;

                paras[4] = new SqlParameter();
                paras[4].ParameterName = "@Contact";
                paras[4].SqlDbType = SqlDbType.VarChar;
                paras[4].Value = data.Contact;

                paras[5] = new SqlParameter();
                paras[5].ParameterName = "@ApplyMobile";
                paras[5].SqlDbType = SqlDbType.VarChar;
                paras[5].Value = data.ApplyMobile;

                paras[6] = new SqlParameter();
                paras[6].ParameterName = "@ProductKind";
                paras[6].SqlDbType = SqlDbType.VarChar;
                paras[6].Value = data.ProductKind;

                paras[7] = new SqlParameter();
                paras[7].ParameterName = "@OpenId";
                paras[7].SqlDbType = SqlDbType.VarChar;
                paras[7].Value = data.OpenId;

                paras[8] = new SqlParameter();
                paras[8].ParameterName = "@FormId";
                paras[8].SqlDbType = SqlDbType.VarChar;
                paras[8].Value = data.FormId;
                #endregion

                resData.Code = DbFactory.Base().ExecuteBySql(sql, paras);
            }
            catch (Exception ex)
            {
                resData.Code = -1;
                resData.Message = ex.Message;
            }

            return new JavaScriptSerializer().Serialize(resData);
        }


        //响应数据（实体集+总数据量+总页数）
        public class CRMCustomerApplyResData
        {
            public int dataCount { get; set; }
            public int pageCount { get; set; }
            public List<CRMCustomerApplyData> Data { get; set; }
        }

        //响应实体数据
        public class CRMCustomerApplyData
        {
            public string CustomerApplyId { get; set; }
            public string CustomerId { get; set; }
            public string CustName { get; set; }
            public string IDNumber { get; set; }
            public string ProductKind { get; set; }
            public string ItemName { get; set; }
        }

        //POST提交数据
        public class CustomerApplyPostData
        {
            public string ChannelId { get; set; }
            public string CustName { get; set; }
            public string Mobile { get; set; }
            public string IDNumber { get; set; }
            public string Contact { get; set; }
            public string ApplyMobile { get; set; }
            public string ProductKind { get; set; }
            public string OpenId { get; set; }
            public string FormId { get; set; }
        }

        //POST提交数据的响应体
        public class AddResData
        {
            public string Id { get; set; }
            public int Code { get; set; }
            public string Message { get; set; }
        }

    }
}
