﻿Shader "Unlit/Curved"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _What("what",float) = 200
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
            float4 _MainTex_ST;

            float _What;
            
            v2f vert (appdata v)
            {
                v2f o;
                
               float4 vPos = mul (UNITY_MATRIX_MV, v.vertex);
               float zOff = vPos.z/_What;
                vPos += float4(15,0,0,0)*zOff*zOff;
                
                // o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertex = mul (UNITY_MATRIX_P, vPos);//vPos;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
