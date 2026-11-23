Shader "Custom/Distorted/Unlit" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
}
SubShader {
 Pass {
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  // Globals set by ThemeManager (same names as the original GLES program).
  float4 _SkyGradientTopColor;
  float4 _SkyGradientBottomColor;
  float _SkyGradientOffset;
  float4 _FogSilhouetteColor;
  float4 _Distort;

  sampler2D _MainTex;
  float4 _MainTex_ST;

  struct appdata { float4 vertex : POSITION; float2 texcoord : TEXCOORD0; };
  struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; fixed4 fog : TEXCOORD1; };

  // Line-by-line port of the original Custom/Distorted/Unlit GLES program.
  // Bend is applied in clip space: x,y += z*z*_Distort; fog colour and alpha from clip w.
  v2f vert(appdata v) {
    v2f o;
    float4 clip = UnityObjectToClipPos(v.vertex);
    clip.x = clip.x + (clip.z * clip.z) * _Distort.x;
    clip.y = clip.y + (clip.z * clip.z) * _Distort.y;
    float screenPosY = clip.y / clip.w;
    float3 col = lerp(_SkyGradientBottomColor.rgb, _SkyGradientTopColor.rgb,
                      saturate((screenPosY - _SkyGradientOffset) / (1.0 - _SkyGradientOffset)));
    col = lerp(col, _FogSilhouetteColor.rgb, saturate((1100.0 - clip.w) / 300.0));
    float fogAmt = saturate((800.0 - clip.w) / 500.0);
    o.pos = clip;
    o.uv = v.texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
    o.fog = fixed4(col, fogAmt);
    return o;
  }
  fixed4 frag(v2f i) : SV_Target {
    fixed4 tex = tex2D(_MainTex, i.uv);
    fixed4 c;
    c.w = tex.w;
    c.xyz = lerp(i.fog.xyz, tex.xyz, i.fog.w);
    return c;
  }
  ENDCG
 }
}
}
