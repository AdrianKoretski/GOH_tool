using UnityEditor;
using UnityEngine;

namespace GOH
{
    public class Visualiser : MonoBehaviour
    {
        public static GameObject CreatePoint(Transform parent, Vector2 position, Color color = new Color(), string name = "")
        {
            GameObject go = new GameObject(name, typeof(SpriteRenderer));
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            go.name = name;
            go.transform.position = position;
            go.transform.parent = parent;
            sr.color = color;
            sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            return go;
        }

        public static GameObject CreateLine(Transform parent, Vector2 position_0, Vector2 position_1, Color color_0 = new Color(), Color color_1 = new Color(), string name = "")
        {
            GameObject go = new GameObject(name, typeof(LineRenderer));
            LineRenderer lr = go.GetComponent<LineRenderer>();
            go.name = name;
            go.transform.parent = parent;
            go.transform.position = (position_0 + position_1) / 2;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = color_0;
            lr.endColor = color_1;
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.SetPosition(0, position_0);
            lr.SetPosition(1, position_1);
            return go;
        }

        public static GameObject CreateLine(GameObject point_0, GameObject point_1, string name = "")
        {
            GameObject go = new GameObject(name, typeof(LineRenderer));
            LineRenderer lr = go.GetComponent<LineRenderer>();
            SpriteRenderer sr_0 = point_0.GetComponent<SpriteRenderer>();
            SpriteRenderer sr_1 = point_1.GetComponent<SpriteRenderer>();
            go.name = name;
            go.transform.parent = point_0.transform.parent;
            go.transform.position = (point_0.transform.position + point_1.transform.position) / 2;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = sr_0.color;
            lr.endColor = sr_1.color;
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.SetPosition(0, point_0.transform.position);
            lr.SetPosition(1, point_1.transform.position);
            return go;
        }
    }
}