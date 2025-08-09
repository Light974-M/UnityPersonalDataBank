using UnityEngine;

public class TextureBoundsComputer : MonoBehaviour
{
    public Material targetMaterial; // Material contenant ton Shader Graph

    public ComputeShader shader;
    public RenderTexture source; // ta render texture alpha
    private ComputeBuffer minMaxBuffer;
    private uint[] results = new uint[4];

    void Start()
    {
        // Init du buffer avec des valeurs extrêmes
        minMaxBuffer = new ComputeBuffer(4, sizeof(uint));
        ResetMinMax();
    }

    void ResetMinMax()
    {
        uint maxVal = 999999;
        uint[] init = new uint[] { maxVal, maxVal, 0, 0 };
        minMaxBuffer.SetData(init);
    }

    void Update()
    {
        ResetMinMax();

        int kernel = shader.FindKernel("CSMain");
        shader.SetTexture(kernel, "InputTexture", source);
        shader.SetBuffer(kernel, "MinMaxBuffer", minMaxBuffer);
        shader.SetInt("width", source.width);
        shader.SetInt("height", source.height);

        minMaxBuffer.SetData(new uint[] { uint.MaxValue, uint.MaxValue, 0, 0 });
        shader.Dispatch(kernel, Mathf.CeilToInt(source.width / 8f), Mathf.CeilToInt(source.height / 8f), 1);

        int groupsX = Mathf.CeilToInt(source.width / 8.0f);
        int groupsY = Mathf.CeilToInt(source.height / 8.0f);
        shader.Dispatch(kernel, groupsX, groupsY, 1);

        minMaxBuffer.GetData(results);

        targetMaterial.SetVector("_AlphaBounds", new Vector4(results[0], results[1], results[2], results[3]));
    }
}