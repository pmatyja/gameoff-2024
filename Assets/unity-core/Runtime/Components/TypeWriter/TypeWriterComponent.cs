using UnityEngine;

public class TypeWriterComponent : MonoBehaviour
{
    [SerializeField]
    private bool debug;

    [SerializeField]
    [Inline]
    private TypeWriterEffect component = new();
    public TypeWriterEffect Component => this.component;

    private Texture2D backgroundTexture;

    private void Start()
    {
        this.backgroundTexture = MakeTex(2, 2, new Color(1.0f, 1f, 1.0f, 1.0f));
    }

    public void Update()
    {
        this.component.Advance();
    }

    private void OnGUI()
    {
        if (this.debug)
        {
            GUI.Box(new Rect(16, 16, 320, 480), this.component.Text, new GUIStyle
            {
                alignment = TextAnchor.UpperLeft,
                normal = new GUIStyleState
                {
                    background = this.backgroundTexture
                }
            });
        }
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}