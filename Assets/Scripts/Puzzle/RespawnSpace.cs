using UnityEngine;

public class RespawnSpace : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Vector2 GetRespawnPosition(Vector2 playerOffset)
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            collider.bounds.extents.y,
            LayerMask.GetMask("TransparentFX", "Ground")
        );

        Vector2 ret = collider.bounds.min;
        if (hit != null)
        {
            ret = new Vector2(hit.point.x, hit.point.y + playerOffset.y);
        }
        return ret;
    }
    private void OnTriggerEnter2D(Collider2D obj)
    {
        if (!obj.gameObject.CompareTag("Player")) return;
        obj.gameObject.GetComponent<PlayerDeathRespawn>().setSpawnPoint(CameraManager.Instance.currentCameraSpace, GetRespawnPosition(obj.gameObject.GetComponent<BoxCollider2D>().bounds.size));
    }
}
