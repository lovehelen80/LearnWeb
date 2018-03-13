using LeaRun.Application.Busines.SystemManage;
using LeaRun.Application.Code;
using LeaRun.Application.Entity.SystemManage;
using LeaRun.Application.Entity.SystemManage.ViewModel;
using LeaRun.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace LeaRun.Application.Web.Areas.GeneratorManage.Controllers
{
    /// <summary>
    /// 版 本 6.1
    /// Copyright (c) 2013-2016 上海力软信息技术有限公司
    /// 创建人：佘赐雄
    /// 日 期：2016.1.11 14:29
    /// 描 述：模板管理
    /// </summary>
    public class TemplateController : MvcControllerBase
    {
        /// <summary>
        /// 模板列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 单表生成器
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SingleTable()
        {
            return View();
        }
        /// <summary>
        /// 多表生成器
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MultiTable()
        {
            return View();
        }
        /// <summary>
        /// 创建代码（自动写入VS里面目录）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public ActionResult CreateCode()
        {
            string strcode = "";
            string strvalue = "";
            string OutputDirectory = Server.MapPath("~/Web.config");
            ;
            for (int i = 0; i < 2; i++)
                OutputDirectory = OutputDirectory.Substring(0, OutputDirectory.LastIndexOf('\\'));
            GetDataItemConst(out strcode, out strvalue);
            string codePath = OutputDirectory +
                              "\\LeaRun.Application.Entity\\SystemManage\\ViewModel\\DataItemCodes.cs";
            DirFileHelper.CreateFileContent(codePath, strcode);
            string valuePath = OutputDirectory +
                               "\\LeaRun.Application.Entity\\SystemManage\\ViewModel\\DataItemValues.cs";
            DirFileHelper.CreateFileContent(valuePath, strvalue);
            return Success("恭喜您，创建成功！");
        }

        /// <summary>
        /// 获取数据字典常量类
        /// </summary>
        /// <returns></returns>
        private void GetDataItemConst(out string strcode, out string strvalue)
        {
            var dataList = new DataItemDetailBLL().GetDataItemList();
            var sord = (from c in dataList orderby c.EnCode select c.EnCode).Distinct();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System.ComponentModel;\r\n");
            sb.AppendLine("namespace LeaRun.Application.Entity\r\n{");

            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// 数据字典值变量");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine("    public class DataItemValues");
            sb.AppendLine("    {");

            foreach (string itemSort in sord)
            {
                var dataItemList = dataList.Where(t => t.EnCode.Equals(itemSort)).OrderBy(t => t.SortCode);
                foreach (DataItemModel itemList in dataItemList)
                {
                    string pname = itemList.EnCode + "_" + itemList.ItemValue;
                    pname = Regex.Replace(pname, @"[\s\-\.\;\:]+", "");
                    sb.AppendLine("\r\n        /// <summary>");
                    sb.AppendLine("        ///  " + itemList.EnCode + "." + itemList.ItemName);
                    sb.AppendLine("        /// </summary>");
                    sb.AppendLine("        [Description(\"" + itemList.ItemName + "\")]");
                    sb.AppendLine("        public const string " + pname + " = \"" +
                                  itemList.ItemValue + "\";");
                }
            }

            sb.AppendLine("    }\r\n}");
            strvalue = sb.ToString();


            var its = new DataItemBLL().GetList();

            StringBuilder sb2 = new StringBuilder();
            sb2.AppendLine("using System.ComponentModel;\r\n");
            sb2.AppendLine("namespace LeaRun.Application.Entity\r\n{");

            sb2.AppendLine("    /// <summary>");
            sb2.AppendLine("    /// 数据字典类型常量");
            sb2.AppendLine("    /// </summary>");
            sb2.AppendLine("    public class DataItemCodes");
            sb2.AppendLine("    {");

            foreach (DataItemEntity itemList in its)
            {
                string pname = itemList.ItemCode;
                pname = Regex.Replace(pname, @"[\s\-\.\;\:]+", "");
                sb2.AppendLine("\r\n        /// <summary>");
                sb2.AppendLine("        ///  " + itemList.ItemCode + "." + itemList.ItemName);
                sb2.AppendLine("        /// </summary>");
                sb2.AppendLine("        [Description(\"" + itemList.ItemName + "\")]");
                sb2.AppendLine("        public const string " + pname + " = \"" +
                               pname + "\";");
            }

            sb2.AppendLine("    }\r\n}");
            strcode = sb2.ToString();
        }
    }
}
