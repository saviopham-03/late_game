using UnityEngine;

public class PlayerDeathRespawn : MonoBehaviour
{

    [SerializeField] private float killDepth = 0.25f;
    private Animator _animator;
    public BoxCollider2D cameraSpace;
    public Vector2 spawnLocation;
    private GameObject player;
    private BoxCollider2D _collider;
    private bool dying = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider2D>();
        player = gameObject;
        cameraSpace = CameraManager.Instance.currentCameraSpace;
        spawnLocation = gameObject.transform.position;
    }

    void FixedUpdate()
    {
        Collider2D[] nearbyColliders =
            Physics2D.OverlapBoxAll(
                _collider.bounds.center,
                _collider.bounds.size,
                0f
            );
        
        foreach (Collider2D other in nearbyColliders)
        {
            if (other == _collider ||
                other.isTrigger ||
                other.attachedRigidbody == _collider.attachedRigidbody ||
                other.CompareTag("Red") ||
                other.CompareTag("Blue") ||
                other.CompareTag("Green") ||
                other.CompareTag("Yellow"))
                continue;

            ColliderDistance2D distance =
                _collider.Distance(other);
            
            if (Mathf.Abs(distance.distance) > killDepth && !dying)
            {
                Debug.Log("died");
                killPlayer();
            }
        }
    }

    public void setSpawnPoint(BoxCollider2D newSpace, Vector2 newSpawn)
    {
        cameraSpace = newSpace;
        spawnLocation = newSpawn;
    }

    public void killPlayer()
    {  
        if (!dying) {
        dying = true;
        player.GetComponent<PlayerGrapple>().DetachGrapple();
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        _animator.SetBool("died", true);
        }
        
    }

    public void revivePlayer()
    {
        GetComponent<Rigidbody2D>().position = spawnLocation;
        CameraManager.Instance.SwitchCamera(cameraSpace);
        _animator.SetBool("died", false);
    }

    public void unlockPlayerMovement()
    {
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        dying = false;
    }
    
}
