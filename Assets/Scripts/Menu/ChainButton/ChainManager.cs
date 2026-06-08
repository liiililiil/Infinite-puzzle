using System.Collections.Generic;
using UnityEngine;

public class ChainManager : MonoBehaviour
{
    [SerializeField]
    List<Chain> chains;

    private void Start()
    {
        for (int i = 0; i < chains.Count; i++)
        {
            chains[i].Bind((byte)i, this);
        }

        // Bind가 모두 끝난 후, 안전하게 초기값을 설정합니다.
        if (chains.Count > 0)
        {
            // 첫 번째 체인을 명시적으로 선택
            chains[0].Select();
        }
    }

    public void Select(byte index)
    {
        for (int i = 0; i < chains.Count; i++)
        {
            if (i == index) continue;
            chains[i].UnSelect();
        }
    }
}
