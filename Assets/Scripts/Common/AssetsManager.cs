using System.Collections.Generic;
using System;

namespace UnityEngine.Assets
{
    public interface IAssetContainer
    {
        void CollectAssets();
    }

    [Serializable]
    public class AssetManager<T> : ScriptableObject, IAssetContainer
        where T : Object
    {
        [SerializeField] protected string assetDirectory;
        [SerializeField] List<T> assets;
        public List<T> Assets => assets;

        public void CollectAssets()
        {
            assets = AssetUtility.LoadAllAssetsAtPath<T>(assetDirectory);
        }
    }
}
