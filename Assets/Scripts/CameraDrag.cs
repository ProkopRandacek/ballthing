using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    private float   _dist = -10.0f;
    private Vector3 _mouseStart;
    private Camera  _camera;

    private void Start()
    {
        _camera = Camera.main;
        _dist   = transform.position.z;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            _mouseStart   = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _dist);
            _mouseStart   = _camera.ScreenToWorldPoint (_mouseStart);
            _mouseStart.z = transform.position.z;
        } 
        else if (Input.GetMouseButton(2))
        {
            Vector3 pos       = transform.position;
            Vector3 mouseMove = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _dist);
            mouseMove          = _camera.ScreenToWorldPoint(mouseMove);
            mouseMove.z        = pos.z;
            transform.position = pos - (mouseMove - _mouseStart);
        }
    }
}
