Shader "MyShader/SpriteShadow_Highlight" {
    Properties{
        _Color("Color", Color) = (1,1,1,1)
        [PerRendererData]_MainTex("Sprite Texture", 2D) = "white" {}
        _Cutoff("Shadow alpha cutoff", Range(0,1)) = 0.5

        [Header(Highlight Settings)]
        _HighlightColor("Highlight Color", Color) = (1,1,1,1)
        _HighlightWidth("Highlight Width", Range(0, 10)) = 2.0
        _HighlightPower("Highlight Power", Range(1, 10)) = 3.0
        _HighlightThreshold("Highlight Threshold", Range(0, 0.5)) = 0.05
    }

        SubShader{
            Tags {
                "Queue" = "AlphaTest"
                "RenderType" = "TransparentCutout"
                "IgnoreProjector" = "True"
                "PreviewType" = "Plane"
            }
            LOD 200
            Cull Off

            CGPROGRAM
            #pragma surface surf Lambert addshadow fullforwardshadows
            #pragma multi_compile_fog
            #pragma target 3.0
            #pragma multi_compile_instancing

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 _Color;
            fixed _Cutoff;

            fixed4 _HighlightColor;
            float _HighlightWidth;
            float _HighlightPower;
            float _HighlightThreshold;

            struct Input {
                float2 uv_MainTex;
            };

            void surf(Input IN, inout SurfaceOutput o) {
                // Step 1: 基础采样和Alpha测试
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
                fixed baseAlpha = c.a * _Color.a; // 合并颜色Alpha通道
                clip(baseAlpha - _Cutoff); // 立即执行裁剪

                // Step 2: 计算基础颜色
                o.Albedo = c.rgb * _Color.rgb;
                o.Alpha = baseAlpha;

                // Step 3: 边缘检测（仅在可见区域执行）
                float2 texelSize = _MainTex_TexelSize.xy * _HighlightWidth;

                // 四方向采样（仅采样Alpha通道）
                float alphaUp = tex2D(_MainTex, IN.uv_MainTex + float2(0, 1) * texelSize).a;
                float alphaDown = tex2D(_MainTex, IN.uv_MainTex + float2(0, -1) * texelSize).a;
                float alphaLeft = tex2D(_MainTex, IN.uv_MainTex + float2(-1, 0) * texelSize).a;
                float alphaRight = tex2D(_MainTex, IN.uv_MainTex + float2(1, 0) * texelSize).a;

                // 边缘强度计算
                float edge = saturate(
                    abs(alphaUp - baseAlpha) +
                    abs(alphaDown - baseAlpha) +
                    abs(alphaLeft - baseAlpha) +
                    abs(alphaRight - baseAlpha)
                ) * _HighlightPower;

                edge = step(_HighlightThreshold, edge);
                float highlightMask = edge * (1 - baseAlpha);

                // Step 4: 应用高亮效果
                o.Emission = _HighlightColor.rgb * highlightMask;
                o.Albedo = lerp(o.Albedo, _HighlightColor.rgb, highlightMask * 0.3);
            }
            ENDCG
        }
            FallBack "Transparent/Cutout/Diffuse"
}