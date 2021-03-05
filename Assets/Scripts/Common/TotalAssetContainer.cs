using System.Collections.Generic;

namespace UnityEngine.Assets
{
    [CreateAssetMenu(fileName = "Total Asset Container", menuName = "Scriptable Object/Total Asset Container", order = int.MaxValue)]
    public class TotalAssetContainer : ScriptableObject
    {
        // 사용법
        // 1) Resources 폴더에 TotalAssetContainer 추가
        // 2) ManagedAssets폴더에 있는 스크립터블 오브젝트 추가
        // 3) Collect하면 에셋 수집 완료
        [SerializeField] List<IAssetContainer> containers = new List<IAssetContainer>();

        public T GetLoader<T>() where T : class, IAssetContainer
        {
            return containers.Find(container => container.GetType() == typeof(T)) as T;
        }

        public void CollectDirectories()
        {
            containers.ForEach(directory => directory.CollectAssets());
        }
    }
}