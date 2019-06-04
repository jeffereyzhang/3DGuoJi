/// <summary>
/// 用于事件系统的枚举
/// </summary>
public enum EventEnum
{
    相机移动,
    进入车内,
    退出车内,
    UI特效,
    区域信息,
    开始堆垛,
    结束堆垛
}
/// <summary>
/// 相机看向的目标点
/// </summary>
public enum RtsCameraTarget
{
    集装箱拖车,
    空箱堆高车,
    龙门吊,
    正面吊,
    重箱堆高车,
    跨运车
}
/// <summary>
/// 集装箱类型
/// </summary>
public enum UIEffectID
{
    NULL,
    集装箱拖车,
    空箱堆高车,
    龙门吊,
    正面吊,
    重箱堆高车,
    提示界面,
	操作提示界面,
    任务按钮,
    跨运车
}
/// <summary>
/// 堆垛的集装箱类型
/// </summary>
public enum DuiDuoContanerType
{
    Null,
    Cosco40,
    中国铁路40,
    中铁联集20
}

public enum PoolObjectID
{
    ServerPrefab,
    Bullet
}
