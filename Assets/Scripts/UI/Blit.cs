using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

namespace RatGamesStudios.OperationDeratization.UI
{
    public class Blit : ScriptableRendererFeature
    {
        public class BlitPass : ScriptableRenderPass
        {
            public Material blitMaterial = null;
            public FilterMode filterMode { get; set; }

            private BlitSettings settings;
            private RTHandle source;
            private RTHandle destination;
            private RTHandle m_TemporaryColorTexture;
            private string m_ProfilerTag;

            private class PassData
            {
                public TextureHandle source;
                public TextureHandle destination;
                public Material material;
                public int passIndex;
            }

            public BlitPass(RenderPassEvent renderPassEvent, BlitSettings settings, string tag)
            {
                this.renderPassEvent = renderPassEvent;
                this.settings = settings;
                blitMaterial = settings.blitMaterial;
                m_ProfilerTag = tag;
                this.requiresIntermediateTexture = true;
            }

            [System.Obsolete]
            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
            {
#pragma warning disable 0618
                if (settings.requireDepthNormals)
                    ConfigureInput(ScriptableRenderPassInput.Normal);

                var renderer = renderingData.cameraData.renderer;
                source = renderer.cameraColorTargetHandle;
#pragma warning restore 0618
            }

            [System.Obsolete]
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
#pragma warning disable 0618 
                if (blitMaterial == null) return;

                CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
                RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
                opaqueDesc.depthBufferBits = 0;

                if (settings.srcType == Target.TextureID)
                {
                    RenderingUtils.ReAllocateHandleIfNeeded(ref source, opaqueDesc, name: settings.srcTextureId);
                }

                if (settings.setInverseViewMatrix)
                    Shader.SetGlobalMatrix("_InverseView", renderingData.cameraData.camera.cameraToWorldMatrix);

                if (settings.dstType == Target.TextureID)
                {
                    if (settings.overrideGraphicsFormat)
                        opaqueDesc.graphicsFormat = settings.graphicsFormat;

                    RenderingUtils.ReAllocateHandleIfNeeded(ref destination, opaqueDesc, filterMode, TextureWrapMode.Clamp, name: settings.dstTextureId);
                    Blitter.BlitCameraTexture(cmd, source, destination, blitMaterial, settings.blitMaterialPassIndex);
                }
                else if (source == destination || (settings.srcType == settings.dstType && settings.srcType == Target.CameraColor))
                {
                    RenderingUtils.ReAllocateHandleIfNeeded(ref m_TemporaryColorTexture, opaqueDesc, filterMode, TextureWrapMode.Clamp, name: "_TemporaryColorTexture");
                    Blitter.BlitCameraTexture(cmd, source, m_TemporaryColorTexture, blitMaterial, settings.blitMaterialPassIndex);
                    Blitter.BlitCameraTexture(cmd, m_TemporaryColorTexture, source);
                }
                else
                {
                    Blitter.BlitCameraTexture(cmd, source, source, blitMaterial, settings.blitMaterialPassIndex);
                }

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
#pragma warning restore 0618
            }

            public override void OnCameraCleanup(CommandBuffer cmd)
            {
                source = null;
                destination = null;
            }

            public void Dispose()
            {
                m_TemporaryColorTexture?.Release();
                destination?.Release();
            }


            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                if (blitMaterial == null) return;

                var resourceData = frameData.Get<UniversalResourceData>();
                var cameraData = frameData.Get<UniversalCameraData>();

                if (settings.setInverseViewMatrix)
                    Shader.SetGlobalMatrix("_InverseView", cameraData.camera.cameraToWorldMatrix);

                TextureHandle srcHandle;

                if (settings.srcType == Target.CameraColor)
                {
                    srcHandle = resourceData.activeColorTexture;
                }
                else
                {
                    var desc = GetCompatibleDescriptor(cameraData.cameraTargetDescriptor, cameraData.cameraTargetDescriptor.graphicsFormat);
                    desc.name = settings.srcTextureId;
                    srcHandle = renderGraph.CreateTexture(desc);
                }

                if (settings.dstType == Target.CameraColor && settings.srcType == Target.CameraColor)
                {
                    var tempDesc = GetCompatibleDescriptor(cameraData.cameraTargetDescriptor, cameraData.cameraTargetDescriptor.graphicsFormat);
                    tempDesc.name = "_BlitTempTexture";
                    TextureHandle tempHandle = renderGraph.CreateTexture(tempDesc);

                    using (var builder = renderGraph.AddRasterRenderPass<PassData>(m_ProfilerTag + "_Apply", out var passData))
                    {
                        passData.source = srcHandle;
                        passData.destination = tempHandle;
                        passData.material = blitMaterial;
                        passData.passIndex = settings.blitMaterialPassIndex;

                        builder.UseTexture(passData.source, AccessFlags.Read);
                        builder.SetRenderAttachment(passData.destination, 0, AccessFlags.Write);

                        builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                        {
                            data.material.SetTexture("_MainTex", data.source);
                            Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), data.material, data.passIndex);
                        });
                    }

                    using (var builder = renderGraph.AddRasterRenderPass<PassData>(m_ProfilerTag + "_CopyBack", out var passData))
                    {
                        passData.source = tempHandle;
                        passData.destination = srcHandle;

                        builder.UseTexture(passData.source, AccessFlags.Read);
                        builder.SetRenderAttachment(passData.destination, 0, AccessFlags.Write);

                        builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                        {
                            Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), 0.0f, false);
                        });
                    }
                }
                else if (settings.dstType == Target.TextureID)
                {
                    var targetFormat = settings.overrideGraphicsFormat ? settings.graphicsFormat : cameraData.cameraTargetDescriptor.graphicsFormat;
                    var destDesc = GetCompatibleDescriptor(cameraData.cameraTargetDescriptor, targetFormat);
                    destDesc.name = settings.dstTextureId;

                    TextureHandle destHandle = renderGraph.CreateTexture(destDesc);

                    using (var builder = renderGraph.AddRasterRenderPass<PassData>(m_ProfilerTag + "_ToCustom", out var passData))
                    {
                        passData.source = srcHandle;
                        passData.destination = destHandle;
                        passData.material = blitMaterial;
                        passData.passIndex = settings.blitMaterialPassIndex;

                        builder.UseTexture(passData.source, AccessFlags.Read);
                        builder.SetRenderAttachment(passData.destination, 0, AccessFlags.Write);

                        builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                        {
                            data.material.SetTexture("_MainTex", data.source);
                            Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), data.material, data.passIndex);
                        });
                    }
                }
            }

            private TextureDesc GetCompatibleDescriptor(RenderTextureDescriptor desc, UnityEngine.Experimental.Rendering.GraphicsFormat format)
            {
                var textureDesc = new TextureDesc(desc.width, desc.height);
                textureDesc.colorFormat = format;
                textureDesc.depthBufferBits = 0;
                textureDesc.msaaSamples = MSAASamples.None;
                textureDesc.useMipMap = false;
                return textureDesc;
            }
        }

        [System.Serializable]
        public class BlitSettings
        {
            public RenderPassEvent Event = RenderPassEvent.AfterRenderingTransparents;

            public Material blitMaterial = null;
            public int blitMaterialPassIndex = 0;
            public bool setInverseViewMatrix = false;
            public bool requireDepthNormals = false;

            public Target srcType = Target.CameraColor;
            public string srcTextureId = "_CameraColorTexture";

            public Target dstType = Target.CameraColor;
            public string dstTextureId = "_BlitPassTexture";

            public bool overrideGraphicsFormat = false;
            public UnityEngine.Experimental.Rendering.GraphicsFormat graphicsFormat;
        }

        public enum Target
        {
            CameraColor,
            TextureID
        }

        public BlitSettings settings = new BlitSettings();
        public BlitPass blitPass;

        public override void Create()
        {
            var passIndex = settings.blitMaterial != null ? settings.blitMaterial.passCount - 1 : 1;
            settings.blitMaterialPassIndex = Mathf.Clamp(settings.blitMaterialPassIndex, -1, passIndex);

            blitPass = new BlitPass(settings.Event, settings, name);

            if (settings.graphicsFormat == UnityEngine.Experimental.Rendering.GraphicsFormat.None)
                settings.graphicsFormat = SystemInfo.GetGraphicsFormat(UnityEngine.Experimental.Rendering.DefaultFormat.LDR);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (settings.blitMaterial == null)
            {
                return;
            }

            renderer.EnqueuePass(blitPass);
        }

        protected override void Dispose(bool disposing)
        {
            blitPass?.Dispose();
        }
    }
}