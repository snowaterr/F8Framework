using System.Drawing;
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    [CustomEditor(typeof(ComponentBind), true)]  // 添加 true 参数以启用绘制基类的属性
    public class ComponentBindEditor : UnityEditor.Editor
    {
        private static GUIStyle _buttonStyle;
        public static GUIStyle ButtonWordWrap
        {
            get
            {
                if (_buttonStyle == null)
                {
                    int line = 2;   // 默认显示2行
                    _buttonStyle = new GUIStyle(GUI.skin.button);
                    _buttonStyle.wordWrap = true; // 启用自动换行
                    _buttonStyle.fontSize = 12;
                    _buttonStyle.fixedHeight = line * _buttonStyle.fontSize * 1.5f;
                }
                return _buttonStyle;
            }
        }

        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            
            foreach (var o in targets)
            {
                var targetObject = (ComponentBind)o;
                if (GUILayout.Button("组件绑定，GameObject名称须包含组件名（可能需要点击两次）", ButtonWordWrap)) 
                {
                    (targetObject).Bind();
                }
            }
        }
    }
}