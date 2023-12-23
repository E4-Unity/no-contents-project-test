using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AddressableAssets.Types;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace AddressableAssets
{
    namespace Types
    {
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
        // 매핑 목록
        static Dictionary<ModelType, string> ModelMap;

        // TODO 어드레서블 스크립터블 오브젝트로 대체
        // Remote Catalog 주소
        static string[] RemoteCatalogPaths =
        {
            "https://e4-unity.github.io/contents-download-test/StandaloneWindows64/catalog_1.0.json"
        };

        public static List<Task> Tasks = new List<Task>();

        /* 초기화 */
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void Init()
        {
            // Remote 카탈로그 로드
            LoadRemoteCatalogs();

            // Enum to Address 매핑
            RegisterAddressNames(ref ModelMap, "Models/");
        }

        static void LoadRemoteCatalogs()
        {
            foreach (var catalogPath in RemoteCatalogPaths)
            {
                Tasks.Add(Addressables.LoadContentCatalogAsync(catalogPath).Task);
            }
        }

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

        /* API */
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
    }
}
