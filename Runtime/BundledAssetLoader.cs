using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EdenMeng.AssetManager
{
    public class BundledAssetLoader : IAssetLoader
    {
        private AssetBundleMapping _assetBundleMapping;
        private AssetBundleMapping AssetBundleMapping
        {
            get
            {
                if (!_initialized)
                    Initialize();
                return _assetBundleMapping;
            }
        }
        private bool _initialized = false;

        private Dictionary<object, LoadedAsset> _loadedAssets = new Dictionary<object, LoadedAsset>();

        private void Initialize()
        {
            _initialized = true;

            var infoBundle = AssetBundle.LoadFromFile(AssetConstPath.BundleInfoPath);
            var bundlesInfoCollection = infoBundle.LoadAsset<BundlesInfoCollection>(AssetConstPath.BundleInfoAssetName);
            infoBundle.Unload(false);
            _assetBundleMapping = bundlesInfoCollection.CreateAssetBundleMapping();
        }

        public T LoadAsset<T>(string path) where T : UnityEngine.Object
        {
            var bundleName = AssetBundleMapping.GetBundleName(path);
            if (string.IsNullOrEmpty(bundleName))
            {
                Debug.LogError($"[Asset] Asset <{path}> is not exists.");
                return null;
            }

            AssetBundle assetBundle = AssetBundleManager.LoadBundleAndDependencies(bundleName);
            if (assetBundle == null)
                return null;
            
            var asset = assetBundle.LoadAsset<T>(path);
            if (asset == null)
            {
                Debug.LogError($"[Asset] Asset <{path}> load failed.");
                return null;
            }

            if (!_loadedAssets.TryGetValue(asset, out var loadedAsset))
                _loadedAssets.Add(asset, new LoadedAsset(asset, path, bundleName));
            else
                loadedAsset.UseCount++;
            return asset;
        }

        public IEnumerator LoadAssetAsync<T>(string path, Action<T> onComplete) where T : UnityEngine.Object
        {
            var bundleName = AssetBundleMapping.GetBundleName(path);
            if (string.IsNullOrEmpty(bundleName))
            {
                Debug.LogError($"[Asset] Asset <{path}> is not exists.");
                onComplete?.Invoke(null);
                yield break;
            }

            AssetBundle assetBundle = null;
            yield return AssetBundleManager.LoadBundleAndDependenciesAsync(bundleName, (bundle) => assetBundle = bundle);

            if (assetBundle == null)
            {
                onComplete?.Invoke(null);
                yield break;
            }

            var request = assetBundle.LoadAssetAsync<T>(path);
            yield return request;
            var asset = request.asset as T;
            if (asset == null)
            {
                Debug.LogError($"[Asset] Asset <{path}> load failed (Async).");
                onComplete?.Invoke(null);
                yield break;
            }
            if (!_loadedAssets.TryGetValue(asset, out var loadedAsset))
                _loadedAssets.Add(asset, new LoadedAsset(asset, path, bundleName));
            else
                loadedAsset.UseCount++;
            onComplete?.Invoke(asset);
        }

        public void UnloadAsset<T>(T asset) where T : UnityEngine.Object
        {
            if (!_loadedAssets.TryGetValue(asset, out var loadedAsset))
            {
                Debug.LogWarningFormat("Trying to unload an asset not loaded before. Asset Name:{0}", asset.name);
                return;
            }
            loadedAsset.UseCount--;
            if (loadedAsset.UseCount <= 0)
                _loadedAssets.Remove(asset);
            
            // 这里是要将计数传递给bundle，否则会出现bundle泄露
            AssetBundleManager.UnloadBundleAndDependencies(loadedAsset.BundleName);
        }

        private class LoadedAsset
        {
            public object Asset { get; }
            public string AssetPath { get; }
            public string BundleName { get; }

            public int UseCount;

            public LoadedAsset(object asset, string assetPath, string bundleName)
            {
                Asset = asset;
                AssetPath = assetPath;
                BundleName = bundleName;
                UseCount = 1;
            }
        }
    }
}