using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideStageWarnEffect : MonoBehaviour
{

    private void OnEnable()
    {
        Invoke("HideThis", 1f);
    }

    private void HideThis()
    {
        PoolManager.Instance.PushObj("Component/Enemy/Boss2RocketsWarn", this.gameObject);
    }

}
