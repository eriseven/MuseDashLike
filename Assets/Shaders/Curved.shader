Shader "Unlit/Curved"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _What("what",float) = 200
		_YOffset("Y Offset", float) = 0
		_ZOffset("Z Offset", float) = 0
		_XCurveFact("X Curve", float) = 15 
		_YCurveFact("Y Curve", float) = 15 
    }
    SubShader
    {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma target 2.0
            #include "UnityCG.cginc"
            

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _What;
			float _YOffset;
			float _ZOffset;
			float _XCurveFact;
            
            v2f vert (appdata v)
            {
                v2f o;
                
				UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				float4 vPos = mul (UNITY_MATRIX_MV, v.vertex);
				float zOff = min (vPos.z + _ZOffset, 0);

				zOff = zOff/_What;


                // vPos -= float4(15 * sign(vPos.x), 15 * sign(vPos.y + 4), 0, 0)*zOff*zOff;
                
                vPos -= float4(_XCurveFact * sign(vPos.x), _YOffset*100*zOff*zOff, 0, 0)*zOff*zOff;
                
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
