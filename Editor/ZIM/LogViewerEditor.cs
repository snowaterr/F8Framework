using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class LogViewerEditor : UnityEditor.Editor
    {
        private LogViewerEditor() { }

        [MenuItem("开发工具/创建LogViewer", false, 103)]
        public static void InstantiatePrefab()
        {
            // 获取当前脚本文件的路径
            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(ScriptableObject.CreateInstance<LogViewerEditor>()));
            var prefabPath = scriptPath.Replace("Editor/ZIM/LogViewerEditor.cs", "Runtime/Log/Extensions/Prefabs/LogViewer.prefab");
            // 加载预制件
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            // 检查预制件是否成功加载
            if (prefab != null)
            {
                // 实例化预制件
                var instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                //var instance = GameObject.Instantiate(prefab);
                instance.transform.SetAsLastSibling();
                instance.transform.GetChild(0).gameObject.SetActive(false);
                LogF8.LogUtil("成功添加 LogViewer 到当前场景");
            }
            else
            {
                LogF8.LogError("未能加载LogViewer");
            }
        }
    }
}
