using AddressableAssets;
using AddressableAssets.Types;

using UnityEngine;
using UnityEngine.AddressableAssets;

public class Test : MonoBehaviour
{
    /* 필드 */
    [SerializeField] ModelType modelType;

    /* MonoBehaviour */
    void Start()
    {
        // AddressablesManager 가 초기화된 이후 초기화 진행
        if (AddressablesManager.IsInitialized)
        {
            Init();
        }
        else
        {
            AddressablesManager.OnInitialized += Init;
        }
    }

    /* 메서드 */
    void Init()
    {
        // 어드레서블 에셋 스폰
        Spawn();
    }

    void Spawn()
    {
        if (modelType == ModelType.None)
        {
            Debug.LogError("Model Type 을 선택해야 합니다.");
            return;
        }

        var address = AddressablesManager.GetAddress(modelType);
        Addressables.InstantiateAsync(address);
    }
}
