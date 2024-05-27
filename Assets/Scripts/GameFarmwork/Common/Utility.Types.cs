using System;
using System.Collections.Generic;
using System.Reflection;


/// <summary>
/// 
/// </summary>
public static partial class Utility
{
    public static class Types
    {
        // 预加载两个程序集
        // C#脚本的程序集
        public readonly static Assembly GAME_CSHARP_ASSEMBLY = Assembly.Load("Assembly-CSharp");
        // unity编辑器的程序集
        public readonly static Assembly GAME_EDITOR_ASSEMBLY = Assembly.Load("Assembly-CSharp-Editor");

        /// <summary>
        /// 获取所有能从某个类型分配的属性列表
        /// basePropertyType，基类型，用于检查属性类型是否可以分配给该类型
        /// objType 需要检查的对象类型
        /// bindingFlags 绑定标志默认包含实例、静态、非公共和公共属性
        /// </summary>
        public static List<PropertyInfo> GetAllAssignablePropertiesFromType(Type basePropertyType, Type objType, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
        {
            // 初始化属性信息的集合
            List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
            // 从目标对象类型中获取包含实例、静态、非公共和公共属性，按照指定的绑定标志获取
            PropertyInfo[] properties = objType.GetProperties(bindingFlags);
            // 遍历数组
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];
                // Type.IsAssignableFrom 方法是 .NET 提供的一个反射方法，用于确定一个类型的实例是否可以分配给另一个类型的实例
                // 它的作用类似于 C# 中的 is 关键字，但 IsAssignableFrom 是类型层级检查，而 is 是实例层级检查。
                if (basePropertyType.IsAssignableFrom(propertyInfo.PropertyType))
                {
                    // 如果可以分配，将propertyInfo添加进propertyInfos集合中
                    propertyInfos.Add(propertyInfo);    
                }
            }
            // 返回一个集合
            return propertyInfos;
        }

        /// <summary>
        /// 获取某个类型的所有子类型
        /// </summary>
        /// <param name="baseClass">父类</param>
        /// <param name="assemblies">程序集,如果为null则查找当前程序集</param>
        /// <returns></returns>
        public static List<Type> GetAllSubclasses(Type baseClass, bool allowAbstractClass, params Assembly[] assemblies)
        {
            // 初始化子集类型列表 
            List<Type> subclasses = new List<Type>();
            // 要查找的程序集，如果为null，则查找调用该方法的程序集。
            if (assemblies == null)
            {
                // 获取当前程序集
                assemblies = new Assembly[] { Assembly.GetCallingAssembly() };
            }
            // 遍历程序集与类型
            foreach (var assembly in assemblies)
            {
                // 遍历每个程序集中的每个类型。
                foreach (var type in assembly.GetTypes())
                {
                    // 检查类型是否可以分配给baseClass，使用baseClass.IsAssignableFrom(type)进行判断。
                    if (!baseClass.IsAssignableFrom(type))
                        continue;

                    // 如果不允许抽象类且类型是抽象的，则跳过该类型
                    if (!allowAbstractClass && type.IsAbstract)
                        continue;
                    // 将符合条件的类型添加到结果列表中
                    subclasses.Add(type);
                }
            }
            // 返会结果列表
            return subclasses;
        }
    }
}
