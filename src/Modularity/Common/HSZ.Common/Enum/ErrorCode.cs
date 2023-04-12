using HSZ.FriendlyException;

namespace HSZ.Common.Enum
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：系统错误码
    /// </summary>
    [ErrorCodeType]
    public enum ErrorCode
    {
        #region 系统类型

        #region Auth 1

        /// <summary>
        /// 用户名或密码不正确
        /// </summary>
        [ErrorCodeItemMetadata("用户名或密码不正确")]
        D1000,

        /// <summary>
        /// 非法操作！禁止删除自己
        /// </summary>
        [ErrorCodeItemMetadata("非法操作，禁止删除自己")]
        D1001,

        /// <summary>
        /// 记录不存在
        /// </summary>
        [ErrorCodeItemMetadata("记录不存在")]
        D1002,

        /// <summary>
        /// 账号已存在
        /// </summary>
        [ErrorCodeItemMetadata("账号已存在")]
        D1003,

        /// <summary>
        /// 旧密码不匹配
        /// </summary>
        [ErrorCodeItemMetadata("旧密码输入错误")]
        D1004,

        /// <summary>
        /// 测试数据禁止更改admin密码
        /// </summary>
        [ErrorCodeItemMetadata("测试数据禁止更改用户【admin】密码")]
        D1005,

        /// <summary>
        /// 数据已存在
        /// </summary>
        [ErrorCodeItemMetadata("数据已存在")]
        D1006,

        /// <summary>
        /// 数据不存在或含有关联引用，禁止删除
        /// </summary>
        [ErrorCodeItemMetadata("数据不存在或含有关联引用，禁止删除")]
        D1007,

        /// <summary>
        /// 禁止为管理员分配角色
        /// </summary>
        [ErrorCodeItemMetadata("禁止为管理员分配角色")]
        D1008,

        /// <summary>
        /// 重复数据或记录含有不存在数据
        /// </summary>
        [ErrorCodeItemMetadata("重复数据或记录含有不存在数据")]
        D1009,

        /// <summary>
        /// 禁止为超级管理员角色分配权限
        /// </summary>
        [ErrorCodeItemMetadata("禁止为超级管理员角色分配权限")]
        D1010,

        /// <summary>
        /// 非法数据
        /// </summary>
        [ErrorCodeItemMetadata("非法数据")]
        D1011,

        /// <summary>
        /// Id不能为空
        /// </summary>
        [ErrorCodeItemMetadata("Id不能为空")]
        D1012,

        /// <summary>
        /// 所属机构不在自己的数据范围内
        /// </summary>
        [ErrorCodeItemMetadata("没有权限操作该数据")]
        D1013,

        /// <summary>
        /// 禁止删除超级管理员
        /// </summary>
        [ErrorCodeItemMetadata("禁止删除超级管理员")]
        D1014,

        /// <summary>
        /// 禁止修改超级管理员状态
        /// </summary>
        [ErrorCodeItemMetadata("禁止修改超级管理员状态")]
        D1015,

        /// <summary>
        /// 没有权限
        /// </summary>
        [ErrorCodeItemMetadata("没有权限")]
        D1016,

        /// <summary>
        /// 账号被删除
        /// </summary>
        [ErrorCodeItemMetadata("账号被删除")]
        D1017,

        /// <summary>
        /// 账号未被激活
        /// </summary>
        [ErrorCodeItemMetadata("账号未被激活")]
        D1018,

        /// <summary>
        /// 账号被禁用
        /// </summary>
        [ErrorCodeItemMetadata("账号被禁用")]
        D1019,

        /// <summary>
        /// 账号在其他地方登录
        /// </summary>
        [ErrorCodeItemMetadata("账号在其他地方登录")]
        D1020,

        /// <summary>
        /// 直属主管不能为自己
        /// </summary>
        [ErrorCodeItemMetadata("直属主管不能为自己")]
        D1021,

        /// <summary>
        /// 账号权限不足,请联系管理员
        /// </summary>
        [ErrorCodeItemMetadata("账号权限不足,请联系管理员")]
        D1022,

        /// <summary>
        /// 租户已到期
        /// </summary>
        [ErrorCodeItemMetadata("租户已到期")]
        D1023,

        /// <summary>
        /// 租户不存在
        /// </summary>
        [ErrorCodeItemMetadata("租户不存在")]
        D1024,

        /// <summary>
        /// 租户登录失败，请用手机验证码登录
        /// </summary>
        [ErrorCodeItemMetadata("租户登录失败，请用手机验证码登录")]
        D1025,

        /// <summary>
        /// 直属主管不能为自己的下属
        /// </summary>
        [ErrorCodeItemMetadata("直属主管不能为自己的下属")]
        D1026,

        /// <summary>
        /// 账号被锁定提示
        /// </summary>
        [ErrorCodeItemMetadata("请等待{0}分钟后再进行登录，或联系管理员解除账号锁定！")]
        D1027,

        /// <summary>
        /// 验证码错误
        /// </summary>
        [ErrorCodeItemMetadata("验证码输入错误，请重新输入！")]
        D1029,

        /// <summary>
        /// 验证码已失效
        /// </summary>
        [ErrorCodeItemMetadata("验证码已失效，请重新输入！")]
        D1030,

        /// <summary>
        /// 请联系管理员解除账号锁定！
        /// </summary>
        [ErrorCodeItemMetadata("请联系管理员解除账号锁定！")]
        D1031,

        #endregion

        #region 机构 2

        /// <summary>
        /// 父机构不存在
        /// </summary>
        [ErrorCodeItemMetadata("父机构不存在")]
        D2000,

        /// <summary>
        /// 当前机构Id不能与父机构Id相同
        /// </summary>
        [ErrorCodeItemMetadata("当前机构Id不能与父机构Id相同")]
        D2001,

        /// <summary>
        /// 机构不存在
        /// </summary>
        [ErrorCodeItemMetadata("机构不存在")]
        D2002,

        /// <summary>
        /// 机构主管不能删除
        /// </summary>
        [ErrorCodeItemMetadata("机构主管不能删除")]
        D2003,

        /// <summary>
        /// 附属机构下有员工禁止删除
        /// </summary>
        [ErrorCodeItemMetadata("附属机构下有员工禁止删除")]
        D2004,

        /// <summary>
        /// 该机构下有机构禁止删除
        /// </summary>
        [ErrorCodeItemMetadata("该机构下有机构禁止删除")]
        D2005,

        /// <summary>
        /// 该机构下有岗位禁止删除
        /// </summary>
        [ErrorCodeItemMetadata("该机构下有岗位禁止删除")]
        D2006,

        /// <summary>
        /// 只能增加下级机构
        /// </summary>
        [ErrorCodeItemMetadata("只能增加下级机构")]
        D2007,

        /// <summary>
        /// 机构编码已存在
        /// </summary>
        [ErrorCodeItemMetadata("机构编码已存在")]
        D2008,

        /// <summary>
        /// 机构名称已存在
        /// </summary>
        [ErrorCodeItemMetadata("机构名称已存在")]
        D2009,

        /// <summary>
        /// 更新机构失败
        /// </summary>
        [ErrorCodeItemMetadata("更新机构失败")]
        D2010,

        /// <summary>
        /// 更新机构状态失败
        /// </summary>
        [ErrorCodeItemMetadata("更新机构状态失败")]
        D2011,

        /// <summary>
        /// 删除机构失败
        /// </summary>
        [ErrorCodeItemMetadata("删除机构失败")]
        D2012,

        /// <summary>
        /// 新增机构失败
        /// </summary>
        [ErrorCodeItemMetadata("新增机构失败")]
        D2013,

        /// <summary>
        /// 部门编码已存在
        /// </summary>
        [ErrorCodeItemMetadata("部门编码已存在")]
        D2014,

        /// <summary>
        /// 部门名称已存在
        /// </summary>
        [ErrorCodeItemMetadata("部门名称已存在")]
        D2019,

        /// <summary>
        /// 新增部门失败
        /// </summary>
        [ErrorCodeItemMetadata("新增部门失败")]
        D2015,

        /// <summary>
        /// 更新部门状态失败
        /// </summary>
        [ErrorCodeItemMetadata("更新部门状态失败")]
        D2016,

        /// <summary>
        /// 删除部门失败
        /// </summary>
        [ErrorCodeItemMetadata("删除部门失败")]
        D2017,

        /// <summary>
        /// 更新部门失败
        /// </summary>
        [ErrorCodeItemMetadata("更新部门失败")]
        D2018,

        /// <summary>
        /// 该机构下有角色禁止删除
        /// </summary>
        [ErrorCodeItemMetadata("该机构下有角色禁止删除")]
        D2020,

        #endregion

        #region 数据字典 3

        /// <summary>
        /// 字典类型不存在
        /// </summary>
        [ErrorCodeItemMetadata("字典类型不存在")]
        D3000,

        /// <summary>
        /// 字典类型已存在
        /// </summary>
        [ErrorCodeItemMetadata("字典类型已存在,名称或编码重复")]
        D3001,

        /// <summary>
        /// 字典类型下面有字典值禁止删除
        /// </summary>
        [ErrorCodeItemMetadata("字典类型下面有字典值禁止删除")]
        D3002,

        /// <summary>
        /// 字典值已存在
        /// </summary>
        [ErrorCodeItemMetadata("字典值已存在,名称或编码重复")]
        D3003,

        /// <summary>
        /// 字典值不存在
        /// </summary>
        [ErrorCodeItemMetadata("字典值不存在")]
        D3004,

        /// <summary>
        /// 字典状态错误
        /// </summary>
        [ErrorCodeItemMetadata("字典状态错误")]
        D3005,

        /// <summary>
        /// 导入文件格式错误
        /// </summary>
        [ErrorCodeItemMetadata("导入文件格式错误")]
        D3006,

        /// <summary>
        /// 上级分类不存在,无法导入
        /// </summary>
        [ErrorCodeItemMetadata("上级分类不存在,无法导入")]
        D3007,

        /// <summary>
        /// 导入失败
        /// </summary>
        [ErrorCodeItemMetadata("导入失败")]
        D3008,

        /// <summary>
        /// 导入文件功能类型错误
        /// </summary>
        [ErrorCodeItemMetadata("导入文件功能类型错误")]
        D3009,

        #endregion

        #region 菜单 4

        /// <summary>
        /// 菜单已存在
        /// </summary>
        [ErrorCodeItemMetadata("菜单已存在")]
        D4000,

        /// <summary>
        /// 路由地址为空
        /// </summary>
        [ErrorCodeItemMetadata("路由地址为空")]
        D4001,

        /// <summary>
        /// 打开方式为空
        /// </summary>
        [ErrorCodeItemMetadata("打开方式为空")]
        D4002,

        /// <summary>
        /// 权限标识格式为空
        /// </summary>
        [ErrorCodeItemMetadata("权限标识格式为空")]
        D4003,

        /// <summary>
        /// 权限标识格式错误
        /// </summary>
        [ErrorCodeItemMetadata("权限标识格式错误")]
        D4004,

        /// <summary>
        /// 权限不存在
        /// </summary>
        [ErrorCodeItemMetadata("权限不存在")]
        D4005,

        /// <summary>
        /// 父级菜单不能为当前节点，请重新选择父级菜单
        /// </summary>
        [ErrorCodeItemMetadata("父级菜单不能为当前节点，请重新选择父级菜单")]
        D4006,

        /// <summary>
        /// 不能移动根节点
        /// </summary>
        [ErrorCodeItemMetadata("不能移动根节点")]
        D4007,

        /// <summary>
        /// 当前目录存在数据,不能修改
        /// </summary>
        [ErrorCodeItemMetadata("当前目录存在数据,不能修改类型")]
        D4008,

        #endregion

        #region 用户 5

        /// <summary>
        /// 账户已存在
        /// </summary>
        [ErrorCodeItemMetadata("账户已存在")]
        D5000,

        /// <summary>
        /// 账户创建失败
        /// </summary>
        [ErrorCodeItemMetadata("账户创建失败")]
        D5001,

        /// <summary>
        /// 用户不存在
        /// </summary>
        [ErrorCodeItemMetadata("用户不存在")]
        D5002,

        /// <summary>
        /// 用户关系移除失败
        /// </summary>
        [ErrorCodeItemMetadata("用户关系移除失败")]
        D5003,

        /// <summary>
        /// 账户更新失败
        /// </summary>
        [ErrorCodeItemMetadata("账户更新失败")]
        D5004,

        /// <summary>
        /// 更新账户状态失败
        /// </summary>
        [ErrorCodeItemMetadata("更新账户状态失败")]
        D5005,

        /// <summary>
        /// 密码不一致
        /// </summary>
        [ErrorCodeItemMetadata("密码不一致")]
        D5006,

        /// <summary>
        /// 旧密码错误，请重新输入
        /// </summary>
        [ErrorCodeItemMetadata("旧密码错误，请重新输入")]
        D5007,

        /// <summary>
        /// 修改密码失败
        /// </summary>
        [ErrorCodeItemMetadata("修改密码失败")]
        D5008,

        /// <summary>
        /// 修改密码失败
        /// </summary>
        [ErrorCodeItemMetadata("修改密码失败")]
        D5009,

        /// <summary>
        /// 修改系统主题失败
        /// </summary>
        [ErrorCodeItemMetadata("修改系统主题失败")]
        D5010,

        /// <summary>
        /// 修改系统语言失败
        /// </summary>
        [ErrorCodeItemMetadata("修改系统语言失败")]
        D5011,

        /// <summary>
        /// 修改个人头像失败
        /// </summary>
        [ErrorCodeItemMetadata("修改个人头像失败")]
        D5012,

        /// <summary>
        /// 上传失败，图片格式不允许上传
        /// </summary>
        [ErrorCodeItemMetadata("上传失败，图片格式不允许上传")]
        D5013,

        /// <summary>
        /// 设计门户失败
        /// </summary>
        [ErrorCodeItemMetadata("设计门户失败")]
        D5014,

        /// <summary>
        /// 验证码错误
        /// </summary>
        [ErrorCodeItemMetadata("验证码输入错误，请重新输入！")]
        D5015,

        /// <summary>
        /// 导入文件格式错误
        /// </summary>
        [ErrorCodeItemMetadata("导入文件格式错误")]
        D5018,

        /// <summary>
        /// 导入的文件无数据
        /// </summary>
        [ErrorCodeItemMetadata("导入的文件是空数据")]
        D5019,

        /// <summary>
        /// 修改切换 组 织失败
        /// </summary>
        [ErrorCodeItemMetadata("修改切换组织失败")]
        D5020,

        /// <summary>
        /// 修改切换 岗位 失败
        /// </summary>
        [ErrorCodeItemMetadata("修改切换岗位失败")]
        D5021,

        /// <summary>
        /// 修改切换 角色 失败
        /// </summary>
        [ErrorCodeItemMetadata("修改切换角色失败")]
        D5022,

        /// <summary>
        /// 该组织下无角色或角色权限为空，组织切换失败
        /// </summary>
        [ErrorCodeItemMetadata("该组织下无角色或角色权限为空，组织切换失败")]
        D5023,

        #endregion

        #region 岗位 6

        /// <summary>
        /// 岗位编码已存在
        /// </summary>
        [ErrorCodeItemMetadata("岗位编码已存在")]
        D6000,

        /// <summary>
        /// 新增岗位失败
        /// </summary>
        [ErrorCodeItemMetadata("新增岗位失败")]
        D6001,

        /// <summary>
        /// 删除岗位失败
        /// </summary>
        [ErrorCodeItemMetadata("删除岗位失败")]
        D6002,

        /// <summary>
        /// 更新岗位失败
        /// </summary>
        [ErrorCodeItemMetadata("更新岗位失败")]
        D6003,

        /// <summary>
        /// 更新岗位状态失败
        /// </summary>
        [ErrorCodeItemMetadata("更新岗位状态失败")]
        D6004,

        /// <summary>
        /// 岗位名称已存在
        /// </summary>
        [ErrorCodeItemMetadata("岗位名称已存在")]
        D6005,

        /// <summary>
        /// 该岗位不存在
        /// </summary>
        [ErrorCodeItemMetadata("该岗位不存在")]
        D6006,

        /// <summary>
        /// 该岗位下有用户
        /// </summary>
        [ErrorCodeItemMetadata("该岗位下有用户")]
        D6007,

        /// <summary>
        /// 该岗位下有用户,不允许变更组织
        /// </summary>
        [ErrorCodeItemMetadata("该岗位下有用户,不允许变更组织")]
        D6008,

        #endregion

        #region 消息 7

        /// <summary>
        /// 通知公告状态错误
        /// </summary>
        [ErrorCodeItemMetadata("通知公告状态错误")]
        D7000,

        /// <summary>
        /// 通知公告删除失败
        /// </summary>
        [ErrorCodeItemMetadata("通知公告删除失败")]
        D7001,

        /// <summary>
        /// 通知公告编辑失败
        /// </summary>
        [ErrorCodeItemMetadata("通知公告编辑失败，类型必须为草稿")]
        D7002,

        /// <summary>
        /// 通知公告发布失败
        /// </summary>
        [ErrorCodeItemMetadata("通知公告发布失败")]
        D7003,

        #endregion

        #region 文件 8

        /// <summary>
        /// 文件不存在
        /// </summary>
        [ErrorCodeItemMetadata("文件不存在")]
        D8000,

        /// <summary>
        /// 文件上传失败
        /// </summary>
        [ErrorCodeItemMetadata("文件上传失败")]
        D8001,

        /// <summary>
        /// 已存在同名文件
        /// </summary>
        [ErrorCodeItemMetadata("已存在同名文件")]
        D8002,

        /// <summary>
        /// 文件下载失败
        /// </summary>
        [ErrorCodeItemMetadata("文件下载失败")]
        D8003,

        /// <summary>
        /// 上传失败，文件格式不允许上传
        /// </summary>
        [ErrorCodeItemMetadata("上传失败，文件格式不允许上传")]
        D1800,

        /// <summary>
        /// 预览失败，文件数据错误
        /// </summary>
        [ErrorCodeItemMetadata("预览失败，文件数据错误")]
        D1801,

        /// <summary>
        /// 预览失败，文件类型不支持
        /// </summary>
        [ErrorCodeItemMetadata("预览失败，文件类型不支持")]
        D1802,

        #endregion

        #region 系统配置 9

        /// <summary>
        /// 已存在同名或同编码参数配置
        /// </summary>
        [ErrorCodeItemMetadata("已存在同名或同编码参数配置")]
        D9000,

        /// <summary>
        /// 禁止删除系统参数
        /// </summary>
        [ErrorCodeItemMetadata("禁止删除系统参数")]
        D9001,

        /// <summary>
        /// 此IP未在白名单中
        /// </summary>
        [ErrorCodeItemMetadata("此IP未在白名单中")]
        D9002,

        /// <summary>
        /// 测试连接失败
        /// </summary>
        [ErrorCodeItemMetadata("测试连接失败")]
        D9003,

        /// <summary>
        /// 检测数据不存在,同步数据失败
        /// </summary>
        [ErrorCodeItemMetadata("检测数据不存在,同步数据失败")]
        D9004,

        /// <summary>
        /// 同步数据保存失败
        /// </summary>
        [ErrorCodeItemMetadata("同步数据保存失败")]
        D9005,

        /// <summary>
        /// 同步数据修改失败
        /// </summary>
        [ErrorCodeItemMetadata("同步数据修改失败")]
        D9006,

        /// <summary>
        /// Token验证失败
        /// </summary>
        [ErrorCodeItemMetadata("Token验证失败")]
        D9007,

        /// <summary>
        /// Token过期时间超过限定
        /// </summary>
        [ErrorCodeItemMetadata("超时登出时间,不能超过8百万分钟")]
        D9008,

        /// <summary>
        /// 验证码限定范围:3 - 6位
        /// </summary>
        [ErrorCodeItemMetadata("验证码限定范围:3 - 6位")]
        D9009,
        #endregion

        #region 单据规则 10
        /// <summary>
        /// 单据规则已引用，无法删除
        /// </summary>
        [ErrorCodeItemMetadata("单据规则已引用，无法删除")]
        BR0001,
        #endregion

        #region 任务调度 11

        /// <summary>
        /// 已存在同名任务调度
        /// </summary>
        [ErrorCodeItemMetadata("已存在同名任务调度")]
        D1100,

        /// <summary>
        /// 任务调度不存在
        /// </summary>
        [ErrorCodeItemMetadata("任务调度不存在")]
        D1101,

        #endregion

        #region 在线开发 14

        /// <summary>
        /// 已存在相同功能
        /// </summary>
        [ErrorCodeItemMetadata("已存在相同功能")]
        D1400,

        /// <summary>
        /// 错误的模板设计,表关系不能多对多
        /// </summary>
        [ErrorCodeItemMetadata("错误的模板设计,表关系不能多对多")]
        D1401,

        /// <summary>
        /// 主表未设置主键字段
        /// </summary>
        [ErrorCodeItemMetadata("主表未设置主键字段")]
        D1402,

        /// <summary>
        /// 复制模板 数据长度超过字段设定长度
        /// </summary>
        [ErrorCodeItemMetadata("已到达该模板复制上限，请复制源模板")]
        D1403,

        /// <summary>
        /// 上传文件 控件 不填写文件大小
        /// </summary>
        [ErrorCodeItemMetadata("文件或者图片上传控件的 [文件大小] 必须输入")]
        D1404,
        #endregion

        #region 数据建模 15
        /// <summary>
        /// 删除表失败
        /// </summary>
        [ErrorCodeItemMetadata("删除表失败")]
        D1500,

        /// <summary>
        /// 新增表失败
        /// </summary>
        [ErrorCodeItemMetadata("新增表失败")]
        D1501,

        /// <summary>
        /// 修改表失败
        /// </summary>
        [ErrorCodeItemMetadata("修改表失败")]
        D1502,

        /// <summary>
        /// 表已存在
        /// </summary>
        [ErrorCodeItemMetadata("表已存在")]
        D1503,

        /// <summary>
        /// 系统自带表,无法删除
        /// </summary>
        [ErrorCodeItemMetadata("系统自带表,无法删除")]
        D1504,

        /// <summary>
        /// 数据库类型不支持
        /// </summary>
        [ErrorCodeItemMetadata("数据库类型不支持")]
        D1505,

        /// <summary>
        /// 目的表存在数据，无法同步
        /// </summary>
        [ErrorCodeItemMetadata("目的表存在数据，无法同步")]
        D1506,

        /// <summary>
        /// 数据库连接失败
        /// </summary>
        [ErrorCodeItemMetadata("数据库连接失败")]
        D1507,

        /// <summary>
        /// 表存在数据,禁止操作
        /// </summary>
        [ErrorCodeItemMetadata("表存在数据,禁止操作")]
        D1508,

        /// <summary>
        /// 主键不能为空
        /// </summary>
        [ErrorCodeItemMetadata("主键不能为空")]
        D1509,
        #endregion

        #region 角色 16

        /// <summary>
        /// 角色编码已存在
        /// </summary>
        [ErrorCodeItemMetadata("角色编码已存在")]
        D1600,

        /// <summary>
        /// 角色名称已存在
        /// </summary>
        [ErrorCodeItemMetadata("角色名称已存在")]
        D1601,

        /// <summary>
        /// 新建角色失败
        /// </summary>
        [ErrorCodeItemMetadata("新建角色失败")]
        D1602,

        /// <summary>
        /// 该角色下有数据权限
        /// </summary>
        [ErrorCodeItemMetadata("该角色下有数据权限")]
        D1603,

        /// <summary>
        /// 该角色下有模块按钮权限
        /// </summary>
        [ErrorCodeItemMetadata("该角色下有模块按钮权限")]
        D1604,

        /// <summary>
        /// 该角色下有模块列表权限
        /// </summary>
        [ErrorCodeItemMetadata("该角色下有模块列表权限")]
        D1605,

        /// <summary>
        /// 该角色下有模块列表权限
        /// </summary>
        [ErrorCodeItemMetadata("该角色下有模块菜单权限")]
        D1606,

        /// <summary>
        /// 该角色下有用户
        /// </summary>
        [ErrorCodeItemMetadata("该角色下有用户")]
        D1607,

        /// <summary>
        /// 该角色不存在
        /// </summary>
        [ErrorCodeItemMetadata("该角色不存在")]
        D1608,

        /// <summary>
        /// 角色删除失败
        /// </summary>
        [ErrorCodeItemMetadata("角色删除失败")]
        D1609,

        /// <summary>
        /// 角色更新失败
        /// </summary>
        [ErrorCodeItemMetadata("角色更新失败")]
        D1610,

        /// <summary>
        /// 该角色下有模块表单权限
        /// </summary>
        [ErrorCodeItemMetadata("该角色下有模块表单权限")]
        D1611,

        /// <summary>
        /// 不是超管账号无权限变更角色信息
        /// </summary>
        [ErrorCodeItemMetadata("不是超管账号无权限变更角色信息")]
        D1612,

        /// <summary>
        /// 该角色下有用户,不允许变更组织
        /// </summary>
        [ErrorCodeItemMetadata("该角色和组织下有用户,不允许变更组织")]
        D1613,

        /// <summary>
        /// 不是超管账号无法批量设置权限
        /// </summary>
        [ErrorCodeItemMetadata("不是超管账号无法批量设置权限")]
        D1614,

        /// <summary>
        /// 该全局角色下有用户,不允许变更成组织角色
        /// </summary>
        [ErrorCodeItemMetadata("该全局角色下有用户,不允许变更成组织角色")]
        D1615,

        #endregion

        #region 系统缓存 17
        /// <summary>
        /// 清除缓存失败
        /// </summary>
        [ErrorCodeItemMetadata("清除缓存失败")]
        D1700,
        #endregion

        #region 公共 18

        /// <summary>
        /// 演示环境禁止修改数据
        /// </summary>
        [ErrorCodeItemMetadata("演示环境禁止修改数据")]
        D1200,

        /// <summary>
        /// 已存在同名或同主机租户
        /// </summary>
        [ErrorCodeItemMetadata("已存在同名或同主机租户")]
        D1300,

        /// <summary>
        /// 已存在同名或同编码项目
        /// </summary>
        [ErrorCodeItemMetadata("已存在同名或同编码项目")]
        xg1000,

        /// <summary>
        /// 已存在相同证件号码人员
        /// </summary>
        [ErrorCodeItemMetadata("已存在相同证件号码人员")]
        xg1001,

        /// <summary>
        /// 检测数据不存在
        /// </summary>
        [ErrorCodeItemMetadata("检测数据不存在")]
        xg1002,

        /// <summary>
        /// 检测必要参数确实
        /// </summary>
        [ErrorCodeItemMetadata("检测必要参数确实")]
        xg1003,

        /// <summary>
        /// 该字段不存在
        /// </summary>
        [ErrorCodeItemMetadata("该字段不存在")]
        xg1004,

        /// <summary>
        /// 预览失败,Sql语句包含敏感词汇
        /// </summary>
        [ErrorCodeItemMetadata("预览失败,Sql语句包含敏感词汇")]
        xg1005,

        #endregion

        #region 门户 19

        /// <summary>
        /// 您没有此门户使用权限，请重新设置
        /// </summary>
        [ErrorCodeItemMetadata("您没有此门户使用权限，请重新设置")]
        D1900,

        /// <summary>
        /// 门户分类不能为空
        /// </summary>
        [ErrorCodeItemMetadata("门户分类不能为空")]
        D1901,

        /// <summary>
        /// 门户名称不能为空
        /// </summary>
        [ErrorCodeItemMetadata("门户名称不能为空")]
        D1902,

        /// <summary>
        /// 门户编码不能为空
        /// </summary>
        [ErrorCodeItemMetadata("门户编码不能为空")]
        D1903,

        #endregion

        #region 代码生成 21

        /// <summary>
        /// 只能生成有表模板
        /// </summary>
        [ErrorCodeItemMetadata("只能生成有表模板")]
        D2100,

        /// <summary>
        /// 子表名称不能重复
        /// </summary>
        [ErrorCodeItemMetadata("子表名称不能重复")]
        D2101,

        /// <summary>
        /// 待功能完善
        /// </summary>
        [ErrorCodeItemMetadata("待功能完善")]
        D2102,

        /// <summary>
        /// 预览失败，数据不存在
        /// </summary>
        [ErrorCodeItemMetadata("预览失败，数据不存在")]
        D2103,

        /// <summary>
        /// 表缺失主键
        /// </summary>
        [ErrorCodeItemMetadata("表缺失主键")]
        D2104,

        /// <summary>
        /// 表缺失流程Id
        /// </summary>
        [ErrorCodeItemMetadata("表缺失流程Id")]
        D2105,

        #endregion

        #region 大屏 22

        /// <summary>
        /// 模块键值已存在
        /// </summary>
        [ErrorCodeItemMetadata("模块键值已存在")]
        D2200,

        #endregion

        #region 分级管理 23

        /// <summary>
        /// 新增分级管理失败
        /// </summary>
        [ErrorCodeItemMetadata("新增分级管理失败")]
        D2300,

        #endregion

        #region 工作流 WF

        /// <summary>
        /// 禁止委托本人
        /// </summary>
        [ErrorCodeItemMetadata("禁止委托本人")]
        WF0001,

        /// <summary>
        /// 复制流程引擎失败
        /// </summary>
        [ErrorCodeItemMetadata("复制流程引擎失败")]
        WF0002,

        /// <summary>
        /// 子流程无法删除
        /// </summary>
        [ErrorCodeItemMetadata("子流程无法删除")]
        WF0003,

        /// <summary>
        /// 系统管理员无法操作流程任务
        /// </summary>
        [ErrorCodeItemMetadata("系统管理员无法操作流程任务")]
        WF0004,

        /// <summary>
        /// 流程任务操作失败
        /// </summary>
        [ErrorCodeItemMetadata("流程任务操作失败")]
        WF0005,

        /// <summary>
        /// 当前流程被处理，无法审批
        /// </summary>
        [ErrorCodeItemMetadata("当前流程被处理，无法审批")]
        WF0006,

        /// <summary>
        /// 转办失败
        /// </summary>
        [ErrorCodeItemMetadata("转办失败")]
        WF0007,

        /// <summary>
        /// 指派失败
        /// </summary>
        [ErrorCodeItemMetadata("指派失败")]
        WF0008,

        /// <summary>
        /// 未找到催办人
        /// </summary>
        [ErrorCodeItemMetadata("未找到催办人")]
        WF0009,

        /// <summary>
        /// 当前流程被打回，无法撤回流程
        /// </summary>
        [ErrorCodeItemMetadata("当前流程被打回，无法撤回流程")]
        WF0010,

        /// <summary>
        /// 当前流程被处理，无法撤回流程
        /// </summary>
        [ErrorCodeItemMetadata("当前流程被处理，无法撤回流程")]
        WF0011,

        /// <summary>
        /// 功能流程无法删除
        /// </summary>
        [ErrorCodeItemMetadata("功能流程无法删除")]
        WF0012,

        /// <summary>
        /// 获取审批人失败
        /// </summary>
        [ErrorCodeItemMetadata("获取审批人失败")]
        WF0013,

        /// <summary>
        /// 子流程无法指派
        /// </summary>
        [ErrorCodeItemMetadata("子流程无法指派")]
        WF0014,

        /// <summary>
        /// 子流程无法终止
        /// </summary>
        [ErrorCodeItemMetadata("子流程无法终止")]
        WF0015,

        /// <summary>
        /// 功能流程无法终止
        /// </summary>
        [ErrorCodeItemMetadata("功能流程无法终止")]
        WF0016,

        /// <summary>
        /// 当前流程包含子流程,无法终止
        /// </summary>
        [ErrorCodeItemMetadata("当前流程包含子流程,无法终止")]
        WF0017,

        /// <summary>
        /// 当前流程包含子流程,无法撤回
        /// </summary>
        [ErrorCodeItemMetadata("当前流程包含子流程,无法撤回")]
        WF0018,

        /// <summary>
        /// 驳回节点包含子流程,审批失败
        /// </summary>
        [ErrorCodeItemMetadata("驳回节点包含子流程,审批失败")]
        WF0019,

        /// <summary>
        /// 流程已委托该用户,请重新委托
        /// </summary>
        [ErrorCodeItemMetadata("流程已委托该用户,请重新委托")]
        WF0020,

        /// <summary>
        /// 新增流程数据失败
        /// </summary>
        [ErrorCodeItemMetadata("新增流程数据失败")]
        WF0021,

        /// <summary>
        /// 所选流程包含条件候选人
        /// </summary>
        [ErrorCodeItemMetadata("所选流程包含条件候选人")]
        WF0022,

        /// <summary>
        /// 表单必填项不能为空
        /// </summary>
        [ErrorCodeItemMetadata("表单必填项不能为空")]
        WF0023,

        /// <summary>
        /// 当前流程未完成,不能修改工作流引擎
        /// </summary>
        [ErrorCodeItemMetadata("当前流程未完成,不能修改工作流引擎")]
        WF0024,

        #endregion

        #region 扩展 Ex
        /// <summary>
        /// 防止恶意创建过多数据
        /// </summary>
        [ErrorCodeItemMetadata("防止恶意创建过多数据")]
        Ex0001,

        /// <summary>
        /// 操作失败,无法移动同一文件下
        /// </summary>
        [ErrorCodeItemMetadata("操作失败,无法移动同一文件下")]
        Ex0002,

        /// <summary>
        /// 邮箱账户认证错误
        /// </summary>
        [ErrorCodeItemMetadata("邮箱账户认证错误")]
        Ex0003,

        /// <summary>
        /// 未设置邮箱账户
        /// </summary>
        [ErrorCodeItemMetadata("未设置邮箱账户")]
        Ex0004,

        /// <summary>
        /// 操作失败,无法移动子文件
        /// </summary>
        [ErrorCodeItemMetadata("操作失败,父级文件夹无法移动到子级文件夹中")]
        Ex0005,

        /// <summary>
        /// 删除失败,该文件夹下存在数据
        /// </summary>
        [ErrorCodeItemMetadata("删除失败,该文件夹下存在数据")]
        Ex0006,
        #endregion

        #region 多租户 Zh
        /// <summary>
        /// 租户不存在
        /// </summary>
        [ErrorCodeItemMetadata("租户不存在")]
        Zh10000,

        /// <summary>
        /// 租户已过期
        /// </summary>
        [ErrorCodeItemMetadata("租户已过期")]
        Zh10001,

        /// <summary>
        /// 租户账户已存在
        /// </summary>
        [ErrorCodeItemMetadata("租户账户已存在")]
        Zh10002,
        #endregion

        #region 分组管理 24

        /// <summary>
        /// 新增分组失败
        /// </summary>
        [ErrorCodeItemMetadata("新增分组失败")]
        D2400,

        /// <summary>
        /// 分组编码已存在
        /// </summary>
        [ErrorCodeItemMetadata("分组编码已存在")]
        D2401,

        /// <summary>
        /// 分组名称已存在
        /// </summary>
        [ErrorCodeItemMetadata("分组名称已存在")]
        D2402,

        /// <summary>
        /// 分组删除失败
        /// </summary>
        [ErrorCodeItemMetadata("分组删除失败")]
        D2403,

        /// <summary>
        /// 分组更新失败
        /// </summary>
        [ErrorCodeItemMetadata("分组更新失败")]
        D2404,

        /// <summary>
        /// 该分组不存在
        /// </summary>
        [ErrorCodeItemMetadata("该分组不存在")]
        D2405,

        /// <summary>
        /// 该分组下有用户
        /// </summary>
        [ErrorCodeItemMetadata("该分组下有用户")]
        D2406,
        #endregion



        #endregion

        #region 公用

        /// <summary>
        /// 新增数据失败
        /// </summary>
        [ErrorCodeItemMetadata("新增数据失败")]
        COM1000,

        /// <summary>
        /// 修改数据失败
        /// </summary>
        [ErrorCodeItemMetadata("修改数据失败")]
        COM1001,

        /// <summary>
        /// 删除数据失败
        /// </summary>
        [ErrorCodeItemMetadata("删除数据失败")]
        COM1002,

        /// <summary>
        /// 修改状态失败
        /// </summary>
        [ErrorCodeItemMetadata("修改状态失败")]
        COM1003,

        /// <summary>
        /// 已存在同名或同编码数据
        /// </summary>
        [ErrorCodeItemMetadata("已存在同名或同编码数据")]
        COM1004,

        /// <summary>
        /// 检测数据不存在
        /// </summary>
        [ErrorCodeItemMetadata("检测数据不存在")]
        COM1005,

        /// <summary>
        /// 同步失败
        /// </summary>
        [ErrorCodeItemMetadata("同步失败")]
        COM1006,

        /// <summary>
        /// 数据不存在
        /// </summary>
        [ErrorCodeItemMetadata("数据不存在")]
        COM1007,

        /// <summary>
        /// 操作失败
        /// </summary>
        [ErrorCodeItemMetadata("操作失败")]
        COM1008,

        /// <summary>
        /// 已到达该模板复制上限，请复制源模板
        /// </summary>
        [ErrorCodeItemMetadata("已到达该模板复制上限，请复制源模板")]
        COM1009,

        #endregion
    }
}
