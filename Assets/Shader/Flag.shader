// Port of the ORIGINAL "Bend/Flag" shader (sharedassets1.assets, path id 371).
// Source: GLES program decoded from the original m_SubProgramBlob (LZ4), cross-checked
// against the original m_Script ShaderLab text and the sibling "Bend UV Scroll (Screen)Simple" shader.
//
// VERIFIED from the original:
//   - Properties: _MainTex, _Color, _AmplitudeX, _AmplitudeY, _FrequenceyX, _FrequenceyY (exact names).
//   - _Distort, _SkyGradientTopColor/BottomColor, _FogSilhouetteColor, _SkyGradientOffset are
//     NOT properties in the original; they are globals (ThemeManager.SetShaderStates sets them).
//   - Wave uses UV2 (TEXCOORD1): y = uv2.x, z = uv2.y. FlagShaderTexcoordSetter writes uv2 = (vertex.y, vertex.z).
//   - Wave is applied in object space, then MVP, then a clip-space bend using clip.z^2 and _Distort.xy.
//   - Sky gradient (screen Y after bend) and fog-silhouette colour computed per vertex; fog alpha in .w.
//   - Fragment: out.rgb = lerp(fog.rgb, tex.rgb * _Color.rgb, fog.a); out.a = tex.a * _Color.a.
//   - No vertex colour (COLOR) used. No Blend, no ZWrite override, Cull Off.
//
// NOT VERIFIED (see report): mesh UV2 presence on flag prefabs without the setter; global _Distort
// value in the original build (repo ThemeManager uses -0.0005,-0.0003; original flag.mat saved -0.001,0).

Shader "Bend/Flag" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _Color ("Color (RGBA)", Color) = (1,1,1,1)
 _AmplitudeX ("Amplitude X", Float) = 0.05
 _AmplitudeY ("Amplitude Y", Float) = 0.05
 _FrequenceyX ("Frequencey X", Float) = 9.5
 _FrequenceyY ("Frequencey Y", Float) = 6.5
}
SubShader {
 Pass {
  // Original state: Cull Off, no Blend, default ZWrite On / ZTest LEqual (opaque, Geometry queue).
  Cull Off

  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  float4 _MainTex_ST;
  float4 _Color;
  float _AmplitudeX;
  float _AmplitudeY;
  float _FrequenceyX;
  float _FrequenceyY;

  // Globals (not material properties). Set by ThemeManager.SetShaderStates and
  // Shader.SetGlobalVector("_Distort") in ThemeManager.cs.
  float4 _Distort;
  float4 _SkyGradientTopColor;
  float4 _SkyGradientBottomColor;
  float4 _FogSilhouetteColor;
  float _SkyGradientOffset;

  struct appdata {
    float4 vertex : POSITION;
    float2 texcoord : TEXCOORD0;   // _glesMultiTexCoord0
    float2 uv2 : TEXCOORD1;        // _glesMultiTexCoord1: wave coordinates (y, z of the mesh)
  };

  struct v2f {
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
    float4 fog : TEXCOORD1;        // rgb = sky/fog colour, a = fog blend factor
  };

  v2f vert(appdata v) {
    v2f o;

    // Original: localPos = (0, uv2.x, uv2.y); z_9 = vertex.z
    float localY = v.uv2.x;
    float localZ = v.uv2.y;
    float phase = 0.01 * v.vertex.z;   // tmpvar_15

    // X wave. Original: frequency = _FrequenceyX + 0.0001 * uv2.x; sin(-t * freq + (uv2.y*0.7 + phase))
    float ampX = _AmplitudeX * localZ;
    float freqX = _FrequenceyX + 0.0001 * localY;
    float offX = sin(-_Time.y * freqX + (localZ * 0.7 + phase)) * ampX;

    // Y wave. Original: frequency = _FrequenceyY; sin(t * freq + (uv2.y*0.1 + phase))
    float ampY = _AmplitudeY * localZ;
    float offY = sin(_Time.y * _FrequenceyY + (localZ * 0.1 + phase)) * ampY;

    // Waved object-space position, w unchanged. off.z = 0.
    float4 wpos = float4(v.vertex.xyz + float3(offX, offY, 0.0), v.vertex.w);
    float4 clip = mul(UNITY_MATRIX_MVP, wpos);

    // Clip-space bend (original applies it before the perspective divide).
    float dd = clip.z * clip.z;
    clip.x += dd * _Distort.x;
    clip.y += dd * _Distort.y;
    o.pos = clip;

    // Sky gradient from post-bend screen Y.
    float screenY = clip.y / clip.w;
    float3 sky = lerp(_SkyGradientBottomColor.rgb, _SkyGradientTopColor.rgb,
                      saturate((screenY - _SkyGradientOffset) / (1.0 - _SkyGradientOffset)));
    // Fog silhouette colour: 1 at depth <= 800, 0 at depth >= 1100 (clip.w = view depth).
    sky = lerp(sky, _FogSilhouetteColor.rgb, saturate((1100.0 - clip.w) / 300.0));
    // Fog blend factor: 1 at depth <= 300, 0 at depth >= 800.
    float fogAlpha = saturate((800.0 - clip.w) / 500.0);
    o.fog = float4(sky, fogAlpha);

    o.uv = v.texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
    return o;
  }

  fixed4 frag(v2f i) : SV_Target {
    fixed4 t = tex2D(_MainTex, i.uv) * _Color;
    // Original: c.rgb = mix(fog.rgb, tex.rgb, fog.a); c.a = tex.a (texture * colour alpha).
    return fixed4(lerp(i.fog.rgb, t.rgb, i.fog.a), t.a);
  }
  ENDCG
 }
}
}
