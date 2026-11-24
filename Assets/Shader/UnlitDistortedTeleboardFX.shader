Shader "Custom/Distorted/Unlit TeleboardFX" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _Scale ("Scale", Range(0,1)) = 1
 _Offset ("Offset", Range(0,1000)) = 0
 _MainColor ("Color (RGBC)", Color) = (1,1,1,0)
}
SubShader {
 Tags { "QUEUE"="Transparent" }
 // Pass 1: depth only (ColorMask 0), as the original.
 Pass {
  Tags { "QUEUE"="Transparent" }
  ColorMask 0
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  float4 _Distort; // global, as in the original

  struct appdata { float4 vertex : POSITION; };
  struct v2f { float4 pos : SV_POSITION; };

  v2f vert(appdata v) {
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.pos.x = o.pos.x + (o.pos.z * o.pos.z) * _Distort.x;
    o.pos.y = o.pos.y + (o.pos.z * o.pos.z) * _Distort.y;
    return o;
  }

  fixed4 frag(v2f i) : SV_Target { return fixed4(1, 1, 1, 1); }
  ENDCG
 }
 // Pass 2: additive screen-space glow. UV is the unnormalised screen position (as the original),
 // scaled by _Scale with _Offset added before scaling in Y.
 Pass {
  Tags { "QUEUE"="Transparent" }
  Blend One One
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  float4 _MainTex_ST;
  float _Scale;
  float _Offset;
  fixed4 _MainColor;
  float4 _Distort;

  struct appdata { float4 vertex : POSITION; };
  struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

  v2f vert(appdata v) {
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.pos.x = o.pos.x + (o.pos.z * o.pos.z) * _Distort.x;
    o.pos.y = o.pos.y + (o.pos.z * o.pos.z) * _Distort.y;
    float4 sp = ComputeScreenPos(o.pos);
    o.uv = float2(sp.x, (sp.y + _Offset) * _Scale);
    return o;
  }

  fixed4 frag(v2f i) : SV_Target {
    return tex2D(_MainTex, i.uv) * _MainColor;
  }
  ENDCG
 }
}
}
