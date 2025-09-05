using System;
using System.Collections;

namespace EdenMeng.AssetManager
{
    public interface IAssetLoader
    {
        T LoadAsset<T>(string path) where T : UnityEngine.Object;

        IEnumerator LoadAssetAsync<T>(string path, Action<T> onComplete) where T : UnityEngine.Object;

        void UnloadAsset<T>(T asset) where T : UnityEngine.Object;
    }

    public class AssetManager
    {
        private static IAssetLoader _assetLoader;

#if UNITY_EDITOR
        public static void InitWithDatabase()
        {
            if (_assetLoader != null)
            {
                Debug.LogError("[Asset] Repeated initializing AssetManager.");
                return;
            }
            _assetLoader = new DatabaseAssetLoader();
        }
#endif

        public static void InitWithAssetBundle(IAssetBundlePath rootPath)
        {
            if (_assetLoader != null)
            {
                Debug.LogError("[Asset] Repeated initializing AssetManager.");
                return;
            }
            AssetConstPath.Initialize(rootPath);
            _assetLoader = new BundledAssetLoader();
        }

        public static void InitWithAssetBundle()
        {
            if (_assetLoader != null)
            {
                Debug.LogError("[Asset] Repeated initializing AssetManager.");
                return;
            }
            AssetConstPath.Initialize(new DefaultAssetBundlePath());
            _assetLoader = new BundledAssetLoader();
        }

        public static T LoadAsset<T>(string path) where T : UnityEngine.Object
        {
            if (_assetLoader == null)
                throw new NullReferenceException("AssetManager is not initialized. Please call AssetManager.InitWithDatabase() or AssetManager.InitWithAssetBundle().");
            return _assetLoader.LoadAsset<T>(path);
        }

        public static IEnumerator LoadAssetAsync<T>(string path, Action<T> onComplete) where T : UnityEngine.Object
        {
            if (_assetLoader == null)
                throw new NullReferenceException("AssetManager is not initialized. Please call AssetManager.InitWithDatabase() or AssetManager.InitWithAssetBundle().");
            return _assetLoader.LoadAssetAsync<T>(path, onComplete);
        }

        public static void UnloadAsset<T>(T obj) where T : UnityEngine.Object
        {
            if (_assetLoader == null)
                throw new NullReferenceException("AssetManager is not initialized. Please call AssetManager.InitWithDatabase() or AssetManager.InitWithAssetBundle().");
            _assetLoader.UnloadAsset(obj);
        }
    }
}