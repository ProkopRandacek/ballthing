using UnityEngine;

public enum Type
{
    Goal,
    Checkpoint,
    Death
}

public class GoalController : MonoBehaviour
{
    public Type type;
    public bool editor;
    
    public BallController ball;

    private void Awake()
    {
        if (editor) return;
        ball = GameObject.Find("Ball").GetComponent<BallController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (editor) return;
        switch (type)
        {
            case Type.Death:
                ball.Die();
                break;
            case Type.Checkpoint:
                ball.HitCheckpoint();
                ball.checkpointPosition = transform.position;
                break;
            case Type.Goal:
                ball.HitGoal();
                ball.checkpointPosition = transform.position;
                break;
        }
    }
}
