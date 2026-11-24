Shader "Custom/Distorted/Unlit with overlay and Rim" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _Color ("Color Tint", Color) = (0,0,0,0)
}
SubShader {
 Pass {
  // Port of the original GLES program (no render-state tags: default Geometry / ZWrite On / Cull Back).
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  float4 _MainTex_ST;
  fixed4 _Color;
  // _Distort is global (not in Properties), as in the original.
  float4 _Distort;

  struct appdata { float4 vertex : POSITION; float3 normal : NORMAL; float4 texcoord : TEXCOORD0; };
  struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD2; float3 nv : TEXCOORD0; float3 vv : TEXCOORD1; };

  v2f vert(appdata v) {
    v2f o;
    o.vv = mul(UNITY_MATRIX_MV, v.vertex).xyz;
    o.nv = mul(UNITY_MATRIX_IT_MV, float4(v.normal, 0)).xyz;

    o.pos = UnityObjectToClipPos(v.vertex);
    o.pos.x = o.pos.x + (o.pos.z * o.pos.z) * _Distort.x;
    o.pos.y = o.pos.y + (o.pos.z * o.pos.z) * _Distort.y;

    o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
    return o;
  }

  fixed4 frag(v2f i) : SV_Target {
    fixed4 t = tex2D(_MainTex, i.uv);
    float rim = 1.5 * pow(1.0 - saturate(-dot(normalize(i.vv), normalize(i.nv))), 2.75);
    float edge = clamp(0.33 * ((t.x + t.z) + t.y), 0.0, 0.33);
    return t * _Color + rim * edge;
  }
  ENDCG
 }
}
}
