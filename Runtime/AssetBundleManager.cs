using System;
using System.Collections;
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
    }
}