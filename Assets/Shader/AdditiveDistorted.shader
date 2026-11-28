Shader "Custom/Distorted/Additive" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _MainColor ("Color (RGBC)", Color) = (1,1,1,0)
}
SubShader {
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
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

  struct appdata { float4 vertex : POSITION; float2 texcoord : TEXCOORD0; };
  struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

  // Line-by-line port of the original Custom/Distorted/Additive GLES program:
  // clip-space bend x,y += z*z*_Distort; uv = texcoord*ST.xy + ST.zw.
  v2f vert(appdata v) {
    v2f o;
    float4 clip = UnityObjectToClipPos(v.vertex);
    clip.x = clip.x + (clip.z * clip.z) * _Distort.x;
    clip.y = clip.y + (clip.z * clip.z) * _Distort.y;
    o.pos = clip;
    o.uv = v.texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
    return o;
  }
  fixed4 frag(v2f i) : SV_Target {
    return tex2D(_MainTex, i.uv) * _MainColor;
  }
  ENDCG
 }
}
}
