using UnityEngine;

public class MenuFloatingAnimation : MonoBehaviour
{
    [SerializeField] private float floatAmount;
    [SerializeField] private float floatSpeed;

    private bool direction;
    private Vector2 original_pos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        original_pos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float newY = original_pos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmount;

        transform.position = new Vector2(
            original_pos.x,
            newY
        );
    }
}
