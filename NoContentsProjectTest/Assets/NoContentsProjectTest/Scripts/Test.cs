using AddressableAssets;
using AddressableAssets.Types;

using UnityEngine;

public class Test : MonoBehaviour
{
    /* 필드 */
    [SerializeField] Models m_Model;

    /* MonoBehaviour */
    void Start()
    {
        // TODO 좀 더 좋은 방법이 없을까?
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
        if (m_Model == Models.None)
        {
            Debug.LogError("Model Type 을 선택해야 합니다.");
            return;
        }

        AddressablesManager<Models>.InstantiateAsync(m_Model);
    }
}
