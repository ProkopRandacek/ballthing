using System;
using UnityEngine;

public class EditorItem : MonoBehaviour
{
    public bool clone;
    public bool dragging;
    public bool destroyable = true;
    
    public EditorItem parent;

    public  bool    scaling;
    
    private GameObject _draggingGO;
    private Camera     _camera;

    private void Start()
    {
        _camera = Camera.main;

        try
        {
            gameObject.GetComponent<BallController>().collider.isTrigger = true;
        }
        catch
        {
            // For Editor Items that are not Ball
        }
    }

    public void Update()
    {
        if (dragging)
        {
            Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
            mousePos.x = Mathf.Round(mousePos.x);
            mousePos.y = Mathf.Round(mousePos.y);

            if (_draggingGO.name.StartsWith("Floor"))
                mousePos += new Vector3(0, 0.5f, 0);
            if (_draggingGO.name.StartsWith("Wall"))
                mousePos += new Vector3(0.5f, 0, 0);
            
            mousePos.z = 5; // originally at the same z as the camera -> camera cant see that
            
            _draggingGO.transform.position = mousePos;
        }

        if (scaling)
        {
            Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
            int diff = (int) Math.Round(transform.position.x - mousePos.x);
            diff = (diff * 2) + 1;

            Vector3 newScale = gameObject.name switch
            {
                "Wall(Clone)"  => new Vector3(0.2f, diff, 1),
                "Floor(Clone)" => new Vector3(diff, 0.2f, 1),
                _              => Vector3.zero
            };

            transform.localScale = newScale;
        }
    }

    private void OnMouseUp()
    {
        if (clone)
        {
            if (parent != null)
                parent.dragging = false;
            transform.SetParent(null);
        }

        scaling  = false;
        dragging = false;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1) && clone && destroyable) // Destroy on rmb
            Destroy(gameObject);
        else if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift) && // Scale on shift + lmb
            ((gameObject.name == "Wall(Clone)") || (gameObject.name == "Floor(Clone)")))
            scaling = true;
        else if (Input.GetMouseButtonDown(0)) // Drag on lmb
        {
            if (!clone)
            {
                _draggingGO = Instantiate(gameObject, transform.position, Quaternion.identity);
                dragging = true;
                _draggingGO.GetComponent<EditorItem>().parent = this;
                _draggingGO.GetComponent<EditorItem>().clone = true;
                _draggingGO.transform.localScale = transform.lossyScale;
            }
            else
            {
                _draggingGO = gameObject;
                dragging = true;
            }
        }
    }
}
