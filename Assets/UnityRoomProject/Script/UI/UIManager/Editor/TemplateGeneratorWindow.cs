using UnityEditor;
using UnityEngine;

namespace GameJamProject.UI.Editor
{
    public class TemplateGeneratorWindow : EditorWindow
    {
        private string scriptName = "SampleScreen"; //スクリプト名
        private string rootPath = "Assets/GameJamProject/GameData/Prefab/UI/Templates/"; //プレハブの保存先
        private string scriptPath = "Assets/GameJamProject/Script/UI/Templates/"; //スクリプトの保存先
        private string namespaceName = "GameJamProject"; //名前空間
        private string description = "This is a description"; //説明
        private string viewName = "SampleView"; //ビュー名
        private string presenterBaseName = "BasePresenter"; //プレゼンターのベース名

        [MenuItem("HikanyanTools/Template Generator")]
        public static void ShowWindow()
        {
            GetWindow<TemplateGeneratorWindow>("Template Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Generate UI Template", EditorStyles.boldLabel);

            scriptName = EditorGUILayout.TextField("Screen Name", scriptName);
            namespaceName = EditorGUILayout.TextField("Namespace", namespaceName);
            description = EditorGUILayout.TextField("Description", description);
            viewName = EditorGUILayout.TextField("View Name", viewName);
            presenterBaseName = EditorGUILayout.TextField("Presenter Base Name", presenterBaseName);
            rootPath = EditorGUILayout.TextField("Root Path", rootPath);
            scriptPath = EditorGUILayout.TextField("Script Path", scriptPath);

            if (GUILayout.Button("Generate Template"))
            {
                TemplateGenerator.GenerateUITemplate(scriptName, scriptPath, rootPath, namespaceName, description,
                    viewName, presenterBaseName);
            }
        }
    }
}