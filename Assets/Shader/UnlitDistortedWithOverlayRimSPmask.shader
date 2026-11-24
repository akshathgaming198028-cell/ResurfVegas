Shader "Custom/Distorted/Unlit with overlay and Rim and SPmask" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _Mask ("Mask (RGB)", 2D) = "white" { }
 _Color ("Color Tint", Color) = (0,0,0,0)
 _RimColor ("Reflection Color", Color) = (1,1,1,0.5)
}
SubShader {
 Pass {
  // Port of the original GLES program: screen-space mask (clip xy / w, clamped, 0..1) plus sky/fog overlay.
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  float4 _MainTex_ST;
  sampler2D _Mask;
  fixed4 _Color;
  // Set as an int by DebugSettingsPopup; unset means off (the original's default branch).
  float _DebugShaders;

  // Globals set by ThemeManager.SetShaderStates (not material properties, as in the original).
  float4 _Distort;
  float4 _SkyGradientTopColor;
  float4 _SkyGradientBottomColor;
  float4 _FogSilhouetteColor;
  float _SkyGradientOffset;

  // Sky gradient + fog silhouette overlay, identical to the original (applied after the clip curve).
  fixed4 SkyFogOverlay(float4 p) {
    float screenPosY = p.y / p.w;
    float3 sky = lerp(_SkyGradientBottomColor.rgb, _SkyGradientTopColor.rgb,
                      saturate((screenPosY - _SkyGradientOffset) / (1.0 - _SkyGradientOffset)));
    sky = lerp(sky, _FogSilhouetteColor.rgb, saturate((1100.0 - p.w) / 300.0));
    return fixed4(sky, saturate((800.0 - p.w) / 500.0));
  }
  fixed4 ApplySkyFog(fixed4 c, fixed4 fog) {
    fixed4 o; o.a = c.a; o.rgb = lerp(fog.rgb, c.rgb, fog.a); return o;
  }

  struct appdata { float4 vertex : POSITION; float4 texcoord : TEXCOORD0; };
  struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD2; float2 muv : TEXCOORD3; fixed4 fog : TEXCOORD4; };

  v2f vert(appdata v) {
    v2f o;

    o.pos = UnityObjectToClipPos(v.vertex);
    o.pos.x = o.pos.x + (o.pos.z * o.pos.z) * _Distort.x;
    o.pos.y = o.pos.y + (o.pos.z * o.pos.z) * _Distort.y;

    float2 m = o.pos.xy / o.pos.w;
    m = clamp(m, float2(-1.0, -1.0), float2(1.0, 1.0));
    o.muv = m * 0.5 + 0.5;
    o.fog = SkyFogOverlay(o.pos);
    o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
    return o;
  }

  fixed4 frag(v2f i) : SV_Target {
    fixed4 mask = tex2D(_Mask, i.muv);
    if (_DebugShaders > 0.5) return mask;
    fixed4 c = tex2D(_MainTex, i.uv) * _Color + mask;
    return ApplySkyFog(c, i.fog);
  }
  ENDCG
 }
}
}
