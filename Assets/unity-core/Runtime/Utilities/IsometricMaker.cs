using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IsometricMaker : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private Light mainLight;

    [SerializeField]
    private Quaternion lightRotation = Quaternion.Euler(45.0f, 45.0f, 0.0f);

    [SerializeField]
    private Color backgroundColor = new Color(1.0f, 0.0f, 1.0f, 1.0f);

    [SerializeField]
    private Vector3 snapshopPoint = Vector3.zero;

    [SerializeField]
    private bool changeSize;

    [SerializeField]
    [Range(1, 8)]
    private int unitBase = 2;

    [SerializeField]
    [Range(32, 4096)]
    private int baseWidth = 128;

    [SerializeField]
    [Range(32, 4096)]
    private int baseHeight = 128;

    [SerializeField]
    [Range(1, 8)]
    private int faces = 4;

    [SerializeField]
    [Readonly]
    private float progress = 0;

    public List<GameObject> prefabs = new();

    [ContextMenu("Make")]
    public void Make()
    {
        this.StopAllCoroutines();
        this.StartCoroutine(this.OnUpate());
    }

    [ContextMenu("Open Prefabs")]
    public void OpenPrefabs()
    {
        this.prefabs.Clear();

        var folderPath = EditorOnly.OpenFolderPanel("Locate prefabs fllder");

        if (Directory.Exists(folderPath))
        {
            var files = Directory.GetFiles(folderPath, "*.prefab");

            foreach (var file in files)
            {
                var path = Path.GetRelativePath(Path.Combine(Application.dataPath), file);

                var prefab = EditorOnly.LoadAsset<GameObject>(Path.Combine("Assets", path));
                if (prefab != null)
                {
                    this.prefabs.Add(prefab);
                }
            }
        }
    }

    public IEnumerator OnUpate()
    {
        this.progress = 0;

        var index = 0;
        var rotationStep = 360 / this.faces;

        if (this.mainLight != null)
        {
            this.mainLight.transform.localRotation = this.lightRotation;
        }

        while (index < this.prefabs.Count)
        {
            var gameObject = this.prefabs[index];
            var boundingBox = BoundsExtensions.getBounds(gameObject);

            var volume = new Vector3Int
            (
                (int)Math.Round(boundingBox.size.x, 0, MidpointRounding.AwayFromZero),
                (int)Math.Round(boundingBox.size.y, 0, MidpointRounding.AwayFromZero),
                (int)Math.Round(boundingBox.size.z, 0, MidpointRounding.AwayFromZero)
            );

            if (volume.x >= unitBase) volume.x /= unitBase;
            if (volume.y >= unitBase) volume.y /= unitBase;
            if (volume.z >= unitBase) volume.z /= unitBase;

            this.mainCamera.backgroundColor = this.backgroundColor;

            if (this.changeSize)
            {
                this.mainCamera.orthographicSize = Math.Max(1, Math.Max(Math.Max(volume.x, volume.z), volume.y));
            }

            Debug.Log($"{gameObject.name} ({volume})");

            for (int faceIndex = 0; faceIndex < this.faces; ++faceIndex)
            {
                var texture = MakeSnapshot
                (
                    gameObject,
                    -boundingBox.center,
                    Quaternion.AngleAxis(rotationStep * faceIndex, Vector3.up), 
                    this.mainCamera,
                    this.baseWidth,  //* Math.Max(volume.x, volume.z), 
                    this.baseHeight //* volume.y
                );
                var bytes = texture.EncodeToPNG();

                File.WriteAllBytes(Path.Combine(Application.dataPath, $"{gameObject.name}-{faceIndex * rotationStep}.png"), bytes);
            }

            this.progress = index++ / (float)this.prefabs.Count;

            yield return Wait.Seconds(0.0001f);
        }

        Debug.Log("Done");

        yield return null;
    }

    private static Texture2D MakeSnapshot(GameObject model, Vector3 position, Quaternion rotation, Camera renderCamera, int width, int height)
    {
        var activeRT = RenderTexture.active;
        var renderTexture = default(RenderTexture);

        var parent = new GameObject("pivot").transform;

        parent.gameObject.hideFlags = HideFlags.HideAndDontSave;

        var previewObject = GameObject.Instantiate(model, parent, false);

        previewObject.gameObject.hideFlags      = HideFlags.HideAndDontSave;
        previewObject.transform.localPosition   = position;
        previewObject.transform.localRotation   = Quaternion.identity;
        previewObject.transform.localScale      = Vector3.one;

        parent.transform.localRotation = rotation;

        try
        {
            var aspect = renderCamera.aspect;

            renderCamera.aspect = (float)width / height;

            renderTexture = RenderTexture.GetTemporary(width, height, 16);

            RenderTexture.active = renderTexture;

            renderCamera.targetTexture = renderTexture;
            renderCamera.Render();
            renderCamera.targetTexture = null;
            renderCamera.aspect = aspect;

            var result = new Texture2D(width, height, TextureFormat.RGB24, false);
            result.ReadPixels(new Rect(0f, 0f, width, height), 0, 0, false);
            result.Apply(false, false);

            return result;
        }
        finally
        {
            RenderTexture.active = activeRT;

            if (renderTexture)
            {
                RenderTexture.ReleaseTemporary(renderTexture);
            }

            UnityEngine.Object.DestroyImmediate(previewObject.gameObject);
            UnityEngine.Object.DestroyImmediate(parent.gameObject);
        }
    }
}