namespace HSZ.Common.Core.Captcha.General
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：常规验证码
    /// </summary>
    public interface IGeneralCaptcha
    {
        /// <summary>
        /// 创建验证码图片
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <param name="length">长度</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns></returns>
        byte[] CreateCaptchaImage(string timestamp, int width, int height, int length = 4);
    }
}
