using UnityEngine;

public class ProbeController : MonoBehaviour
{
    public float y = 0;
    ReflectionProbe probe;

    void Start()
    {
        probe = GetComponent<ReflectionProbe>();
    }

    void Update()
    {
        probe.transform.position = new Vector3(
            Camera.main.transform.position.x,
            y,
            Camera.main.transform.position.z
        );

        probe.RenderProbe();
    }
}