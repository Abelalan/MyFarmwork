using System;

namespace UnityEngine
{
	public static class TransformExt
    {
        /// <summary>
        /// 设置父节点并且重置位置,旋转和缩放
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="parent"></param>
        public static void SetParentAndResetAll(this Transform transform, Transform parent)
        {
            transform.SetParent(parent);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// 查找子物体,根据子物体名称和组件类型
        /// </summary>
        /// <typeparam name="T">子物体上的某个组件类型</typeparam>
        /// <param name="transform"></param>
        /// <param name="name">子物体名称,*匹配任意名称</param>
        /// <param name="index">子物体序号</param>
        /// <returns></returns>
        public static T FindChild<T>(this Transform transform, string name, int index) where T : Component
        {
            return FindChild(transform, typeof(T), name, index) as T;
        }

        public static Component FindChild(this Transform transform, Type type, string name, int index)
        {
            int currentIndex = 0;
            // 获取指定类型 type 的所有子对象组件，包括未激活的（true 表示包括未激活的子对象）。
            Component[] components = transform.GetComponentsInChildren(type, true);
            // 遍历 components 数组中的每一个组件。
            for (int i = 0; i < components.Length; i++)
            {
                // 检查组件的名称是否与传入的 name 参数匹配，或者 name 参数为通配符 "*"，表示匹配所有名称。
                if (components[i].name == name || name == "*")
                {
                    // 如果当前组件的索引 currentIndex 与传入的 index 参数匹配，则返回当前组件。
                    // 同时，currentIndex + 1 ，表示找到了一个符合条件的组件。
                    // index与当前的索引不相同时，currentIndex依旧自增
                    if (index == currentIndex++)
                    {
                        return components[i];
                    }
                }
            }
            return null;
        }
    }
}