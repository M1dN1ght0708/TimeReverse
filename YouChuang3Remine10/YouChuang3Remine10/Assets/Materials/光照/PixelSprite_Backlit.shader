Shader "MyShader/PixelSprite_Backlit" {
    Properties{
        _Color("Main Color", Color) = (1,1,1,1)
        [PerRendererData]_MainTex("Sprite Texture", 2D) = "white" {}
        _Cutoff("Alpha Cutoff", Range(0,1)) = 0.5

            // ���տ���
            _LightSensitivity("Light Sensitivity", Range(0,3)) = 1.0
            _LightContrast("Light Contrast", Range(0.1,5)) = 2.0

            // �������Ե��
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
            fixed4 _Color;
            fixed _Cutoff, _LightSensitivity, _LightContrast;
            half _BacklightIntensity, _EdgeIntensity, _EdgeWidth;

            struct Input {
                float2 uv_MainTex;
                float3 viewDir;
                float3 worldNormal;
            };

            // �ؼ��޸ģ���Ե����ɫȡ�Գ�����Դ
            half4 LightingCustomLighting(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
                // ===== �������߼��� =====
                half3 normal = normalize(s.Normal);
                half NdotL = dot(normal, lightDir);

                // ===== ������� =====
                half backlightMask = saturate(-NdotL); // �������ܹ�ʱ����
                half3 backlight = _LightColor0.rgb      // ʹ�ù�Դ��ɫ
                               * backlightMask
                               * _BacklightIntensity;

                // ===== ��Ե����� =====
                half rim = 1 - saturate(dot(normalize(viewDir), normal));
                half edgeMask = smoothstep(0.5 - _EdgeWidth, 0.5 + _EdgeWidth, rim);
                half3 edgeLight = _LightColor0.rgb     // ʹ�ù�Դ��ɫ
                                * edgeMask
                                * _EdgeIntensity;

                // ===== ��������� =====
                half diffuse = pow(saturate(NdotL), _LightContrast);
                half3 finalDiffuse = diffuse * s.Albedo
                                   * _LightColor0.rgb
                                   * _LightSensitivity;

                // ===== ������ɫ�ϳ� =====
                half3 finalColor = (finalDiffuse + backlight + edgeLight) * atten;

                return half4(finalColor, s.Alpha);
            }

            void surf(Input IN, inout SurfaceOutput o) {
                // ������ͼ��ȷ����ͼFilter ModeΪPoint��
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = c.rgb;
                o.Alpha = c.a;
                clip(c.a - _Cutoff);

                // ���߳������������ǿ�������жȣ�
                o.Normal = normalize(IN.viewDir);
            }
            ENDCG
        }
            FallBack "Diffuse"
}