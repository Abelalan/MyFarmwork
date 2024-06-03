using System;


public interface IAwakeSystem : ISystem
{
    Type ComponentType();
}

/// <summary>
/// AwakeœµÕ≥
/// </summary>
/// <typeparam name="C"></typeparam>
[ECSSystem]
public abstract class AwakeSystem<C> : IAwakeSystem where C : ECSComponent
{
    public abstract void Awake(C c);

    public Type ComponentType()
    {
        return typeof(C);
    }

    public Type SystemType()
    {
        return GetType();
    }
}
/// <summary>
/// ÷ÿ‘ÿ
/// </summary>
/// <typeparam name="C"></typeparam>
/// <typeparam name="P1"></typeparam>
[ECSSystem]
public abstract class AwakeSystem<C, P1> : IAwakeSystem where C : ECSComponent
{
    public abstract void Awake(C c, P1 p1);

    public Type ComponentType()
    {
        return typeof(C);
    }

    public Type SystemType()
    {
        return GetType();
    }
}


[ECSSystem]
public abstract class AwakeSystem<C, P1, P2> : IAwakeSystem where C : ECSComponent
{
    public abstract void Awake(C c, P1 p1, P2 p2);

    public Type ComponentType()
    {
        return typeof(C);
    }

    public Type SystemType()
    {
        return GetType();
    }
}

