﻿using Newtonsoft.Json;

namespace AsNum.Xmj.API.Entity {
    /// <summary>
    /// 支持的线上发货服务
    /// </summary>
    public class SupportOnlineLogisticsService {

        [JsonProperty("logisticsServiceName")]
        public string ServiceName { get; set; }

        [JsonProperty("logisticsServiceId")]
        public string ServiceID { get; set; }

        /// <summary>
        /// 仓库地址
        /// </summary>
        [JsonProperty("deliveryAddress")]
        public string DeliveryAddress { get; set; }

        /// <summary>
        /// 运费试算
        /// </summary>
        [JsonProperty("trialResult")]
        public string Fee { get; set; }

        /// <summary>
        /// 参考运输时效
        /// </summary>
        [JsonProperty("logisticsTimeliness")]
        public string Days { get; set; }
    }
}
