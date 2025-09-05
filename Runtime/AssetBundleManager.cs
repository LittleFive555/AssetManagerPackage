using System;
using System.Collections;
using System.Text;
using UnityEngine;

namespace EdenMeng.AssetManager
{
    public interface IAssetBundleLoader
    {
        AssetBundle LoadBundleAndDependencies(string assetBundleName);
        IEnumerator LoadBundleAndDependenciesAsync(string assetBundleName, Action<AssetBundle> onComplete);
        void UnloadBundleAndDependencies(string assetBundleName);
    }

    public class AssetBundleManager
    {
        private static IAssetBundleLoader _assetBundleLoader;
        public static IAssetBundleLoader AssetBundleLoader
        {
            get
            {
                if (_assetBundleLoader == null)
                    _assetBundleLoader = new AssetBundleLoader();
                return _assetBundleLoader;
            }
        }

        public static AssetBundle LoadBundleAndDependencies(string assetBundleName)
        {
            return AssetBundleLoader.LoadBundleAndDependencies(assetBundleName);
        }

        public static IEnumerator LoadBundleAndDependenciesAsync(string assetBundleName, Action<AssetBundle> onComplete)
        {
            yield return AssetBundleLoader.LoadBundleAndDependenciesAsync(assetBundleName, onComplete);
        }

        public static void UnloadBundleAndDependencies(string assetBundleName)
        {
            AssetBundleLoader.UnloadBundleAndDependencies(assetBundleName);
        }

        public static void LogAllLoadedBundle()
        {
            StringBuilder stringBuilder = new StringBuilder();
            int count = 0;
            foreach (var assetBundle in AssetBundle.GetAllLoadedAssetBundles())
            {
                stringBuilder.AppendLine(assetBundle.name);
                count++;
            }
            stringBuilder.AppendLine($"LoadedCount: {count}");
            Debug.Log(stringBuilder.ToString());
        }
    }
}