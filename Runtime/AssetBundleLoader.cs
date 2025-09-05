using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace EdenMeng.AssetManager
{
    public partial class AssetBundleLoader : IAssetBundleLoader
    {
        private AssetBundleDependencies _assetBundleDependencies;
        public AssetBundleDependencies AssetBundleDependencies
        {
            get
            {
                if (!_initialized)
                    Initialize();
                return _assetBundleDependencies;
            }
        }

        private bool _initialized = false;

        private Dictionary<string, BundleLoadingInfo> _bundlesLoadingInfo = new Dictionary<string, BundleLoadingInfo>();

        private void Initialize()
        {
            _initialized = true;

            var infoBundle = AssetBundle.LoadFromFile(AssetConstPath.BundleInfoPath);
            var bundlesInfoCollection = infoBundle.LoadAsset<BundlesInfoCollection>(AssetConstPath.BundleInfoAssetName);
            infoBundle.Unload(false);
            _assetBundleDependencies = bundlesInfoCollection.CreateAssetBundleDependencies();
        }

        public AssetBundle LoadBundleAndDependencies(string assetBundleName)
        {
            // NOTE 这里需要先加载目标bundle，构建引用信息；然后再加载依赖bundle
            AssetBundle assetBundle = LoadOrGetAssetBundle(assetBundleName);
            
            var allDependencies = AssetBundleDependencies.GetAllDependencies(assetBundleName);
            if (allDependencies != null && allDependencies.Count > 0)
            {
                foreach (var dependency in allDependencies)
                    LoadOrGetAssetBundle(dependency);
            }
            return assetBundle;
        }

        public IEnumerator LoadBundleAndDependenciesAsync(string assetBundleName, Action<AssetBundle> onComplete)
        {
            // NOTE 这里需要先加载目标bundle，构建引用信息；然后再加载依赖bundle
            yield return LoadAssetBundleAsync(assetBundleName);

            var allDependencies = AssetBundleDependencies.GetAllDependencies(assetBundleName);
            if (allDependencies != null && allDependencies.Count > 0)
            {
                foreach (var dependency in allDependencies)
                    yield return LoadAssetBundleAsync(dependency); // 这里依次加载所有的依赖，考虑是否可以同时加载所有的依赖？是否能更快？
            }

            if (_bundlesLoadingInfo.TryGetValue(assetBundleName, out var bundleInfo))
                onComplete?.Invoke(bundleInfo.Bundle);
            else
                onComplete?.Invoke(null);
        }

        public void UnloadBundleAndDependencies(string assetBundleName)
        {
            DecrementCountOrUnloadAssetBundle(assetBundleName);

            var allDependencies = AssetBundleDependencies.GetAllDependencies(assetBundleName);
            foreach (var bundleName in allDependencies)
                DecrementCountOrUnloadAssetBundle(bundleName);
        }

        private AssetBundle LoadOrGetAssetBundle(string bundleName)
        {
            BundleLoadingInfo loadingInfo = GetOrCreateBundleLoadingInfo(bundleName);
            if (loadingInfo == null)
            {
                Debug.LogError($"[Asset] Bundle <{bundleName}> is not exists.");
                return null;
            }
            return loadingInfo.Load();
        }

        private IEnumerator LoadAssetBundleAsync(string bundleName)
        {
            BundleLoadingInfo loadingInfo = GetOrCreateBundleLoadingInfo(bundleName);
            if (loadingInfo == null)
            {
                Debug.LogError($"[Asset] Bundle <{bundleName}> is not exists.");
                yield break;
            }
            yield return loadingInfo.LoadAsync(null);
        }

        private void DecrementCountOrUnloadAssetBundle(string bundleName)
        {
            if (!_bundlesLoadingInfo.TryGetValue(bundleName, out var loadingInfo))
            {
                Debug.LogWarningFormat("[Asset] Trying to unload a AssetBundle not loaded before. AssetBundle Name:{0}", bundleName);
                return;
            }

            if (loadingInfo.Unload())
                _bundlesLoadingInfo.Remove(bundleName);
        }

        private BundleLoadingInfo GetOrCreateBundleLoadingInfo(string bundleName)
        {
            BundleLoadingInfo loadingInfo;
            if (!_bundlesLoadingInfo.TryGetValue(bundleName, out loadingInfo))
            {
                var bundlePath = Path.Combine(AssetConstPath.BundlePath, bundleName);
                if (!File.Exists(bundlePath))
                    return null;
                loadingInfo = new BundleLoadingInfo(bundleName);
                _bundlesLoadingInfo.Add(bundleName, loadingInfo);
            }
            return loadingInfo;
        }

        private void LogCurrentAssetBundleStatus()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Bundles: ");
            foreach (var loadingInfo in _bundlesLoadingInfo)
                stringBuilder.AppendLine(loadingInfo.ToString());
            Debug.Log(stringBuilder.ToString());
        }
    }
}
