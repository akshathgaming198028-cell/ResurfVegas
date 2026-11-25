// Custom/Distorted/FlyingCarpet
// Line-by-line port of the 1.56 original's GLES program (sharedassets1, path_id 374),
// decompressed with LZ4 (decompressedSize 1728), translated to CG so it uses the same
// built-ins (UNITY_MATRIX_MVP, _Time) as the working shaders in this project.
// Properties match the original ShaderLab (_MainTex, _Color, _Frequency); _Distort is a
// global-style uniform, as in the original (not declared in Properties).
Shader "Custom/Distorted/FlyingCarpet" {
Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _Color ("Color (RGBA)", Color) = (1,1,1,1)
    _Frequency ("Frequency", Range(0.01,5)) = 4
}
SubShader {
    Pass {
        Cull Off

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"

        sampler2D _MainTex;
        float4 _MainTex_ST;
        fixed4 _Color;
        float _Frequency;
        float2 _Distort;

        struct appdata {
            float4 vertex : POSITION;
            fixed4 color : COLOR;
            float4 texcoord : TEXCOORD0;
        };

        struct v2f {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
        };

        v2f vert (appdata v)
        {
            v2f o;
            float4 tmpvar_1 = v.vertex;
            fixed4 tmpvar_2 = v.color;
            float4 tmpvar_3;
            tmpvar_3.w = tmpvar_1.w;
            float intensity_5 = tmpvar_2.x;
            float3 off_6;
            float3 tmpvar_7;
            tmpvar_7.z = 0.0;
            tmpvar_7.xy = tmpvar_1.xz;
            float3 tmpvar_8 = abs(((frac((((_Time.x * float3(50.0, 100.0, 0.0)) * _Frequency) - dot(tmpvar_7, float3(1.0, 1.0, 1.0))) / 9.42477) + 0.5)) * 2.0) - 1.0);
            off_6.x = 0.0;
            off_6.y = ((intensity_5 * (((tmpvar_8 * tmpvar_8) * (3.0 - (2.0 * tmpvar_8))) * 2.0) - 1.0).y * 0.3);
            off_6.z = 0.0;
            tmpvar_3.xyz = (v.vertex.xyz + off_6);
            float4 tmpvar_4 = mul(UNITY_MATRIX_MVP, tmpvar_3);
            tmpvar_4.x = (tmpvar_4.x + ((tmpvar_4.z * tmpvar_4.z) * _Distort.x));
            tmpvar_4.y = (tmpvar_4.y + ((tmpvar_4.z * tmpvar_4.z) * _Distort.y));
            o.pos = tmpvar_4;
            o.uv = (v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
            return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
            return tex2D(_MainTex, i.uv) * _Color;
        }
        ENDCG
    }
}
}
