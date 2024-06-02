/// <summary>
/// ������ECSComponent����ʾECS�е����
/// </summary>
public abstract class ECSComponent
{
    // ÿ���������һ��Ψһ��ID��ֻ��Get����������ʾ�����Ψһ��ʶ
    public long ID { get; private set; }

    // // �������ʵ��� ID���ɶ���д����ʾ������������ʵ��
    public long EntityID { get; set; }

    // ����Ƿ��ѱ��ͷŵı�־���ɶ���д����ʾ����Ƿ�����
    public bool Disposed { get; set; }

    // ͨ��EntityID ��ȡ������������ʵ��
    public ECSEntity Entity
    {
        get
        {
            // ���EntityID == 0������Ĭ��ֵnull;
            if (EntityID == 0)
                return default;

            // ͨ��ECSModule���ٲ��Ҳ����ض�Ӧ��ʵ��
            return TGameFramework.Instance.GetModule<ECSModule>().FindEntity(EntityID);
        }
    }
    /// <summary>
    /// ECSComponent �Ĺ��캯�������ڳ�ʼ�����
    /// </summary>
    public ECSComponent()
    {
        // ʹ��IDGenerator����һ���µ�ID
        ID = IDGenerator.NewInstanceID();
        // δ����
        Disposed = false;
    }
}

