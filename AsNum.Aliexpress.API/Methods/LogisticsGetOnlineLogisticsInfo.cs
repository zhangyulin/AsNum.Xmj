﻿using AsNum.Xmj.API.Attributes;
using AsNum.Xmj.API.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AsNum.Xmj.API.Methods {
    /// <summary>
    /// 获取邮政小包订单信息(线上物流发货专用接口)
    /// </summary>
    public class LogisticsGetOnlineLogisticsInfo : MethodBase<List<OnlineLogisticsInfo>> {
        protected override string APIName {
            get { return "api.getOnlineLogisticsInfo"; }
        }

        [Param("orderId")]
        public string OrderID { get; set; }

        /// <summary>
        /// 国际物流单号,即指申请的线上发货单号
        /// </summary>
        [Param("internationalLogisticsId")]
        public string TrackNO { get; set; }

        /// <summary>
        /// 国内快递单号
        /// </summary>
        [Param("chinaLogisticsId")]
        public string LocalTrackNO { get; set; }

        [EnumNameParam("logisticsStatus")]
        public OnlineLogisticStatus? Status { get; set; }

        [Param("gmtCreateStartStr")]
        public DateTime? Begin { get; set; }

        [Param("gmtCreateEndStr")]
        public DateTime? End { get; set; }

        [Param("currentPage")]
        public int? Page { get; set; }

        [Param("pageSize")]
        public int? PageSize { get; set; }

        public override List<OnlineLogisticsInfo> Execute(Auth auth) {
            var str = this.GetResult(auth);
            var o = new {
                currentPage = 1,
                success = true,
                totalPage = 1,
                result = new List<OnlineLogisticsInfo>()
            };
            o = JsonConvert.DeserializeAnonymousType(str, o);
            return o.result;
        }
    }
}
