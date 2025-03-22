using UnityEngine;
using System.Collections;

public class ProbeController : MonoBehaviour
{

    ReflectionProbe probe;

    void Start()
    {
        this.probe = GetComponent<ReflectionProbe>();
    }

    void Update()
    {
        this.probe.transform.position = new Vector3(
            Camera.main.transform.position.x,
            (float)0,
            Camera.main.transform.position.z
        );

        probe.RenderProbe();
    }
}