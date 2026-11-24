Shader "Custom/Distorted/Additive (fog falloff)" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _MainColor ("Color (RGBC)", Color) = (1,1,1,0)
}
SubShader {
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  ZWrite Off
  Blend One One
  // Port of the original GLES program: clip-space bend, then a distance fade from view depth:
  // t = clamp((-viewZ - 300) / 500, 0, 1); alpha = 1 - t*t; out = tex * _MainColor * alpha.
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  float4 _MainTex_ST;
  fixed4 _MainColor;
  float4 _Distort; // global, as in the original

  struct appdata { float4 vertex : POSITION; float4 texcoord : TEXCOORD0; };
  struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; fixed alpha : TEXCOORD1; };

  v2f vert(appdata v) {
    v2f o;
    float viewZ = UnityObjectToViewPos(v.vertex).z;
    float t = saturate((-viewZ - 300.0) / 500.0);
    o.alpha = 1.0 - t * t;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.pos.x = o.pos.x + (o.pos.z * o.pos.z) * _Distort.x;
    o.pos.y = o.pos.y + (o.pos.z * o.pos.z) * _Distort.y;
    o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
    return o;
  }

  fixed4 frag(v2f i) : SV_Target {
    return tex2D(_MainTex, i.uv) * _MainColor * i.alpha;
  }
  ENDCG
 }
}
}
