using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;

    [Header("Horizontal Limits")]
    [SerializeField] private float minX = -5f;
    [SerializeField] private float maxX = 5f;


    private PlayerInputHandler inputHandler;

   void Start()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        Move();
    }

    // ------------------ Handles horizontal movement ------------------
    private void Move()
    {
        Vector3 position = transform.position;

        position.x += inputHandler.MoveInput * moveSpeed * Time.deltaTime;

        position.x = Mathf.Clamp(position.x, minX, maxX);

        transform.position = position;
    }

    // ------------------ Draw movement limits ------------------
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Vector3 leftPoint = new Vector3(minX, transform.position.y, 0f);
        Vector3 rightPoint = new Vector3(maxX, transform.position.y, 0f);

        Gizmos.DrawLine(leftPoint, rightPoint);

        Gizmos.DrawSphere(leftPoint, 0.2f);
        Gizmos.DrawSphere(rightPoint, 0.2f);
    }
}