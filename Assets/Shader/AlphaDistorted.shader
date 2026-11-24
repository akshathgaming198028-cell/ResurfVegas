Shader "Custom/Distorted/Transparent" {
Properties {
 _MainTex ("Base (RGB), Alpha (A)", 2D) = "white" { }
}
SubShader {
 Tags { "Queue"="Transparent" }
 LOD 100
 Pass {
  ZWrite Off
  Blend SrcAlpha OneMinusSrcAlpha

  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  float4 _MainTex_ST;
  // Global, set by ThemeManager.SetShaderStates (not a material property, as in the original).
  float4 _Distort;

  struct appdata {
    float4 vertex : POSITION;
    float2 texcoord : TEXCOORD0;
  };

  struct v2f {
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
  };

  v2f vert(appdata v) {
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    // Clip-space curve, identical to the original (tmpvar_1.x += z*z*_Distort.x).
    o.pos.x = o.pos.x + (o.pos.z * o.pos.z) * _Distort.x;
    o.pos.y = o.pos.y + (o.pos.z * o.pos.z) * _Distort.y;
    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
    return o;
  }

  fixed4 frag(v2f i) : SV_Target {
    return tex2D(_MainTex, i.uv);
  }
  ENDCG
 }
}
}
