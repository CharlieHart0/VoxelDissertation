// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Voxel/2DArrayTexture"
{
    Properties
    {
        _MyArr("Texture", 2DArray) = "" {}
        _SliceRange("Slices", Range(0,16)) = 0
        _UVScale("UV Scale", Float) = 1.0
    }
        SubShader
        {
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                // texture arrays are not available everywhere,
                // only compile shader on platforms where they are
                #pragma require 2darray

                #include "UnityCG.cginc"

                

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

                struct v2f
                {
                    float3 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                

                float _SliceRange;
                float _UVScale;

                v2f vert(appdata app)
                {
                    v2f OUT;
                    OUT.vertex = UnityObjectToClipPos(app.vertex);
                    OUT.uv.xy = (app.uv.xy) * _UVScale;
                    OUT.uv.z =  _SliceRange;
                    return OUT;
                }

                UNITY_DECLARE_TEX2DARRAY(_MyArr);

                half4 frag(v2f IN) : SV_Target
                {
                    return UNITY_SAMPLE_TEX2DARRAY(_MyArr, IN.uv);
                }
                ENDCG
            }
        }
}