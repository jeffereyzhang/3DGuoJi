using System;

/// <summary>
/// 消息ID
/// </summary>
public class MessageId
{
    public const int 租船订舱_缮制单据 = 195;
    public const int 租船订舱_缮制单据完成 = 196;
    public const int 租船订舱_订舱 = 197;
    public const int 租船订舱_订舱完成 = 198;
    public const int 租船订舱_装船 = 199;
    public const int 租船订舱_装船完成 = 200;
    public const int 租船订舱完成 = 103;


    public const int 投保_投保单 = 201;
    public const int 投保_投保单完成 = 202;
    public const int 投保_保险单 = 203;
    public const int 投保_保险单完成 = 204;
    public const int 投保完成 = 103;

    public const int 提空装箱_交接单 = 205;
    public const int 提空装箱_交接单完成 = 206;
    public const int 提空装箱_提取 = 207;
    public const int 提空装箱_提取完成 = 208;
    public const int 提空装箱完成 = 103;

    public const int 签发提单_提单 = 209;
    public const int 签发提单_提单完成 = 210;
    public const int 签发提单完成 = 103;

    public const int 交单结汇_汇票 = 211;
    public const int 交单结汇_汇票完成 = 212;
    public const int 交单结汇_收汇 = 213;
    public const int 交单结汇_收汇完成 = 214;
    public const int 交单结汇完成 = 103;

    public const int 核销退税_核销单 = 215;
    public const int 核销退税_核销单完成 = 216;
    public const int 核销退税_登记表 = 217;
    public const int 核销退税_登记表完成 = 218;
    public const int 核销退税完成 = 103;


    public const int 核销退税_打开知识点 = 180;
    public const int 核销退知识点关闭 = 181;
}

/// <summary>
/// 任务描述
/// </summary>
public class TaskDesc
{
    public const string 租船订舱_开始缮制单据 = "我们需要把发票/装箱单和国际货物运输托运单交给上海德威国际集装箱货运公司。送出前，我们先点击电脑进行查看准备好的单据。";
    public const string 租船订舱_缮制单据完成 = "发票/装箱单和国际货物运输托运单填写无误，将以e-mail的形式发送给上海德威国际集装箱货运公司,他们公司业务员将根据上述资料制作装货单。";
    public const string 租船订舱_开始订舱 = "公司收到发票/装箱单和托运单审核正确后开具装货单。点击电脑，查看装货单信息。";
    public const string 租船订舱_订舱完成 = "装货单信息完整，将托运单的配舱回单退回，并签发装货单给托运人。";
    public const string 租船订舱_开始装船 = "货物经海关查验放行装船后，由船长或大副签收收货单（也称场站（大副）收据），点击电脑查看收货单。";
    public const string 租船订舱_装船完成 = "接下来杭州婉丽进出口有限公司凭收货单向上海德威国际集装箱货运公司交付运费并换取正式提单。";

    public const string 投保_开始投保单 = "在完成托运手续确认船期后，公司已准备好了投保单和商业发票，在把投保单和商业发票提交给保险公司进行投保之前，请先点击电脑确认单据无误。";
    public const string 投保_投保单完成 = "将投保单和商业发票提交给保险公司进行投保。";
    public const string 投保_开始保险单 = "保险公司接受杭州婉丽进出口有限公司的投保申请后，收取保险费后，制单员根据投保单及任务提示缮制货物运输保险单。点击电脑查看保险单";
    public const string 投保_保险单完成 = "打印出的保险单经加盖公章后生效，现已移交给杭州婉丽进出口有限公司人员。";

    public const string 提空装箱_开始制作交接单 = "上海德威国际集装箱货运公司已制作好设备交接单，可点击电脑查看。";
    public const string 提空装箱_制作交接单完成 = "设备交接单已递交给车队司机，由其到上海港空箱堆场提取空箱。";
    public const string 提空装箱_开始展示交接单 = "司机给码头堆场值班员提供了设备交接单，接下来进行查看。";
    public const string 提空装箱_展示交接单完成 = "将检查无误后的集装箱装车，司机驾驶车辆离开堆场。";

    public const string 签发提单_开始签发提单 = "中远上海分公司缮制海运提单，交由出口商工作人员，点击电脑查看单据并进行确认。";
    public const string 签发提单_签发提单完成 = "单据确认无误后中远上海分公司作为承运人签发提单给杭州婉丽进出口有限公司。";

    public const string 交单结汇_开始汇票 = "杭州婉丽进出口有限公司根据信用证中关于汇票的条款和相关的信息缮制汇票，点击电脑查看。";
    public const string 交单结汇_汇票完成 = "汇票正确无误，单证员备齐相关单据后，到中国银行杭州支行办公大厅办理押汇。";
    public const string 交单结汇_开始押汇 = "杭州婉丽进出口有限公司的单证员到银行办理押汇，收到银行工作人员给出《出口收汇核销专用联》。点击电脑查看。";
    public const string 交单结汇_押汇完成 = "交单结汇办理完成！";

    public const string 核销退税_开始核销单 = "单证员已从外汇局拿到出口收汇核销单，根据相关资料，按照相关部门的相关要求填写完成出口收汇核销单。点击电脑查看。";
    public const string 核销退税_核销单完成 = "单证员备齐所需单证后，到国家外汇局办理核销业务。";
    public const string 核销退税_开始登记表 = "杭州婉丽进出口有限公司单证员前来办理核销，并已做登记，点击查看登记表。";
    public const string 核销退税_登记表完成 = "单证员收到出口退税专用联，之后带着出口收汇核销单、报关单及相关单据到国税局办理退税手续。";

    //核销退税新添加退税步骤
    public const string 核销退税_开始退税 = "杭州婉丽进出口有限公司单证员已做退税登记，然后后带着出口收汇核销单、报关单及相关单据到国税局办理退税手续。。";
    public const string 核销退税_退税完成 = "杭州婉丽进出口有限公司该票货物已完成核销退税。";
}