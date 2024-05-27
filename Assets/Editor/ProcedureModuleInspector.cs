using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TGame.Editor.Inspector
{
    // 将ProcedureModuleInspector这个自定义编辑器类与ProcedureModule关联起来
    // 在unity编辑器中选择该ProcedureModule组件时，将使用指定的ProcedureModuleInspector自定义编辑器来显示和编辑器属性
    [CustomEditor(typeof(ProcedureModule))]
    public class ProcedureModuleInspector : BaseInspector
    {
        // 获取ProcedureModule中的序列化属性
        // proceduresProperty流程属性
        private SerializedProperty proceduresProperty;
        // 默认流程属性
        private SerializedProperty defaultProcedureProperty;
        // 所有流程类型的名字
        private List<string> allProcedureTypes;
        /// <summary>
        /// 
        /// </summary>
        protected override void OnInspectorEnable()
        {
            base.OnInspectorEnable();
            // serializedObject 是 Editor 类中的一个属性，表示当前编辑器正在检查的对象
            // serializedObject 提供了一个接口，通过该接口可以以序列化的方式访问和修改目标对象的属性
            // 查找属性：使用 serializedObject.FindProperty 方法查找目标对象的特定属性，并返回一个 SerializedProperty 对象。
            // 通过 SerializedProperty 对象，可以读取和写入属性值，并在自定义 Inspector 中进行操作。

            // 找到proceduresNames的字符串数组
            proceduresProperty = serializedObject.FindProperty("proceduresNames");
            // 找到defaultProcedureName默认流程
            defaultProcedureProperty = serializedObject.FindProperty("defaultProcedureName");

            UpdateProcedures();
        }

        protected override void OnCompileComplete()
        {
            base.OnCompileComplete();
            UpdateProcedures();
        }

        private void UpdateProcedures()
        {
            // 获取BaseProcedure的所有子类型 false，不允许返回抽象类型，Utility.Types.GAME_CSHARP_ASSEMBLY指定搜索的程序集，  ConvertAll ，将每个 Type 对象转换为其全名字符串，并存储在 allProcedureTypes 列表中。
            allProcedureTypes = Utility.Types.GetAllSubclasses(typeof(BaseProcedure), false, Utility.Types.GAME_CSHARP_ASSEMBLY).ConvertAll((Type t) => { return t.FullName; });

            //移除不存在的procedure
            // 从 proceduresProperty 数组的末尾开始向前遍历，这是因为在删除数组元素时从后向前遍历可以避免索引问题。
            for (int i = proceduresProperty.arraySize - 1; i >= 0; i--)
            {
                // 获取当前数组元素的字符串值，即流程类型的名称
                string procedureTypeName = proceduresProperty.GetArrayElementAtIndex(i).stringValue;
                // 检查这个流程类型名称是否存在于 allProcedureTypes 列表中
                if (!allProcedureTypes.Contains(procedureTypeName))
                {
                    // 如果不存在，调用 proceduresProperty.DeleteArrayElementAtIndex(i) 删除该元素。
                    proceduresProperty.DeleteArrayElementAtIndex(i);
                }
            }
            // 调用 serializedObject.ApplyModifiedProperties() 将对 serializedObject 所做的所有更改应用到实际对象上。
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            // EditorGUI.BeginDisabledGroup(Application.isPlaying);：开始一个禁用组，如果 Application.isPlaying 为 true，则禁用该组内的所有控件。
            // 在游戏运行时，用户无法修改这些设置。
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            {
                // 检查 allProcedureTypes 列表是否有内容：只有当 allProcedureTypes 列表有内容时，才显示以下内容。
                if (allProcedureTypes.Count > 0)
                {
                    // 显示所有的可用的流程类型
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    {
                        // 遍历allProcedureTypes，为每个流程类型显示一个带有左侧切换按钮的 标签
                        for (int i = 0; i < allProcedureTypes.Count; i++)
                        {
                            // GUI.changed 是 Unity 的一个静态属性，用于检测用户界面（GUI）中的任何更改
                            // 通过将 GUI.changed 设置为 false，可以确保在接下来的 GUI 绘制过程中捕捉到的任何变化都是由用户的交互引起的，而不是之前的状态。
                            GUI.changed = false;
                            // FindProcedureTypeIndex 方法：检查当前流程类型是否已存在于 proceduresProperty 中
                            int? index = FindProcedureTypeIndex(allProcedureTypes[i]);
                            // 显示一个带有左侧切换按钮的标签，按钮的状态取决于当前流程类型是否已存在。
                            bool selected = EditorGUILayout.ToggleLeft(allProcedureTypes[i], index.HasValue);

                            // 处理切换状态的变化：如果切换状态发生变化（用户点击了按钮），则添加或移除流程类型。
                            if (GUI.changed)
                            {
                                // AddProcedure 和 RemoveProcedure 方法：根据用户的操作，添加或移除流程类型。
                                if (selected)
                                {
                                    // 点击切换标签属性，添加属性
                                    AddProcedure(allProcedureTypes[i]);
                                }
                                else
                                {
                                    // 移除属性
                                    RemoveProcedure(index.Value);
                                }
                            }
                        }
                    }
                    // 关闭垂直控件
                    EditorGUILayout.EndVertical();
                }
            }
            // 关闭禁用组
            EditorGUI.EndDisabledGroup();

            // 输出帮助信息
            if (proceduresProperty.arraySize == 0)
            {
                if (allProcedureTypes.Count == 0)
                {
                    EditorGUILayout.HelpBox("Can't find any procedure", UnityEditor.MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("Please select a procedure at least", UnityEditor.MessageType.Info);
                }
            }
            else
            {
                // 游戏正在运行，显示当前流程名字
                if (Application.isPlaying)
                {
                    //播放中显示当前状态
                    EditorGUILayout.LabelField("Current Procedure", TGameFramework.Instance.GetModule<ProcedureModule>().CurrentProcedure?.GetType().FullName);
                }
                else
                {
                    // 显示默认状态
                    List<string> selectedProcedures = new List<string>();
                    for (int i = 0; i < proceduresProperty.arraySize; i++)
                    {
                        selectedProcedures.Add(proceduresProperty.GetArrayElementAtIndex(i).stringValue);
                    }
                    // 排序
                    selectedProcedures.Sort();
                    // 获取默认状态的下标
                    int defaultProcedureIndex = selectedProcedures.IndexOf(defaultProcedureProperty.stringValue);
                    // 显示默认流程类型的下拉菜单：允许用户选择默认流程类型，
                    defaultProcedureIndex = EditorGUILayout.Popup("Default Procedure", defaultProcedureIndex, selectedProcedures.ToArray());
                    // 更新 defaultProcedureProperty 的值。
                    if (defaultProcedureIndex >= 0)
                    {
                        defaultProcedureProperty.stringValue = selectedProcedures[defaultProcedureIndex];
                    }
                }
            }
            // 应用所有更改：调用 serializedObject.ApplyModifiedProperties() 方法，将所有修改应用到实际对象上。
            serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// 添加进一个流程
        /// </summary>
        /// <param name="procedureType"></param>
        private void AddProcedure(string procedureType)
        {
            // 在数组的指定索引处添加一个新的元素
            proceduresProperty.InsertArrayElementAtIndex(0);
            // 返回数组中指定索引处的元素，设置数组中第一个元素的字符串值为 procedureType。
            // 这样就将指定的流程类型添加到了数组的开头。
            proceduresProperty.GetArrayElementAtIndex(0).stringValue = procedureType;
        }

        private void RemoveProcedure(int index)
        {
            // 获取指定索引处的流程类型的字符串值。
            string procedureType = proceduresProperty.GetArrayElementAtIndex(index).stringValue;
            // 检查要删除的流程类型是否是默认流程类型。如果是，则发出警告并返回，不执行删除操作。
            if (procedureType == defaultProcedureProperty.stringValue)
            {
                Debug.LogWarning("Can't remove default procedure");
                return;
            }
            // 删除数组中指定索引处的元素，即移除指定索引处的流程类型。
            proceduresProperty.DeleteArrayElementAtIndex(index);
        }
        /// <summary>
        /// 在流程属性数组中查找指定流程类型的索引。
        /// </summary>
        /// <param name="procedureType"></param>
        /// <returns></returns>
        private int? FindProcedureTypeIndex(string procedureType)
        {
            // 遍历流程属性数组，检查每个元素的字符串值是否等于指定的流程类型。
            for (int i = 0; i < proceduresProperty.arraySize; i++)
            {
                SerializedProperty p = proceduresProperty.GetArrayElementAtIndex(i);
                if (p.stringValue == procedureType)
                {
                    // 如果找到匹配的流程类型，返回其索引。
                    return i;
                }
            }
            // 如果未找到匹配的流程类型，则返回 null，表示未找到匹配项。
            return null;
        }
    }
}