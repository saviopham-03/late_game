using UnityEngine;

public class CloneOrb : MonoBehaviour
{
    [SerializeField] private PlayerColour cloneColour;
    [SerializeField] private GameObject player_obj;
    private CloneManager _manager;
    [SerializeField] private Animator _animator;
    private bool active = true;

    void Start()
    {
        _manager = CloneManager.Instance;
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!active || other.GetComponent<PlayerMovement>() == null) return;
        active = false;
        GameObject clone = Instantiate(player_obj, new Vector3(transform.position.x, transform.position.y, 5), Quaternion.identity);
        _manager.addClone(clone);
        _animator.SetTrigger("pickup");
        PlayerColourController playerControl = clone.GetComponent<PlayerColourController>();
        playerControl.SetColour(cloneColour);
    }
}
