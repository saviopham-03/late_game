using UnityEngine;

public class DamageTile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void OnTriggerEnter2D(Collider2D obj)
    {
        if (!obj.gameObject.CompareTag("Player")) return;
        obj.GetComponent<PlayerDeathRespawn>().killPlayer();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
