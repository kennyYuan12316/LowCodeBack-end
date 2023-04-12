namespace HSZ.Message.Entitys.Dto.IM
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：信息图片输入
    /// </summary>
    public class MessagetImageInput
    {
        /// <summary>
        /// 64进制图片
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        public int height { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public int width { get; set; }
    }
}
