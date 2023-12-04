Shader "Unlit/Rainbow"
{
    Properties
    {
        _Color ("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }
        LOD 100

        Cull Off
        Blend One OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            fixed4 _Color;

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
                o.uv = float2(0, v.vertex.y / 2);
                return o;
            }

            float3 hue2rgb(float hue) {
                hue = frac(hue); //only use fractional part of hue, making it loop
                float r = abs(hue * 6 - 3) - 1; //red
                float g = 2 - abs(hue * 6 - 2); //green
                float b = 2 - abs(hue * 6 - 4); //blue
                float3 rgb = float3(r,g,b); //combine components
                rgb = saturate(rgb); //clamp between 0 and 1
                return rgb;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 col = hue2rgb(i.uv.y);
	            return float4(col * _Color.a, _Color.a);
            }
            ENDCG
        }
    }
}
