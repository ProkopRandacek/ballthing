using System;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public void MoveArrow(Vector2 a, Vector2 b)
    {
        Vector2 v2 = (b + a) / 2; // average position
        Vector3 v3 = v2;
        v3.z               = 1;
        transform.position = v3;
        float yDiff = b.y - a.y;
        float xDiff = b.x - a.x;
        float angle = (float) ((180 / Math.PI) * Math.Atan(yDiff / xDiff));
        transform.eulerAngles = new Vector3(0, 0, angle);
        float length = Vector2.Distance(b, a);
        transform.localScale = new Vector3(length * 2, 0.2f, 1);
    }
}
