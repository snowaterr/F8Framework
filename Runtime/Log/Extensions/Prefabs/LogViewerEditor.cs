using log4net.Util;
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class LogViewerEditor : UnityEditor.Editor
    {
        private LogViewerEditor() { }

        [MenuItem("开发工具/添加LogViewer至当前场景", false, 103)]
        public static void InstantiatePrefab()
        {
            // 获取当前脚本文件的路径
            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(ScriptableObject.CreateInstance<LogViewerEditor>()));
            var prefabPath = scriptPath.Replace("/LogViewerEditor.cs", "/LogViewer.prefab");
            // 加载预制件
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            // 检查预制件是否成功加载
            if (prefab != null)
            {
                // 实例化预制件
                var instance = GameObject.Instantiate(prefab);
                instance.transform.GetChild(0).gameObject.SetActive(false);
                instance.name = "LogViewer--F8Framework";
                LogF8.Log("成功添加LogViewer");
            }
            else
            {
                LogF8.LogError("未能加载LogViewer");
            }
        }
    }
}
