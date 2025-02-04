using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ShadowCtrl : ScriptableRendererFeature
{
    class VignetteRenderPass : ScriptableRenderPass
    {
        public Material vignetteMaterial = null;
        // 使用 int 类型的临时 RT ID 替代 RenderTargetHandle
        private int temporaryRTId;

        public VignetteRenderPass()
        {
            temporaryRTId = Shader.PropertyToID("_TemporaryColorTexture");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (vignetteMaterial == null)
                return;

            // 获取摄像机的目标 RenderTarget
            RenderTargetIdentifier cameraColorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;
            // 获取摄像机目标的描述信息
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;

            CommandBuffer cmd = CommandBufferPool.Get("VignetteEffect");

            // 分配临时 RT（使用 int 临时 RT ID）
            cmd.GetTemporaryRT(temporaryRTId, opaqueDesc, FilterMode.Bilinear);
            // 将摄像机渲染结果拷贝到临时 RT
            cmd.Blit(cameraColorTarget, temporaryRTId);
            // 使用自定义材质对临时 RT 应用效果，然后 Blit 回摄像机目标
            cmd.Blit(temporaryRTId, cameraColorTarget, vignetteMaterial);
            // 释放临时 RT
            cmd.ReleaseTemporaryRT(temporaryRTId);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    VignetteRenderPass m_ScriptablePass;
    public Material vignetteMaterial;

    public override void Create()
    {
        m_ScriptablePass = new VignetteRenderPass();
        m_ScriptablePass.vignetteMaterial = vignetteMaterial;
        // 选择合适的渲染时机，例如在所有后处理之后执行
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // 将自定义 Pass 入队
        renderer.EnqueuePass(m_ScriptablePass);
    }
}
