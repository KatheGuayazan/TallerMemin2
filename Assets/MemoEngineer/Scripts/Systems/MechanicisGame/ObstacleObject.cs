using UnityEngine;

public class ObstacleObject : MonoBehaviour, IPlayerCollision, IPoolable
{
    private ObjectPool pool;

    // ------------------ Receives pool reference ------------------
    public void SetPool(ObjectPool pool)
    {
        this.pool = pool;
    }

    // ------------------ Trigger detection ------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerCollision();

            pool.Return(gameObject);
        }

        if (other.CompareTag("Ground"))
        {

            ScoreEvents.OnToxicDodged?.Invoke();
            pool.Return(gameObject);
        }
    }

    // ------------------ Obstacle logic ------------------
    public void OnPlayerCollision()
    {
        
        ScoreEvents.FinishGame?.Invoke();
    }
}