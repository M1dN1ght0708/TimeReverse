Shader "MyShader/PixelSprite_Backlit" {
    Properties{
        _Color("Main Color", Color) = (1,1,1,1)
        [PerRendererData]_MainTex("Sprite Texture", 2D) = "white" {}
        _Cutoff("Alpha Cutoff", Range(0,1)) = 0.5

            // ����������ͼ����
            [Normal]_NormalMap("Normal Map", 2D) = "bump" {}
            _NormalStrength("Normal Strength", Range(0,3)) = 1

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
            sampler2D _NormalMap;  // ����������ͼ������
            fixed4 _Color;
            fixed _Cutoff, _LightSensitivity, _LightContrast;
            half _BacklightIntensity, _EdgeIntensity, _EdgeWidth;
            float _NormalStrength;  // ����ǿ�ȿ���

            struct Input {
                float2 uv_MainTex;
                float3 viewDir;
                float3 worldNormal;
                INTERNAL_DATA  // ��Ҫ��������֧�����編�߼���
            };

            half4 LightingCustomLighting(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
                // ʹ�÷�����ͼ��ķ�������
                half3 normal = normalize(s.Normal);
                half NdotL = dot(normal, lightDir);

                // �������
                half backlightMask = saturate(-NdotL);
                half3 backlight = _LightColor0.rgb * backlightMask * _BacklightIntensity;

                // ��Ե����㣨ʹ���޸ĺ�ķ��ߣ�
                half rim = 1 - saturate(dot(normalize(viewDir), normal));
                half edgeMask = smoothstep(0.5 - _EdgeWidth, 0.5 + _EdgeWidth, rim);
                half3 edgeLight = _LightColor0.rgb * edgeMask * _EdgeIntensity;

                // ���������
                half diffuse = pow(saturate(NdotL), _LightContrast);
                half3 finalDiffuse = diffuse * s.Albedo * _LightColor0.rgb * _LightSensitivity;

                // ������ɫ�ϳ�
                half3 finalColor = (finalDiffuse + backlight + edgeLight) * atten;
                return half4(finalColor, s.Alpha);
            }

            void surf(Input IN, inout SurfaceOutput o) {
                // ���������
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = c.rgb;
                o.Alpha = c.a;
                clip(c.a - _Cutoff);

                // ������ͼ�����봦��
                fixed4 normalMap = tex2D(_NormalMap, IN.uv_MainTex);
                o.Normal = UnpackNormal(normalMap);  // �����������
                o.Normal.xy *= _NormalStrength;      // ��������ǿ��
                o.Normal = normalize(o.Normal);      // ȷ�����߹�һ��
            }
            ENDCG
            }
                FallBack "Diffuse"
}