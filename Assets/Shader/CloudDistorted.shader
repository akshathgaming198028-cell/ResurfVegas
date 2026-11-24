Shader "Custom/Distorted/Transparent" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _MainColor ("Color (RGBC)", Color) = (1,1,1,0)
}
SubShader {
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  ZWrite Off
  Blend SrcAlpha OneMinusSrcAlpha
  // Port of the original GLES program: clip-space bend, out = plain texture (no tint, no vertex colour).
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  float4 _MainTex_ST;
  float4 _Distort; // global, as in the original

  struct appdata { float4 vertex : POSITION; float4 texcoord : TEXCOORD0; };
  struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

  v2f vert(appdata v) {
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.pos.x = o.pos.x + (o.pos.z * o.pos.z) * _Distort.x;
    o.pos.y = o.pos.y + (o.pos.z * o.pos.z) * _Distort.y;
    o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
    return o;
  }

  fixed4 frag(v2f i) : SV_Target { return tex2D(_MainTex, i.uv); }
  ENDCG
 }
}
}
