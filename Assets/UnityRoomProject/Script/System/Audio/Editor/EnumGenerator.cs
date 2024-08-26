using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityRoomProject.Audio;

namespace GameJamProject.Audio.Editor
{
    public class EnumGenerator : UnityEditor.Editor
    {
        [MenuItem("HikanyanTools/Generate SoundType Enums")]
        public static void GenerateEnums()
        {
            // ScriptableObjectを取得する
            string[] guids = AssetDatabase.FindAssets("t:AudioClipRegistrar",
                new[] { "Assets/UnityRoomProject/GameData/ScriptableObject/Audio/" });
            if (guids.Length == 0)
            {
                Debug.LogError("AudioClipRegistrarが見つかりませんでした。");
                return;
            }

            // ScriptableObjectをロード
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            var audioClipRegistrar = AssetDatabase.LoadAssetAtPath<AudioClipRegistrar>(path);

            if (audioClipRegistrar == null)
            {
                Debug.LogError("AudioClipRegistrarが見つかりませんでした。");
                return;
            }

            // ディレクトリパス
            string directoryPath = "Assets/UnityRoomProject/Script/System/Audio/";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // BGM Enumを生成
            GenerateEnumFile("BGMType", audioClipRegistrar.BGMClips, directoryPath);

            // SE Enumを生成
            GenerateEnumFile("SEType", audioClipRegistrar.SEClips, directoryPath);

            // Voice Enumを生成
            GenerateEnumFile("VoiceType", audioClipRegistrar.VoiceClips, directoryPath);

            AssetDatabase.Refresh();
            Debug.Log("Enumファイルが生成されました。");
        }

        private static void GenerateEnumFile(string enumName, List<AudioClip> clips, string directoryPath)
        {
            string enumFilePath = Path.Combine(directoryPath, enumName + ".cs");

            using (StreamWriter writer = new StreamWriter(enumFilePath))
            {
                string enumContent = GenerateEnumScript(enumName, clips.ToArray());
                writer.Write(enumContent);
            }

            Debug.Log($"{enumName} Enumファイルが生成されました: {enumFilePath}");
        }

        private static string GenerateEnumScript(string enumName, AudioClip[] clips)
        {
            string entries = string.Join(",\n        ", FormatEnumEntries(clips)); // エントリのインデントを調整

            return $@"
namespace UnityRoomProject.Audio
{{
    public enum {enumName}
    {{
        {entries}
    }}
}}";
        }


        private static string[] FormatEnumEntries(AudioClip[] clips)
        {
            string[] entries = new string[clips.Length];
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i] != null)
                {
                    entries[i] = FormatEnumEntry(clips[i].name);
                }
            }

            return entries;
        }

        private static string FormatEnumEntry(string clipName)
        {
            // 無効な文字をアンダースコアに置き換え、Enum名として有効な形式に変換
            clipName = clipName.Replace(" ", "_").Replace("-", "_");
            return char.ToUpper(clipName[0]) + clipName.Substring(1);
        }
    }
}