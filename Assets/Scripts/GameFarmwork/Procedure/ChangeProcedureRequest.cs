/// <summary>
/// 改变流程请求
/// </summary>
public class ChangeProcedureRequest
{
    // 目标流程
    public BaseProcedure TargetProcedure { get; set; }
    public object Value { get; set; }
}