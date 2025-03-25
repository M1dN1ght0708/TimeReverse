using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Teleporter : MonoBehaviour
{
    [Header("��������")]
    public Vector3 targetPosition = new Vector3(0, 0.5f, 0); // Ŀ��λ��
    public KeyCode activateKey = KeyCode.F;  // ��������

    private bool isInRange = false;
    private Transform playerTransform;

    void Update()
    {
        if (isInRange && Input.GetKeyDown(activateKey))
        {
            TeleportPlayer();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
            playerTransform = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
            playerTransform = null;
        }
    }

    void TeleportPlayer()
    {
        if (playerTransform == null) return;

        CharacterController controller = playerTransform.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
            playerTransform.position = targetPosition;
            controller.enabled = true;
        }
        else
        {
            playerTransform.position = targetPosition;
        }
    }

    // ���Կ��ӻ�
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(targetPosition, Vector3.one * 0.5f);
        Gizmos.DrawLine(transform.position, targetPosition);
    }
}