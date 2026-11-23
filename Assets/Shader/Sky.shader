Shader "Custom/Sky" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _GradientFade ("Fade", Float) = 1
 _GradientOffset ("Gradient offset", Float) = 0
}
SubShader {
 Tags { "QUEUE"="Background+2000" "RenderType"="Opaque" }
 Pass {
  Tags { "QUEUE"="Background+2000" "RenderType"="Opaque" }
  ZWrite Off
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  // Globals set by ThemeManager.SetShaderStates (same names as the original GLES program).
  float4 _SkyGradientTopColor;
  float4 _SkyGradientBottomColor;
  float _SkyGradientOffset;

  struct appdata { float4 vertex : POSITION; };
  struct v2f { float4 pos : SV_POSITION; fixed4 col : COLOR0; };

  // Line-by-line port of the original Custom/Sky pass 1 (GLES):
  // gl_Position = MVP * vertex; gradient from bottom to top by clip-space screen Y.
  v2f vert(appdata v) {
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    float screenPosY = o.pos.y / o.pos.w;
    o.col = fixed4(lerp(_SkyGradientBottomColor.rgb, _SkyGradientTopColor.rgb,
                        saturate((screenPosY - _SkyGradientOffset) / (1.0 - _SkyGradientOffset))), 1);
    return o;
  }
  fixed4 frag(v2f i) : SV_Target { return i.col; }
  ENDCG
 }
 Pass {
  Tags { "QUEUE"="Background" "RenderType"="Opaque" }
  ZWrite Off
  Cull Front
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  float4 _MainTex_ST;

  struct appdata { float4 vertex : POSITION; float4 texcoord : TEXCOORD0; };
  struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

  // Line-by-line port of the original Custom/Sky pass 2 (GLES): plain textured draw.
  v2f vert(appdata v) {
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
    return o;
  }
  fixed4 frag(v2f i) : SV_Target { return tex2D(_MainTex, i.uv); }
  ENDCG
 }
}
Fallback "Diffuse"
}
