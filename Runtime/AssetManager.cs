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
        private static IAssetLoader AssetLoader
        {
            get
            {
                if (UseAB)
                    _assetLoader = new BundledAssetLoader();
                else
                    _assetLoader = new DatabaseAssetLoader();

                return _assetLoader;
            }
        }
#if UNITY_EDITOR
        public static bool UseAB = false;
#else
        public static bool UseAB = true;
#endif

        public static T LoadAsset<T>(string path) where T : UnityEngine.Object
        {
            return AssetLoader.LoadAsset<T>(path);
        }

        public static IEnumerator LoadAssetAsync<T>(string path, Action<T> onComplete) where T : UnityEngine.Object
        {
            return AssetLoader.LoadAssetAsync<T>(path, onComplete);
        }

        public static void UnloadAsset<T>(T obj) where T : UnityEngine.Object
        {
            AssetLoader.UnloadAsset(obj);
        }
    }
}