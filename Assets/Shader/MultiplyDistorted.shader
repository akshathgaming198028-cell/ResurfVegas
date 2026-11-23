Shader "Custom/Distorted/Multiply" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _Fade ("Fade Factor", Range(0,1)) = 0
}
SubShader {
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  Blend DstColor Zero
  Offset 0, -1
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
  float _Fade;

  struct appdata { float4 vertex : POSITION; float2 texcoord : TEXCOORD0; };
  struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; float4 fog : TEXCOORD1; };

  // Line-by-line port of the original Custom/Distorted/Multiply GLES program.
  // Bend in clip space, gradient from clip-space screen Y (after the bend), distance fog and alpha.
  v2f vert(appdata v) {
    v2f o;
    float4 clip = UnityObjectToClipPos(v.vertex);
    clip.x = clip.x + (clip.z * clip.z) * _Distort.x;
    clip.y = clip.y + (clip.z * clip.z) * _Distort.y;
    o.uv = v.texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
    float screenPosY = clip.y / clip.w;
    float3 col = lerp(_SkyGradientBottomColor.rgb, _SkyGradientTopColor.rgb,
                      saturate((screenPosY - _SkyGradientOffset) / (1.0 - _SkyGradientOffset)));
    col = lerp(col, _FogSilhouetteColor.rgb, saturate((1100.0 - clip.w) / 300.0));
    o.fog = float4(col, saturate((800.0 - clip.w) / 500.0));
    o.pos = clip;
    return o;
  }
  fixed4 frag(v2f i) : SV_Target {
    fixed4 t2 = lerp(tex2D(_MainTex, i.uv), fixed4(1, 1, 1, 1), _Fade);
    fixed4 cf;
    cf.xyz = lerp(i.fog.xyz, float3(1, 1, 1), t2.xyz);
    cf.w = 1;
    return lerp(cf, t2, i.fog.w);
  }
  ENDCG
 }
}
}
