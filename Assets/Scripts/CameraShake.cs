using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Transform of the GameObject you want to shake
    private Transform _transform;
 
    // Desired duration of the shake effect
    public float shakeDuration = 0f;
 
    // A measure of magnitude for the shake. Tweak based on your preference
    private float _shakeMagnitude = 0.3f;
 
    // A measure of how quickly the shake effect should evaporate
    private float _dampingSpeed = 1.0f;
 
    // The initial position of the GameObject
    private Vector3 _initialPosition;

    void Awake()
    {
        if (_transform == null)
            _transform = GetComponent(typeof(Transform)) as Transform;
    }
    
    void OnEnable()
    {
        _initialPosition = _transform.localPosition;
    }
    
    void Update()
    {
        if (shakeDuration > 0)
        {
            _transform.localPosition =  _initialPosition + Random.insideUnitSphere * _shakeMagnitude;
            shakeDuration            -= Time.deltaTime * _dampingSpeed;
        }
        else
        {
            shakeDuration            = 0f;
            _transform.localPosition = _initialPosition;
        }
    }
    
    public void TriggerShake()
    {
        shakeDuration = 0.3f;
    }
}
