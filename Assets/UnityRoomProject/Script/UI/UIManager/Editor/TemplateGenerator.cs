using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameJamProject.UI.Editor
{
    public static class TemplateGenerator
    {
        public static void GenerateUITemplate(string scriptName, string scriptPath, string rootPath,
            string namespaceName, string description, string viewName, string presenterBaseName)
        {
            string fullScriptPath = scriptPath + scriptName + ".cs";
            string prefabPath = rootPath;

            // テンプレートを読み込み、プレースホルダを置換
            string templateContent = LoadTemplate();
            string scriptContent = templateContent
                .Replace("#NAMESPACE_NAME#", namespaceName)
                .Replace("#DESCRIPTION#", description)
                .Replace("#VIEW_NAME#", viewName)
                .Replace("#PRESENTER_NAME#", scriptName)
                .Replace("#PRESENTER_BASE_NAME#", presenterBaseName);

            // スクリプトの生成
            if (!Directory.Exists(scriptPath))
            {
                Directory.CreateDirectory(scriptPath);
            }

            File.WriteAllText(fullScriptPath, scriptContent);

            // プレハブの生成（必要に応じて）
            GameObject newPrefab = new GameObject(scriptName);
            newPrefab.AddComponent<Node>();
            if (!Directory.Exists(prefabPath))
            {
                Directory.CreateDirectory(prefabPath);
            }

            PrefabUtility.SaveAsPrefabAsset(newPrefab, prefabPath + scriptName + ".prefab");
            Object.DestroyImmediate(newPrefab);

            AssetDatabase.Refresh();

            Debug.Log($"UI Template '{scriptName}' generated successfully at {prefabPath} and {fullScriptPath}");
        }

        private static string LoadTemplate()
        {
            // テンプレート文字列（実際にはファイルから読み込むことも可能）
            return @"
namespace #NAMESPACE_NAME#
{
    /// <summary>
    /// #DESCRIPTION#のScreenのPresenter
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(#VIEW_NAME#))]
    public class #PRESENTER_NAME# : #PRESENTER_BASE_NAME#<#VIEW_NAME#, TransitionParam>
    {
        /// <summary>
        /// 初期化
        /// </summary>
        protected override async UniTask InitializeAsync(TransitionParam param, CancellationToken ct)
        {
            // 初期化コード
        }
    }
}";
        }
    }
}