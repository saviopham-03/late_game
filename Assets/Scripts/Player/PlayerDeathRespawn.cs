using UnityEngine;

public class PlayerDeathRespawn : MonoBehaviour
{
    private Animator _animator;
    public BoxCollider2D cameraSpace;
    public Vector2 spawnLocation;
    private GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
        player = gameObject;
        cameraSpace = CameraManager.Instance.currentCameraSpace;
        spawnLocation = gameObject.transform.position;
    }

    public void setSpawnPoint(BoxCollider2D newSpace, Vector2 newSpawn)
    {
        cameraSpace = newSpace;
        spawnLocation = newSpawn;
    }

    public void killPlayer()
    {  
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        _animator.SetBool("died", true);
    }

    public void revivePlayer()
    {
        GetComponent<Rigidbody2D>().position = spawnLocation;
        CameraManager.Instance.SwitchCamera(cameraSpace);
        _animator.SetBool("died", false);
    }

    public void unlockPlayer()
    {
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
    }
    
}
