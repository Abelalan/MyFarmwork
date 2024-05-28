using UnityEditor;

namespace TGame.Editor.Inspector
{
    /// <summary>
    /// 用于创建自定义Inspector界面的基类
    /// </summary>
	public class BaseInspector : UnityEditor.Editor
    {
        // DrawBaseGUI是一个虚拟的只读属性，返回值为true，表示默认情况下在Inspector中绘制基础GUI。
        protected virtual bool DrawBaseGUI { get { return true; } }
        // 用于跟踪是否在编译过程中
        private bool isCompiling = false;
        /// <summary>
        /// 子类可以重写来实现自定义的更新逻辑
        /// </summary>
        protected virtual void OnInspectorUpdateInEditor() { }

        private void OnEnable()
        {
            OnInspectorEnable();
            EditorApplication.update += UpdateEditor;
        }
        protected virtual void OnInspectorEnable() { }

        private void OnDisable()
        {
            EditorApplication.update -= UpdateEditor;
            OnInspectorDisable();
        }
        protected virtual void OnInspectorDisable() { }

        private void UpdateEditor()
        {
            // 如果之前没有在编译 (isCompiling 为 false)，但现在开始编译了 
            // 将 isCompiling 设为 true 并调用 OnCompileStart() 方法。
            if (!isCompiling && EditorApplication.isCompiling)
            {
                isCompiling = true;
                OnCompileStart();
            }
            // 如果之前在编译 (isCompiling 为 true)，但现在编译结束了 
            // 将 isCompiling 设为 false 并调用 OnCompileComplete() 方法。
            else if (isCompiling && !EditorApplication.isCompiling)
            {
                isCompiling = false;
                OnCompileComplete();
            }
            // 无论编译状态是否改变，OnInspectorUpdateInEditor() 方法都会在每次编辑器更新时调用，用于执行任何必要的自定义更新逻辑。
            OnInspectorUpdateInEditor();
        }

        public override void OnInspectorGUI()
        {
            if (DrawBaseGUI)
            {
                base.OnInspectorGUI();
            }
        }

        protected virtual void OnCompileStart() { }
        protected virtual void OnCompileComplete() { }
    }
}