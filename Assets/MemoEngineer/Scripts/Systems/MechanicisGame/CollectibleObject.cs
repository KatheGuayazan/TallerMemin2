using UnityEngine;

public class CollectibleObject : MonoBehaviour, IPlayerCollision, IPoolable
{
    private ObjectPool pool;

    private void OnEnable()
    {
        ScoreEvents.FinishGame += OnReturnToPool;
    }

    private void OnDisable()
    {
        ScoreEvents.FinishGame -= OnReturnToPool;
    }


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
            ScoreEvents.OnBadCollected?.Invoke();
            pool.Return(gameObject);
        }
    }

    public void OnReturnToPool()
    {

        pool.Return(gameObject);
    }

    // ------------------ Collectible logic ------------------
    public void OnPlayerCollision()
    {

        ScoreEvents.OnGoodCollected?.Invoke();
    }
}