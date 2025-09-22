Shader "Hidden/NormalMapGenerator"
{
    Properties
    {
        _MainTex ("Height Texture", 2D) = "white" { }
        _Strength ("Strength", Float) = 1.0
        _Resolution ("Resolution", Vector) = (1, 1, 0, 0)
        _NormalDirection ("Normal Direction", Float) = 1.0
        _Intensity ("Intensity", Float) = 0.75
        _Contrast ("Contrast", Float) = 1.5 // Nouveau paramètre
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

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

            sampler2D _MainTex;
            float _Strength;
            float4 _Resolution;
            float _NormalDirection;
            float _Intensity;
            float _Contrast;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float2 texelSize = 1.0 / _Resolution.xy;

    // Échantillonnage des hauteurs autour du pixel courant
    float hL = tex2D(_MainTex, i.uv + float2(-texelSize.x, 0)).r; // gauche
    float hR = tex2D(_MainTex, i.uv + float2(texelSize.x, 0)).r;  // droite
    float hD = tex2D(_MainTex, i.uv + float2(0, -texelSize.y)).r; // bas
    float hU = tex2D(_MainTex, i.uv + float2(0, texelSize.y)).r;  // haut

    // Gradient
    float dx = hR - hL;
    float dy = hU - hD;

    // Contraste sur les gradients
    dx = (dx) * _Contrast;
    dy = (dy) * _Contrast;

    // Calcul de la normale
    float3 normal = normalize(float3(dx, dy, 1.0 / _Intensity)); // Z = "hauteur", inversée selon _Intensity

    // Application de la direction des normales
    normal *= _NormalDirection;

    // Conversion en [0, 1]
    normal = normal * 0.5 + 0.5;

    return half4(normal, 1.0);
            }
            ENDCG
        }
    }
}