using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace EdenMeng.AssetManager
{
    public partial class AssetBundleLoader
    {
        private class BundleLoadingInfo
        {
            private readonly string _bundleName;

            private AssetBundleCreateRequest _request = null;

            private AssetBundle _assetBundle;
            public AssetBundle Bundle => _assetBundle;

            private int _useCount;

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
