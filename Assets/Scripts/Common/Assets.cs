using System.Collections.Generic;
using System.IO;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace UnityEngine.Assets
{
    public static class AssetUtility
    {
        public static List<T> LoadAllAssetsAtPath<T>(string targetPath, string searchPattern = null)
            where T : Object
        {
            var list = new List<T>();
            var directory = new DirectoryInfo(targetPath);

            if (searchPattern.IsNullOrEmpty())
                searchPattern = "*.*";

            var files = directory
                            .GetFiles(searchPattern, SearchOption.AllDirectories)
                            .Where(file => file.Extension != ".meta");

#if UNITY_EDITOR
            foreach (var file in files)
            {
                var path = file.FullName.Substring(Application.dataPath.Length - 6/*6 => Assets*/);
                var item = AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
                list.Add(item);
            }
#endif
            return list;
        }
    }

    public static class Assets
    {
        static TotalAssetContainer totalAssetContainer;

        static Assets()
        {
            totalAssetContainer = Resources.Load<TotalAssetContainer>("Total Asset Container");
            totalAssetContainer.CollectDirectories();
        }
    }
}