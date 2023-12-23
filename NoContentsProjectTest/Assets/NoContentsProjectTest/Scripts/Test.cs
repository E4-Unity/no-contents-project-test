using System.Threading.Tasks;
using AddressableAssets;
using AddressableAssets.Types;

using UnityEngine;
using UnityEngine.AddressableAssets;

public class Test : MonoBehaviour
{
    [SerializeField] ModelType modelType;

    void Start()
    {
        AsyncStart();
    }

    async void AsyncStart()
    {
        // 로딩 대기
        await Task.WhenAll(AddressablesManager.Tasks);

        // 스폰
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
