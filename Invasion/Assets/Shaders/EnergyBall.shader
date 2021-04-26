Shader "Unlit/EnergyBall"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (0, 0, 1, 1)
        _Speed("Speed", Vector) = (1, 1, 1, 1)
        _Scale("Scale", Vector) = (1, 1, 1, 1)
        _Distance("Distance", Vector) = (1, 1, 1, 1)
        _ShadowIntensity("Shadow Intensity", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _Color;
            float3 _Speed;
            float3 _Distance;
            float3 _Scale;

            float _ShadowIntensity;

            v2f vert (appdata v)
            {
                v2f o;

                v.vertex.x = v.vertex.x + sin(_Time.y * _Speed.x + (v.vertex.z + v.vertex.y) / _Scale.x) * _Distance.x;
                v.vertex.y = v.vertex.y + sin(_Time.y * _Speed.y + (v.vertex.x + v.vertex.z) / _Scale.y) * _Distance.y;
                v.vertex.z = v.vertex.z + sin(_Time.y * _Speed.z + (v.vertex.x + v.vertex.y) / _Scale.z) * _Distance.z;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float4 lightDirection = _WorldSpaceLightPos0;

                float lightAtten = saturate(dot(lightDirection.xyz, i.worldNormal));
                float voxelLightAtten = 1 + _ShadowIntensity * (lightAtten - 1);

                col *= voxelLightAtten;
                col *= _Color;
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
