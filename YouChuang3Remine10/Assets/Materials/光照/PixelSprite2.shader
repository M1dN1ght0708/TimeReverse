Shader "MyShader/PixelSprite_Backlit" {
    Properties{
        _Color("Main Color", Color) = (1,1,1,1)
        [PerRendererData]_MainTex("Sprite Texture", 2D) = "white" {}
        _Cutoff("Alpha Cutoff", Range(0,1)) = 0.5

            // 新增法线贴图属性
            [Normal]_NormalMap("Normal Map", 2D) = "bump" {}
            _NormalStrength("Normal Strength", Range(0,3)) = 1

                // 光照控制
                _LightSensitivity("Light Sensitivity", Range(0,3)) = 1.0
                _LightContrast("Light Contrast", Range(0.1,5)) = 2.0

                // 背光与边缘光
                _BacklightIntensity("Backlight Intensity", Range(0,2)) = 0.3
                _EdgeIntensity("Edge Intensity", Range(0,2)) = 0.5
                _EdgeWidth("Edge Width", Range(0,0.5)) = 0.2
    }
        SubShader{
            Tags {
                "Queue" = "Geometry"
                "RenderType" = "TransparentCutout"
            }
            LOD 200
            Cull Off

            CGPROGRAM
            #pragma surface surf CustomLighting addshadow fullforwardshadows

            sampler2D _MainTex;
            sampler2D _NormalMap;  // 新增法线贴图采样器
            fixed4 _Color;
            fixed _Cutoff, _LightSensitivity, _LightContrast;
            half _BacklightIntensity, _EdgeIntensity, _EdgeWidth;
            float _NormalStrength;  // 法线强度控制

            struct Input {
                float2 uv_MainTex;
                float3 viewDir;
                float3 worldNormal;
                INTERNAL_DATA  // 需要添加这个以支持世界法线计算
            };

            half4 LightingCustomLighting(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
                // 使用法线贴图后的法线数据
                half3 normal = normalize(s.Normal);
                half NdotL = dot(normal, lightDir);

                // 背光计算
                half backlightMask = saturate(-NdotL);
                half3 backlight = _LightColor0.rgb * backlightMask * _BacklightIntensity;

                // 边缘光计算（使用修改后的法线）
                half rim = 1 - saturate(dot(normalize(viewDir), normal));
                half edgeMask = smoothstep(0.5 - _EdgeWidth, 0.5 + _EdgeWidth, rim);
                half3 edgeLight = _LightColor0.rgb * edgeMask * _EdgeIntensity;

                // 漫反射计算
                half diffuse = pow(saturate(NdotL), _LightContrast);
                half3 finalDiffuse = diffuse * s.Albedo * _LightColor0.rgb * _LightSensitivity;

                // 最终颜色合成
                half3 finalColor = (finalDiffuse + backlight + edgeLight) * atten;
                return half4(finalColor, s.Alpha);
            }

            void surf(Input IN, inout SurfaceOutput o) {
                // 主纹理采样
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = c.rgb;
                o.Alpha = c.a;
                clip(c.a - _Cutoff);

                // 法线贴图采样与处理
                fixed4 normalMap = tex2D(_NormalMap, IN.uv_MainTex);
                o.Normal = UnpackNormal(normalMap);  // 解包法线数据
                o.Normal.xy *= _NormalStrength;      // 调整法线强度
                o.Normal = normalize(o.Normal);      // 确保法线归一化
            }
            ENDCG
            }
                FallBack "Diffuse"
}