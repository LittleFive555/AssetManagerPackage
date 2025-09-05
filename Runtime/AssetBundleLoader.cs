using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace EdenMeng.AssetManager
{
    public class AssetBundleLoader : IAssetBundleLoader
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
            yield return LoadAssetBundleAsync(assetBundleName, onComplete);
            
            var allDependencies = AssetBundleDependencies.GetAllDependencies(assetBundleName);
            if (allDependencies != null && allDependencies.Count > 0)
            {
                foreach (var dependency in allDependencies)
                    yield return LoadAssetBundleAsync(dependency, null); // 这里依次加载所有的依赖，考虑是否可以同时加载所有的依赖？是否能更快？
            }
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
            BundleLoadingInfo loadingInfo = GetBundleLoadingInfo(bundleName);
            if (loadingInfo == null)
            {
                Debug.LogError($"[Asset] Bundle <{bundleName}> is not exists.");
                return null;
            }
            return loadingInfo.Load();
        }

        private IEnumerator LoadAssetBundleAsync(string bundleName, Action<AssetBundle> onComplete)
        {
            BundleLoadingInfo loadingInfo = GetBundleLoadingInfo(bundleName);
            if (loadingInfo == null)
            {
                Debug.LogError($"[Asset] Bundle <{bundleName}> is not exists.");
                onComplete?.Invoke(null);
                yield break;
            }
            yield return loadingInfo.LoadAsync(onComplete);
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

        private BundleLoadingInfo GetBundleLoadingInfo(string bundleName)
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

        private class BundleLoadingInfo
        {
            private readonly string _bundleName;

            private AssetBundleCreateRequest _request = null;

            private AssetBundle _assetBundle;

            private int _useCount;

            /// <summary>
            /// Use count will set 1 in constructor.
            /// </summary>
            /// <param name="assetBundle"></param>
            public BundleLoadingInfo(string assetBundleName)
            {
                _bundleName = assetBundleName;
            }

            public AssetBundle Load()
            {
                _useCount++;
                int counter = _useCount;

                // 已经加载好
                if (_assetBundle != null)
                {
                    Debug.Log($"[Asset] Use loaded bundle <{_bundleName}>, Counter {counter}.");
                    return _assetBundle;
                }

                Debug.Log($"[Asset] Load bundle <{_bundleName}>.");
                if (_request != null)
                {
                    _assetBundle = _request.assetBundle;
                }
                else
                {
                    var bundlePath = Path.Combine(AssetConstPath.BundlePath, _bundleName);
                    _assetBundle = AssetBundle.LoadFromFile(bundlePath);
                }
                Debug.Log($"[Asset] Load bundle <{_bundleName}> finished, Counter {counter}.");
                
                return _assetBundle;
            }

            public IEnumerator LoadAsync(Action<AssetBundle> callback)
            {
                _useCount++;
                int counter = _useCount;

                // 已经加载好
                if (_assetBundle != null)
                {
                    Debug.Log($"[Asset] Use loaded bundle (Async) <{_bundleName}>, Counter {counter}.");
                    callback?.Invoke(_assetBundle);
                    yield break;
                }

                Debug.Log($"[Asset] Load bundle (Async) <{_bundleName}>.");
                if (_request == null)
                {
                    var bundlePath = Path.Combine(AssetConstPath.BundlePath, _bundleName);
                    _request = AssetBundle.LoadFromFileAsync(bundlePath);
                    yield return _request;
                    Debug.Log($"[Asset] Load bundle (Async) <{_bundleName}> finished, Counter {counter}.");
                }
                else
                {
                    yield return new WaitUntil(() => _request.isDone);
                    Debug.Log($"[Asset] Load bundle (Async) <{_bundleName}> wait finished, Counter {counter}.");
                }
                
                if (_request.assetBundle != null)
                    _assetBundle = _request.assetBundle;
                callback?.Invoke(_assetBundle);
            }

            /// <returns>Return true if use counter is less than zero and really unloaded bundle.</returns>
            public bool Unload()
            {
                _useCount--;
                int counter = _useCount;

                if (_useCount <= 0)
                {
                    _assetBundle = null;
                    // NOTE 如果此时还没加载完，调用_request.assetBundle会把加载改为同步
                    if (_request != null)
                    {
                        _request.assetBundle.Unload(true);
                        _request = null;
                    }
                    Debug.Log($"[Asset] Unload bundle <{_bundleName}> released, Counter {counter}.");
                    return true;
                }
                else
                {
                    Debug.Log($"[Asset] Unload bundle <{_bundleName}>, Counter {counter}.");
                    return false;
                }
            }

            public override string ToString()
            {
                return $"[Asset] Bundle<{_bundleName}> UseCount {_useCount}, isLoaded {_assetBundle != null}";
            }
        }
    }
}
