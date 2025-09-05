using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EdenMeng.AssetManager.Editor
{
    public class AssetBundleBuildWindow : EditorWindow
    {
        private const string BuildTargetIndexKey = "ABBuild_BuildTargetIndex";
        private int BuildTargetIndex
        {
            get => EditorPrefs.GetInt(BuildTargetIndexKey, 0);
            set => EditorPrefs.SetInt(BuildTargetIndexKey, value);
        }

        private const string OutputPathKey = "ABBuild_OutputPath";
        private string OutputPath 
        {
            get => EditorPrefs.GetString(OutputPathKey, Application.streamingAssetsPath);
            set => EditorPrefs.SetString(OutputPathKey, value);
        }

        private const string BundleInfoPathKey = "ABBuild_BundleInfoPath";
        private string BundleInfoPath
        {
            get => EditorPrefs.GetString(BundleInfoPathKey, "Assets");
            set => EditorPrefs.SetString(BundleInfoPathKey, value);
        }

        private string[] m_BuildTargetsStrList;


        [MenuItem("Window/AssetBundle Builder")]
        public static void ShowWindow()
        {
            GetWindow<AssetBundleBuildWindow>("AssetBundle Builder");
        }

        private void OnGUI()
        {
            // BuildTarget
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Build Target", GUILayout.Width(100));

                // 将BuildTarget作为下拉选项进行选择
                if (m_BuildTargetsStrList == null)
                {
                    BuildTarget[] buildTargets = (BuildTarget[])Enum.GetValues(typeof(BuildTarget));
                    m_BuildTargetsStrList = buildTargets.Select(buildTarget => buildTarget.ToString()).ToArray();
                }
                BuildTargetIndex = EditorGUILayout.Popup(BuildTargetIndex, m_BuildTargetsStrList);
            }
            GUILayout.EndHorizontal();

            // Output Path
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Output Path", GUILayout.Width(100));
                OutputPath = EditorGUILayout.TextField(OutputPath);
            }
            GUILayout.EndHorizontal();

            // Bundle Info Path
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Bundle Info Path", GUILayout.Width(100));
                BundleInfoPath = EditorGUILayout.TextField(BundleInfoPath);
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Build AssetBundle"))
            {
                var assetBundleBuildInfo = new AssetBundleBuildInfo()
                {
                    BuildTarget = (BuildTarget)Enum.Parse(typeof(BuildTarget), m_BuildTargetsStrList[BuildTargetIndex]),
                    OutputPath = OutputPath,
                    BundleInfoPath = BundleInfoPath,
                };
                CreateAssetBundles.BuildAllAssetBundles(assetBundleBuildInfo);
            }
        }
    }

    public class AssetBundleBuildInfo
    {
        public BuildTarget BuildTarget;
        public string OutputPath;
        public string BundleInfoPath;
    }
}