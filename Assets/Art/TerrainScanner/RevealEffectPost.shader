Shader "PostFX/RevealEffectPost"
{
    Properties
    {
        [HideInInspector]
        _MainTex ("Texture", 2D) = "white" {}

        _RevealOrigin("Reveal Origin in World Space", Vector) = (0,0,0)
        _Radius("Radius Of Detection Region, Change in Scripts to animate Aura", Range(0,100)) = 0.5

        _OverlayColor("Overlay Colour", Color) = (1,1,1,1)
        _OverlayEmission("Overlay Emission", Range(0,5)) = 1
        _Power("Overlay Attenuation", Range(0,20)) = 10

        [Space(20)]
        [Toggle(EDGEDISPLACEMENT)]
        _EdgeDisplacement("Use Edge Displacement", Float) = 0
        _RadiusDispl("Displacement", Range(0,0.5)) = 0.2
        _EdgeTex("Edge Displacement Texture", 2D) = "white" {}

        [Space(20)]
        [Toggle(REVEALFLAG)]
        _Reveal("Use Revealed Area texture", float) = 0
        _RevealTex("Reveal Texture", 2D) = "white" {}

        [Space(20)]
        [Toggle(CONCENTRIC)]
        _ConcentricLines("Use Concentric Lines", Float) = 0
        _LineWidth("Width of Each Line", Range(0,1)) = 0.1
        _LineDisplacement("Space between each Line", Range(0,10)) = 2.5
        _MaxLineCount("Max Lines Drawn", Float) = 10
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature REVEALFLAG
            #pragma shader_feature CONCENTRIC
            #pragma shader_feature EDGEDISPLACEMENT

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldDirection : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float2 uvEdge : TEXCOORD2;
                float2 uvReveal :TEXCOORD3;
            };

            float4x4 _ViewProjectInverse;

            sampler2D _MainTex;
            sampler2D _RevealTex;
            float4 _RevealTex_ST;
            sampler2D _EdgeTex;
            float4 _EdgeTex_ST;

            float3 _RevealOrigin;

            fixed4 _OverlayColor;
            half _OverlayEmission;

            float _Radius;
            float _RadiusDispl;
            float _EdgeWidth;

            float _LineWidth;
            float _LineDisplacement;

            float _Power;
            float _MaxLineCount;

            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uvEdge = TRANSFORM_TEX(v.uv, _EdgeTex);
                o.uvReveal = TRANSFORM_TEX(v.uv, _RevealTex);

                //float4 D = mul(unity_CameraInvProjection, float4((v.uv.x) * 2 - 1, (v.uv.y) * 2 - 1, 0.5, 1));
                float4 D = mul(_ViewProjectInverse, float4((v.uv.x) * 2 - 1, (v.uv.y) * 2 - 1, 0.5, 1));
                D.xyz /= D.w;
                D.xyz -= _WorldSpaceCameraPos;
                //float4 D0 = mul(unity_CameraInvProjection, float4(0, 0, 0.5, 1));
                float4 D0 = mul(_ViewProjectInverse, float4(0, 0, 0.5, 1));
                D0.xyz /= D0.w;
                D0.xyz -= _WorldSpaceCameraPos;
                o.worldDirection = D.xyz / length(D0.xyz);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);

                // get linear depth from the depth
                float sceneZ = LinearEyeDepth(depth);

                float3 worldPos = i.worldDirection;

                // calculate the world position
                // multiply the view plane by the linear depth to get the camera relative world space position
                // add the world space camera position to get the world space position from the depth texture
                worldPos = worldPos * sceneZ + _WorldSpaceCameraPos;
                //worldPos = mul(unity_CameraToWorld, float4(worldPos, 1.0));

                float displ = 0;
                #ifdef EDGEDISPLACEMENT
                displ = pow(tex2D(_EdgeTex, i.uvEdge + _Time.y * 0.1), 0.1);
                #endif

                float flag = distance(_RevealOrigin, worldPos) < (_Radius + _RadiusDispl * displ);
                float ratio = saturate(distance(_RevealOrigin, worldPos) / (_Radius));

                fixed4 interiorCol = flag * _OverlayColor;
                fixed4 edgeCol = (flag)*_OverlayColor;

                #ifdef REVEALFLAG
                fixed4 noise = tex2D(_RevealTex, i.uvReveal + _Time.y * 5);
                interiorCol *= noise;
                #endif

                float flagConcentric = 0;
                #ifdef CONCENTRIC

                float count = (int)_Radius / _LineDisplacement;
                for (int j = 1; j < min(count + 1, (int)_MaxLineCount); j++)
                {
                    float dist = distance(_RevealOrigin, worldPos);
                    flagConcentric += (dist > (j * _LineDisplacement + _RadiusDispl * displ)) *
                        (dist < (j* _LineDisplacement + _LineWidth + _RadiusDispl * displ));
                }

                #endif

                float isNotZero = _Radius > 0; 
                fixed4 colorMod = isNotZero * (interiorCol * _OverlayEmission / 10 + edgeCol * lerp(0, _OverlayEmission, pow(ratio, _Power)) + edgeCol * _OverlayEmission * flagConcentric * flag);

                fixed4 c = tex2D (_MainTex, i.uv);
                c += colorMod;

                return c;
            }
            ENDCG
        }
    }
}
