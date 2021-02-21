using UnityEngine;

public class StartController : MonoBehaviour
{
    public bool     start = true;
    public TextMesh tm;

    private GameObject _menu;

    private void Awake()
    {
        _menu     = GameObject.Find("Menu");
    }

    private void OnMouseDown()
    {
        if (GameObject.Find("Ball").transform.parent != null)
            return;
        
        GameObject[] gos = FindObjectsOfType<GameObject>();

        foreach (GameObject go in gos)
        {
            string n = go.name;
            switch (n)
            {
                case "Death(Clone)":
                case "CheckPoint(Clone)":
                case "Goal":
                    go.GetComponent<GoalController>().editor = !start;
                    go.GetComponent<GoalController>().ball = GameObject.Find("Ball").GetComponent<BallController>();
                    go.GetComponent<EditorItem>().enabled = !start;
                    break;
                case "Wall(Clone)":
                case "Floor(Clone)":
                    go.GetComponent<EditorItem>().enabled = !start;
                    break;
                case "Ball":
                    go.GetComponent<BallController>().enabled = start;
                    go.GetComponent<BallController>().editor = !start;
                    go.GetComponent<BallController>().ResetFlags();
                    go.GetComponent<EditorItem>().enabled = !start;
                    break;
            }
        }

        _menu.SetActive(!start);
        if (start)
        {
            transform.SetParent(Camera.main.transform);
            tm.text = "Stop";
        }
        else
        {
            transform.SetParent(_menu.transform);
            tm.text = "Start";
        }

        start = !start;
    }
}
