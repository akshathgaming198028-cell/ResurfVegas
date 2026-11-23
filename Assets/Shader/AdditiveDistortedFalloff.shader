Shader "Custom/Distorted/Additive (falloff)" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _MainColor ("Color (RGBC)", Color) = (1,1,1,0)
 _Falloff ("Falloff Distance", Float) = 200
}
SubShader {
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  ZWrite Off
  Blend One One
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  // Global set by ThemeManager (same name as the original GLES program).
  float4 _Distort;

  sampler2D _MainTex;
  float4 _MainTex_ST;
  float4 _MainColor;
  float _Falloff;

  struct appdata { float4 vertex : POSITION; float2 texcoord : TEXCOORD0; };
  struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; fixed alpha : TEXCOORD1; };

  // Line-by-line port of the original Custom/Distorted/Additive (falloff) GLES program:
  // alpha = 1 - (z / Falloff)^2 taken from the clip-space z before the bend (the bend only moves x and y).
  v2f vert(appdata v) {
    v2f o;
    float4 clip = UnityObjectToClipPos(v.vertex);
    float normalizedDistance = clip.z / _Falloff;
    clip.x = clip.x + (clip.z * clip.z) * _Distort.x;
    clip.y = clip.y + (clip.z * clip.z) * _Distort.y;
    o.pos = clip;
    o.uv = v.texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
    o.alpha = 1.0 - normalizedDistance * normalizedDistance;
    return o;
  }
  fixed4 frag(v2f i) : SV_Target {
    return tex2D(_MainTex, i.uv) * _MainColor * i.alpha;
  }
  ENDCG
 }
}
}
