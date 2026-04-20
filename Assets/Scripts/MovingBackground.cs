using UnityEngine;

public class MovingBackground : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private float targetOffset = 19.8f;
    
    private Vector3 startPoint;

    private void Start()
    {
        startPoint = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
        
        if ((startPoint.x - transform.position.x) > targetOffset)
        {
            transform.position = new Vector2(startPoint.x, startPoint.y);
        }
        
    }
}
