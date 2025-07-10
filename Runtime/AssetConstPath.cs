using System.IO;

namespace EdenMeng.AssetManager
{
    public class AssetConstPath
    {
        private static IAssetBundlePath m_Path;

        public const string AssetBundlesDirName = "AssetBundles";
        public const string BundleInfoBundleName = "bundleinfo";
        public const string BundleInfoAssetName = "BundlesInfo";

        public static string BundlePath => Path.Combine(m_Path.Path, AssetBundlesDirName);
        public static string BundleInfoPath => Path.Combine(BundlePath, BundleInfoBundleName);

        public static void Initialize(IAssetBundlePath path)
        {
            m_Path = path;
        }
    }

    public interface IAssetBundlePath
    {
        public string Path { get; }
    }

    public struct DefaultAssetBundlePath : IAssetBundlePath
    {
        public string Path => "Assets";
    }
}