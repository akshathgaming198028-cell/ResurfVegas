Shader "Custom/Distorted/Unlit with Additive Layer" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _Color ("Color Tint", Color) = (0,0,0,0)
 _ShiftColor ("Shift Color", Float) = 0
}
SubShader {
 Pass {
  // Port of the original GLES program (verified from the APK shader blob).
  // No render-state tags in the original: default Geometry queue, opaque, ZWrite On, Cull Back, no blend.
  // Output is mix(texture, _Color, _ShiftColor). ThemeManager/CharacterScreenController drive _ShiftColor
  // (0 = texture, 0.6 = faded toward the overlay colour) and set _Color to white.
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  float4 _MainTex_ST;
  fixed4 _Color;
  float _ShiftColor;
  // _Distort is global (not in Properties), as in the original.
  float4 _Distort;

  struct appdata { float4 vertex : POSITION; float4 texcoord : TEXCOORD0; };
  struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

  v2f vert(appdata v) {
    v2f o;
    // Clip-space curve: x += z*z*_Distort.x, y += z*z*_Distort.y (same as the original).
    o.pos = UnityObjectToClipPos(v.vertex);
    o.pos.x = o.pos.x + (o.pos.z * o.pos.z) * _Distort.x;
    o.pos.y = o.pos.y + (o.pos.z * o.pos.z) * _Distort.y;
    o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
    return o;
  }

  fixed4 frag(v2f i) : SV_Target {
    fixed4 t = tex2D(_MainTex, i.uv);
    return lerp(t, _Color, _ShiftColor);
  }
  ENDCG
 }
}
}
