Shader "Custom/Distorted/Unlit with overlay" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _Color ("Color Tint", Color) = (0,0,0,0)
}
SubShader {
 Pass {
  // Line-by-line port of the original GLES program (no render-state tags, so default Geometry / ZWrite On / Cull Back).
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  float4 _MainTex_ST;
  fixed4 _Color;

  // Globals set by ThemeManager.SetShaderStates (same names as the original GLES program).
  // _Distort is intentionally NOT in Properties: the original uses the global value.
  float4 _Distort;
  float4 _SkyGradientTopColor;
  float4 _SkyGradientBottomColor;
  float4 _FogSilhouetteColor;
  float _SkyGradientOffset;

  struct appdata { float4 vertex : POSITION; float4 texcoord : TEXCOORD0; };
  struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; fixed4 fog : TEXCOORD1; };

  v2f vert(appdata v) {
    v2f o;
    // tmpvar_2 = MVP * vertex, then clip-space curve: x += z*z*_Distort.x, y += z*z*_Distort.y
    o.pos = UnityObjectToClipPos(v.vertex);
    o.pos.x = o.pos.x + (o.pos.z * o.pos.z) * _Distort.x;
    o.pos.y = o.pos.y + (o.pos.z * o.pos.z) * _Distort.y;
    o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;

    // Sky gradient by clip-space screen Y (after the curve), as the original.
    float screenPosY = o.pos.y / o.pos.w;
    float3 sky = lerp(_SkyGradientBottomColor.rgb, _SkyGradientTopColor.rgb,
                      saturate((screenPosY - _SkyGradientOffset) / (1.0 - _SkyGradientOffset)));
    // Fog silhouette colour mix by distance (1100..800), and fog alpha (800..1300).
    float fogColorAmt = saturate((1100.0 - o.pos.w) / 300.0);
    sky = lerp(sky, _FogSilhouetteColor.rgb, fogColorAmt);
    float fogAlpha = saturate((800.0 - o.pos.w) / 500.0);
    o.fog = fixed4(sky, fogAlpha);
    return o;
  }

  fixed4 frag(v2f i) : SV_Target {
    // c = tex * _Color; out.a = c.a; out.rgb = mix(sky, c.rgb, fogAlpha)
    fixed4 c = tex2D(_MainTex, i.uv) * _Color;
    fixed4 outc;
    outc.a = c.a;
    outc.rgb = lerp(i.fog.rgb, c.rgb, i.fog.a);
    return outc;
  }
  ENDCG
 }
}
}
