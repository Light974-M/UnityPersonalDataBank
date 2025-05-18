using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace URPGrabPass.Runtime
{
    /// <summary>
    ///     Pass that grabs the color texture of the camera.
    /// </summary>
    public class GrabColorTexturePass : ScriptableRenderPass
    {
        private readonly RTHandle _grabbedTextureHandle;
        private readonly string _grabbedTextureName;
        private readonly int _grabbedTexturePropertyId;

        private ScriptableRenderer _renderer;

        public GrabColorTexturePass(GrabTiming timing, string grabbedTextureName)
        {
            renderPassEvent = timing.ToRenderPassEvent();
            _grabbedTextureName = grabbedTextureName;
            _grabbedTexturePropertyId = Shader.PropertyToID(_grabbedTextureName);

            // Allocation du RTHandle avec une taille dynamique basée sur la caméra
            _grabbedTextureHandle = RTHandles.Alloc(
                name: _grabbedTextureName,
                scaleFactor: Vector2.one,
                dimension: TextureDimension.Tex2D,
                useDynamicScale: true
            );
        }

        public void BeforeEnqueue(ScriptableRenderer renderer)
        {
            _renderer = renderer;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            ConfigureTarget(_grabbedTextureHandle);
            ConfigureClear(ClearFlag.None, Color.clear);

            // Définir la texture globale pour les shaders
            Shader.SetGlobalTexture(_grabbedTexturePropertyId, _grabbedTextureHandle);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get(nameof(GrabColorTexturePass));

            // Blit moderne avec RTHandles
            RTHandle source = renderingData.cameraData.renderer.cameraColorTargetHandle;
            Blitter.BlitCameraTexture(cmd, source, _grabbedTextureHandle);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            // Ne rien faire ici : RTHandles doivent être libérés manuellement dans la classe appelante (ex: renderer feature)
        }

        public void Dispose()
        {
            RTHandles.Release(_grabbedTextureHandle);
        }
    }
}