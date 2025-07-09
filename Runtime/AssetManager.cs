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
        public static bool UseAB = false;

        private static IAssetLoader _assetLoader;

        public static void Initialize()
        {
#if UNITY_EDITOR
            if (UseAB)
                _assetLoader = new BundledAssetLoader();
            else
                _assetLoader = new DatabaseAssetLoader();
#else
            _assetLoader = new BundledAssetLoader();
#endif
        }

        public static T LoadAsset<T>(string path) where T : UnityEngine.Object
        {
            if (_assetLoader == null)
                throw new NullReferenceException("AssetManager is not initialized. Please call AssetManager.Initialize(bool).");
            return _assetLoader.LoadAsset<T>(path);
        }

        public static IEnumerator LoadAssetAsync<T>(string path, Action<T> onComplete) where T : UnityEngine.Object
        {
            if (_assetLoader == null)
                throw new NullReferenceException("AssetManager is not initialized. Please call AssetManager.Initialize(bool).");
            return _assetLoader.LoadAssetAsync<T>(path, onComplete);
        }

        public static void UnloadAsset<T>(T obj) where T : UnityEngine.Object
        {
            if (_assetLoader == null)
                throw new NullReferenceException("AssetManager is not initialized. Please call AssetManager.Initialize(bool).");
            _assetLoader.UnloadAsset(obj);
        }
    }
}