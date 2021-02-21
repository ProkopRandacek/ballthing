using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

public enum TransferType { Export, Import }

public class DataTransfer : MonoBehaviour
{
    public TransferType type;
    public GameObject   death;
    public GameObject   checkPoint;
    public GameObject   wall;
    public GameObject   floor;

    private void OnMouseDown()
    {
        if (type == TransferType.Export)
        {
            string data = Export();
            //Debug.Log(data);
            data = Compress(data);
            //Debug.Log(data);
            GUIUtility.systemCopyBuffer = data;
        }
        else if (type == TransferType.Import)
            try
            {
                Import(Decompress(GUIUtility.systemCopyBuffer));
            }
            catch (Exception e)
            {
                Debug.LogError("Import failed");
                //Debug.LogError(e);
            }
    }

    private string Export()
    {
        GameObject[] gos = FindObjectsOfType<GameObject>();

        string data = "";
        
        foreach (GameObject go in gos)
        {
            string n = go.name;
            switch (n)
            {
                case "Death(Clone)":
                    data += "D" + Vec2Str(go.transform.position) + ";";
                    break;
                case "CheckPoint(Clone)":
                    data += "C" + Vec2Str(go.transform.position) + ";";
                    break;
                case "Wall(Clone)":
                    data += "W" + Vec2Str(go.transform.position) + "," + go.transform.localScale.y + ";";
                    break;
                case "Floor(Clone)":
                    data += "F" + Vec2Str(go.transform.position) + "," + go.transform.localScale.x + ";";
                    break;
                case "Goal":
                    if (go.transform.parent != null)
                        break;
                    data += "G" + Vec2Str(go.transform.position) + ";";
                    break;
                case "Ball":
                    if (go.transform.parent != null)
                        break;
                    data += "B" + Vec2Str(go.transform.position) + ";";
                    break;
            }
        }

        return data;
    }

    private void Import(string data)
    {
        //Debug.Log("Import data: " + data);
        foreach (GameObject go in FindObjectsOfType<GameObject>())
            switch (go.name)
            {
                case "Death(Clone)":
                case "CheckPoint(Clone)":
                case "Wall(Clone)":
                case "Floor(Clone)":
                    Destroy(go);
                    break;
            }

        string[] objects = data.Remove(data.Length - 1, 1).Split(';');

        foreach (string t in objects)
        {
            char item = t[0];
            GameObject go = item switch
            {
                'D' => Instantiate(death),
                'C' => Instantiate(checkPoint),
                'G' => GameObject.Find("Goal"),
                'W' => Instantiate(wall),
                'F' => Instantiate(floor),
                'B' => GameObject.Find("Ball"),
                _   => Instantiate(wall)
            };

            string[] vec = t.Remove(0, 1).Split(',');
            Vector2  pos = new Vector2(float.Parse(vec[0]), float.Parse(vec[1]));
            go.transform.position = pos;
            
            if (item == 'W')
                go.transform.localScale = new Vector3(0.2f, float.Parse(vec[2]), 1.0f);
            else if (item == 'F')
                go.transform.localScale = new Vector3(float.Parse(vec[2]), 0.2f, 1.0f);
            
            go.GetComponent<EditorItem>().clone = true;
            
            if ((item == 'B') || (item == 'G'))
                go.transform.SetParent(null);
        }
    }

    private string Vec2Str(Vector2 v)
    {
        return $"{v.x},{v.y}";
    }
    
    private string Compress(string uncompressedString)
    {
        if (string.IsNullOrEmpty(uncompressedString)) return uncompressedString;

        using MemoryStream compressedStream   = new MemoryStream();
        using MemoryStream uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(uncompressedString));
        using (DeflateStream compressorStream = new DeflateStream(compressedStream, CompressionMode.Compress, true))
        {
            uncompressedStream.CopyTo(compressorStream);
        }

        return Convert.ToBase64String(compressedStream.ToArray());
    }

    private string Decompress(string compressedString)
    {
        if (string.IsNullOrEmpty(compressedString)) return compressedString;

        using MemoryStream decompressedStream = new MemoryStream();
        using MemoryStream compressedStream   = new MemoryStream(Convert.FromBase64String(compressedString));
        using (DeflateStream decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
        {
            decompressorStream.CopyTo(decompressedStream);
        }

        return Encoding.UTF8.GetString(decompressedStream.ToArray());
    }
}