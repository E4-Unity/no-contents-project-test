using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AddressableAssets.Types;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace AddressableAssets
{
    namespace Types
    {
        /* Enum 정의 */
        public enum ModelType
        {
            // 기본값
            None,

            // 이곳에 모델 이름 등록
            RobotKyle
        }
    }

    public abstract class AddressablesManager
    {
        /* 필드 */
        // Enum 매핑
        static Dictionary<ModelType, string> ModelMap;

        // 초기화
        public static bool IsInitialized;
        public static event Action OnInitialized;

        /* 초기화 */
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void Init()
        {
            // 이벤트 바인딩
            OnInitialized += () => IsInitialized = true;

            // Enum to Address 매핑
            RegisterAddressNames(ref ModelMap, "Models/");

            // Remote 카탈로그 로드
            LoadRemoteCatalogs();
        }

        /* API */
        // 입력된 Enum 에 대응하는 Address Name 반환
        public static string GetAddress<TEnum>(TEnum type) where TEnum : Enum
        {
            switch (type)
            {
                case ModelType prefabType:
                    return ModelMap.TryGetValue(prefabType, out var address) ? address : string.Empty;

                default:
                    return string.Empty;
            }
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

            // 모든 Task 가 처리될 때까지 대기
            await Task.WhenAll(loadRemoteCatalogTasks);

            // 초기화 완료
            OnInitialized?.Invoke();
        }

        // Enum to Address 매핑
        static void RegisterAddressNames<TEnum>(ref Dictionary<TEnum, string> map, string path) where TEnum : Enum
        {
            // 모든 Enum 값 가져오기 (None 제외)
            var types = Enum.GetValues(typeof(TEnum));
            var prefabTypes = new TEnum[types.Length - 1];
            Array.Copy(types, 1, prefabTypes, 0, prefabTypes.Length);

            // Dictionary 초기화
            map = new Dictionary<TEnum, string>(prefabTypes.Length);

            // Dictionary 요소 등록
            foreach (var prefabType in prefabTypes)
            {
                map.Add(prefabType, path + prefabType);
            }
        }
    }
}
