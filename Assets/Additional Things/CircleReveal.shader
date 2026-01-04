Shader "UI/CircleRevealUV_Aspect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CenterUV ("CenterUV", Vector) = (0.5, 0.5, 0, 0)
        _Radius ("Radius", Float) = 0.1
        _Softness ("Softness", Float) = 0.02
        _ScreenAspect ("ScreenAspect", Float) = 1.0
    }

    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float2 _CenterUV;
            float _Radius;
            float _Softness;
            float _ScreenAspect;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 offset = i.uv - _CenterUV;
                offset.x *= _ScreenAspect; // врахування співвідношення сторін

                float dist = length(offset);
                float alpha = smoothstep(_Radius, _Radius + _Softness, dist);
                return fixed4(0, 0, 0, alpha);
            }
            ENDCG
        }
    }
}
