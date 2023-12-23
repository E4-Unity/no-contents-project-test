using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AddressableAssets
{
    namespace Types
    {
        /* Enum 정의 (Enum 이름은 반드시 상위 Address 이름과 동일해야 한다) */
        public enum Models
        {
            // 기본값
            None,

            // 이곳에 모델 이름 등록
            RobotKyle
        }
    }

    public abstract class AddressablesManager<TEnum> where TEnum : Enum
    {
        /* 필드 */
        // Enum to Address Map
        static readonly Dictionary<TEnum, string> AddressMap = AddressablesManager.CreateAddressMap<TEnum>();

        /* API */
        public static AsyncOperationHandle<GameObject> InstantiateAsync(TEnum type, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true)
        {
            var address = GetAddress(type);
            return Addressables.InstantiateAsync(address, parent, instantiateInWorldSpace, trackHandle);
        }

        public static AsyncOperationHandle<TObject> LoadAssetAsync<TObject>(TEnum type)
        {
            var address = GetAddress(type);
            return Addressables.LoadAssetAsync<TObject>(address);
        }

        /* 메서드 */
        static string GetAddress(TEnum type) => AddressMap.TryGetValue(type, out var address) ? address : string.Empty;
    }

    public abstract class AddressablesManager
    {
        // 초기화
        public static bool IsInitialized;
        public static event Action OnInitialized;

        /* 초기화 */
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void Init()
        {
            // 이벤트 바인딩
            OnInitialized += () => IsInitialized = true;

            // Remote 카탈로그 로드
            LoadRemoteCatalogs();
        }

        /* API */
        // Enum to Address 매핑
        public static Dictionary<TEnum, string> CreateAddressMap<TEnum>() where TEnum : Enum
        {
            // 모든 Enum 값 가져오기 (None 제외)
            var types = Enum.GetValues(typeof(TEnum));
            var prefabTypes = new TEnum[types.Length - 1];
            Array.Copy(types, 1, prefabTypes, 0, prefabTypes.Length);

            // Dictionary 초기화
            var addressMap = new Dictionary<TEnum, string>(prefabTypes.Length);

            // Dictionary 요소 등록
            foreach (var prefabType in prefabTypes)
            {
                addressMap.Add(prefabType, typeof(TEnum).Name + "/" + prefabType);
            }

            return addressMap;
        }

        /* 메서드 */
        // Remote 카탈로그 로드
        static async void LoadRemoteCatalogs()
        {
            // StreamingAssets/Catalogs 폴더 정보 불러오기
            var catalogFolderPath = Path.Combine(Application.streamingAssetsPath, "Catalogs");
            var catalogDirectoryInfo = new DirectoryInfo(catalogFolderPath);
            var files = catalogDirectoryInfo.GetFiles();

            // Remote Catalog 경로 추출 (.json 파일 필터링)
            List<string> remoteCatalogPaths = new List<string>(files.Length);
            foreach (var file in files)
            {
                if (file.Extension == ".json")
                {
                    remoteCatalogPaths.Add(file.FullName);
                }
            }

            // Load Remote Catalog Task 생성
            Task[] loadRemoteCatalogTasks = new Task[remoteCatalogPaths.Count];
            for (int i = 0; i < loadRemoteCatalogTasks.Length; i++)
            {
                loadRemoteCatalogTasks[i] = Addressables.LoadContentCatalogAsync(remoteCatalogPaths[i]).Task;
            }

            // TODO Task.WaitAll 을 쓰고 싶은데 무한 루프에 빠져버린다.
            // 모든 Task 가 처리될 때까지 대기
            await Task.WhenAll(loadRemoteCatalogTasks);

            // 초기화 완료
            OnInitialized?.Invoke();
        }
    }
}
