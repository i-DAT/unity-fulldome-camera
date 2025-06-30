using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

[ExecuteInEditMode]
public class FisheyeCam : MonoBehaviour
{
    public Shader shader;
    public int resolution = 512;
    public float fov = 180;

    new Camera camera;
    RenderTexture texture;
    Material material;
    GameObject canvas;

    void OnEnable()
    {
        // To enable realtime preview in editor, we need to manage resources when the script is enabled.

        // Grab the camera and disable its postprocessing - this ensures there are no visible seams 
        // on the cubemap from vignette.
        // TODO: this should specifically disable vignette to allow other postprocessing effects.
        camera = GetComponent<Camera>();
        gameObject.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = false;

        // Create a new cubemap texture at the desired resolution.
        texture = new(resolution, resolution, 24);
        texture.dimension = UnityEngine.Rendering.TextureDimension.Cube;
        texture.filterMode = FilterMode.Trilinear;
        texture.anisoLevel = 4;
        texture.Create();

        // Create a material from the shader and assign the cubemap texture to its _MainTex slot.
        material = new Material(shader);
        material.SetTexture("_MainTex", texture);

        if (GameObject.Find("FisheyeCanvas") is GameObject c)
        {
            canvas = c;
        }
        else
        {
            // If there is no canvas object yet, create it, and set it to display our material.
            canvas = new GameObject("FisheyeCanvas");
            canvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();
            canvas.AddComponent<RawImage>().material = material;
        }
    }

    void OnDisable()
    {
        DestroyImmediate(texture);
        DestroyImmediate(material);
        DestroyImmediate(canvas);
    }

    void LateUpdate()
    {
        // After each Update() frame, update the shader with the camera rotation and FOV, then
        // render to the cubemap texture on all faces (bitmap 63).
        // This is an expensive operation as the scene is rendered 6 times!
        // TODO: backface culling to reduce to 5?
        material.SetMatrix("_CameraRotation", Matrix4x4.Rotate(camera.transform.rotation));
        material.SetFloat("_FOV", fov);
        camera.RenderToCubemap(texture, 63);
    }

    void OnValidate()
    {
        // When the texture resolution value is updated in editor, resize the texture.
        // Unity doesn't really like this hack and sometimes throws errors, but it shouldn't
        // affect a built version of the game.
        if (texture != null && 0 < resolution && resolution <= 4096)
        {
            texture.Release();
            texture.width = resolution;
            texture.height = resolution;
            texture.Create();
        }
    }
}
