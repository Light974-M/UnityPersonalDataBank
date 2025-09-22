Shader "Hidden/SmoothnessMapGenerator"
{
    Properties
    {
        _MainTex ("Source Texture", 2D) = "white" {}
        _Contrast ("Contrast", Float) = 1.0
        _Brightness ("Brightness", Float) = 1.0
        _Invert ("Invert", Range(0, 1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Contrast;
            float _Brightness;
            float _Invert;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);

                // Convert to grayscale using perceptual luminance
                float gray = dot(col.rgb, float3(0.2126, 0.7152, 0.0722));

                // Apply contrast and brightness
                gray = pow(gray, 1.0 / max(_Contrast, 0.0001));
                gray = saturate(gray * _Brightness);

                // Optional inversion
                gray = lerp(gray, 1.0 - gray, _Invert);

                // Output smoothness as grayscale
                return half4(gray, gray, gray, 1.0);
            }
            ENDCG
        }
    }
}