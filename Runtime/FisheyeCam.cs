using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways, RequireComponent(typeof(Camera))]
public class FisheyeCam : MonoBehaviour
{
    [SerializeField]
    public Shader shader;

    [SerializeField, Range(512, 4096)]
    public int resolution = 512;

    Shader _shader;
    int _resolution = 512;
    CamInstance _instance;

    void OnEnable()
    {
        if (_shader != null)
        {
            if (_instance != null) _instance.Dispose();
            _instance = new(GetComponent<Camera>(), _shader, _resolution);
        }
    }

    void OnDisable()
    {
        if (_instance != null) _instance.Dispose();
        _instance = null;
    }

    void OnValidate()
    {
        if (_resolution != resolution)
        {
            _resolution = resolution;
            if (_instance != null) _instance.Resize(_resolution);
        }

        if (_shader != shader)
        {
            _shader = shader;
            OnEnable();
        }
    }

    void LateUpdate()
    {
        if (_instance != null) _instance.Render();
    }
}

public class CamInstance : IDisposable
{
    Camera _camera;
    RenderTexture _texture;
    Material _material;
    GameObject _canvas;

    public CamInstance(Camera camera, Shader shader, int resolution)
    {
        _camera = camera;

        _texture = new(resolution, resolution, 24)
        {
            dimension = UnityEngine.Rendering.TextureDimension.Cube,
            filterMode = FilterMode.Trilinear,
            anisoLevel = 4
        };
        _texture.Create();

        _material = new(shader);
        _material.SetTexture("_MainTex", _texture);

        _canvas = new("FisheyeCanvas");
        _canvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.AddComponent<CanvasScaler>();
        _canvas.AddComponent<GraphicRaycaster>();
        _canvas.AddComponent<RawImage>().material = _material;
    }

    public void Dispose()
    {
        GameObject.DestroyImmediate(_texture);
        GameObject.DestroyImmediate(_material);
        GameObject.DestroyImmediate(_canvas);
    }

    public void Render()
    {
        _material.SetMatrix("_CameraRotation", Matrix4x4.Rotate(_camera.transform.rotation));
        _material.SetFloat("_FOV", 180);
        _camera.RenderToCubemap(_texture, 63);
    }

    public void Resize(int resolution)
    {
        _texture.Release();
        _texture.width = resolution;
        _texture.height = resolution;
        _texture.Create();
    }
}