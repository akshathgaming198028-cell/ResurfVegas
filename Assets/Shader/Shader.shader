Shader "Standard" {
Properties {
 _Color ("Color", Color) = (1,1,1,1)
 _MainTex ("Albedo", 2D) = "white" { }
 _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
 _Glossiness ("Smoothness", Range(0,1)) = 0.5
[Gamma]  _Metallic ("Metallic", Range(0,1)) = 0
 _MetallicGlossMap ("Metallic", 2D) = "white" { }
 _BumpScale ("Scale", Float) = 1
 _BumpMap ("Normal Map", 2D) = "bump" { }
 _Parallax ("Height Scale", Range(0.005,0.08)) = 0.02
 _ParallaxMap ("Height Map", 2D) = "black" { }
 _OcclusionStrength ("Strength", Range(0,1)) = 1
 _OcclusionMap ("Occlusion", 2D) = "white" { }
 _EmissionColor ("Color", Color) = (0,0,0,1)
 _EmissionMap ("Emission", 2D) = "white" { }
 _DetailMask ("Detail Mask", 2D) = "white" { }
 _DetailAlbedoMap ("Detail Albedo x2", 2D) = "grey" { }
 _DetailNormalMapScale ("Scale", Float) = 1
 _DetailNormalMap ("Normal Map", 2D) = "bump" { }
[Enum(UV0,0,UV1,1)]  _UVSec ("UV Set for secondary textures", Float) = 0
[HideInInspector]  _Mode ("__mode", Float) = 0
[HideInInspector]  _SrcBlend ("__src", Float) = 1
[HideInInspector]  _DstBlend ("__dst", Float) = 0
[HideInInspector]  _ZWrite ("__zw", Float) = 1
}
SubShader { 
 LOD 300
 Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardBase" "SHADOWSUPPORT"="true" "RenderType"="Opaque" "PerformanceChecks"="False" }
  ZWrite [_ZWrite]
  Blend [_SrcBlend] [_DstBlend]
GLSLPROGRAM

ENDGLSL

 }
 Pass {
  Name "FORWARD_DELTA"
  Tags { "LIGHTMODE"="ForwardAdd" "SHADOWSUPPORT"="true" "RenderType"="Opaque" "PerformanceChecks"="False" }
  ZWrite Off
  Blend [_SrcBlend] One
GLSLPROGRAM

ENDGLSL

 }
 Pass {
  Name "SHADOWCASTER"
  Tags { "LIGHTMODE"="SHADOWCASTER" "SHADOWSUPPORT"="true" "RenderType"="Opaque" "PerformanceChecks"="False" }
GLSLPROGRAM

ENDGLSL

 }
}
SubShader { 
 LOD 150
 Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardBase" "SHADOWSUPPORT"="true" "RenderType"="Opaque" "PerformanceChecks"="False" }
  ZWrite [_ZWrite]
  Blend [_SrcBlend] [_DstBlend]
GLSLPROGRAM
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" }
					
					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					attribute vec3 _glesNormal;
					attribute vec4 _glesMultiTexCoord0;
					attribute vec4 _glesMultiTexCoord1;
					uniform highp vec3 _WorldSpaceCameraPos;
					uniform mediump vec4 unity_SHAr;
					uniform mediump vec4 unity_SHAg;
					uniform mediump vec4 unity_SHAb;
					uniform mediump vec4 unity_SHBr;
					uniform mediump vec4 unity_SHBg;
					uniform mediump vec4 unity_SHBb;
					uniform mediump vec4 unity_SHC;
					uniform highp mat4 glstate_matrix_mvp;
					uniform highp mat4 _Object2World;
					uniform highp mat4 _World2Object;
					uniform mediump vec4 unity_ColorSpaceDielectricSpec;
					uniform highp vec4 _MainTex_ST;
					uniform highp vec4 _DetailAlbedoMap_ST;
					uniform mediump float _Metallic;
					uniform mediump float _Glossiness;
					uniform mediump float _UVSec;
					varying highp vec4 xlv_TEXCOORD0;
					varying mediump vec4 xlv_TEXCOORD1;
					varying mediump vec4 xlv_TEXCOORD2;
					varying mediump vec4 xlv_TEXCOORD4;
					varying mediump vec4 xlv_TEXCOORD5;
					void main ()
					{
					  mediump vec3 tmpvar_1;
					  tmpvar_1 = _glesNormal;
					  highp vec2 tmpvar_2;
					  tmpvar_2 = _glesMultiTexCoord0.xy;
					  highp vec2 tmpvar_3;
					  tmpvar_3 = _glesMultiTexCoord1.xy;
					  mediump vec3 normalWorld_4;
					  mediump vec3 eyeVec_5;
					  highp vec4 tmpvar_6;
					  mediump vec4 tmpvar_7;
					  mediump vec4 tmpvar_8;
					  mediump vec4 tmpvar_9;
					  highp vec4 tmpvar_10;
					  tmpvar_10 = (_Object2World * _glesVertex);
					  tmpvar_6 = (glstate_matrix_mvp * _glesVertex);
					  highp vec4 texcoord_11;
					  texcoord_11.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
					  highp vec2 tmpvar_12;
					  if ((_UVSec == 0.0)) {
					    tmpvar_12 = tmpvar_2;
					  } else {
					    tmpvar_12 = tmpvar_3;
					  };
					  texcoord_11.zw = ((tmpvar_12 * _DetailAlbedoMap_ST.xy) + _DetailAlbedoMap_ST.zw);
					  highp vec3 tmpvar_13;
					  tmpvar_13 = normalize((tmpvar_10.xyz - _WorldSpaceCameraPos));
					  eyeVec_5 = tmpvar_13;
					  highp vec3 norm_14;
					  norm_14 = tmpvar_1;
					  highp vec4 v_15;
					  v_15.x = _World2Object[0].x;
					  v_15.y = _World2Object[1].x;
					  v_15.z = _World2Object[2].x;
					  v_15.w = _World2Object[3].x;
					  highp vec4 v_16;
					  v_16.x = _World2Object[0].y;
					  v_16.y = _World2Object[1].y;
					  v_16.z = _World2Object[2].y;
					  v_16.w = _World2Object[3].y;
					  highp vec4 v_17;
					  v_17.x = _World2Object[0].z;
					  v_17.y = _World2Object[1].z;
					  v_17.z = _World2Object[2].z;
					  v_17.w = _World2Object[3].z;
					  highp vec3 tmpvar_18;
					  tmpvar_18 = normalize(((
					    (v_15.xyz * norm_14.x)
					   + 
					    (v_16.xyz * norm_14.y)
					  ) + (v_17.xyz * norm_14.z)));
					  normalWorld_4 = tmpvar_18;
					  tmpvar_9.xyz = normalWorld_4;
					  tmpvar_7.xyz = eyeVec_5;
					  mediump vec4 ambientOrLightmapUV_19;
					  ambientOrLightmapUV_19.w = 0.0;
					  mediump vec4 tmpvar_20;
					  tmpvar_20.w = 1.0;
					  tmpvar_20.xyz = normalWorld_4;
					  mediump vec3 res_21;
					  mediump vec3 x_22;
					  x_22.x = dot (unity_SHAr, tmpvar_20);
					  x_22.y = dot (unity_SHAg, tmpvar_20);
					  x_22.z = dot (unity_SHAb, tmpvar_20);
					  mediump vec3 x1_23;
					  mediump vec4 tmpvar_24;
					  tmpvar_24 = (normalWorld_4.xyzz * normalWorld_4.yzzx);
					  x1_23.x = dot (unity_SHBr, tmpvar_24);
					  x1_23.y = dot (unity_SHBg, tmpvar_24);
					  x1_23.z = dot (unity_SHBb, tmpvar_24);
					  res_21 = (x_22 + (x1_23 + (unity_SHC.xyz * 
					    ((normalWorld_4.x * normalWorld_4.x) - (normalWorld_4.y * normalWorld_4.y))
					  )));
					  res_21 = max (((1.055 * 
					    pow (max (res_21, vec3(0.0, 0.0, 0.0)), vec3(0.4166667, 0.4166667, 0.4166667))
					  ) - 0.055), vec3(0.0, 0.0, 0.0));
					  ambientOrLightmapUV_19.xyz = max (vec3(0.0, 0.0, 0.0), res_21);
					  tmpvar_8.yzw = (eyeVec_5 - (2.0 * (
					    dot (normalWorld_4, eyeVec_5)
					   * normalWorld_4)));
					  mediump float x_25;
					  x_25 = (1.0 - clamp (dot (normalWorld_4, 
					    -(eyeVec_5)
					  ), 0.0, 1.0));
					  tmpvar_9.w = ((x_25 * x_25) * (x_25 * x_25));
					  tmpvar_7.w = clamp ((_Glossiness + (1.0 - 
					    (unity_ColorSpaceDielectricSpec.w - (_Metallic * unity_ColorSpaceDielectricSpec.w))
					  )), 0.0, 1.0);
					  gl_Position = tmpvar_6;
					  xlv_TEXCOORD0 = texcoord_11;
					  xlv_TEXCOORD1 = tmpvar_7;
					  xlv_TEXCOORD2 = ambientOrLightmapUV_19;
					  xlv_TEXCOORD4 = tmpvar_8;
					  xlv_TEXCOORD5 = tmpvar_9;
					}
					
					
					#endif
					#ifdef FRAGMENT
					uniform mediump vec4 _WorldSpaceLightPos0;
					uniform lowp samplerCube unity_SpecCube0;
					uniform mediump vec4 unity_SpecCube0_HDR;
					uniform mediump vec4 unity_ColorSpaceDielectricSpec;
					uniform lowp vec4 _LightColor0;
					uniform sampler2D unity_NHxRoughness;
					uniform mediump vec4 _Color;
					uniform sampler2D _MainTex;
					uniform mediump float _Metallic;
					uniform mediump float _Glossiness;
					uniform sampler2D _OcclusionMap;
					varying highp vec4 xlv_TEXCOORD0;
					varying mediump vec4 xlv_TEXCOORD1;
					varying mediump vec4 xlv_TEXCOORD2;
					varying mediump vec4 xlv_TEXCOORD4;
					varying mediump vec4 xlv_TEXCOORD5;
					void main ()
					{
					  mediump float rl_1;
					  lowp vec4 tmpvar_2;
					  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD0.xy);
					  mediump vec3 tmpvar_3;
					  tmpvar_3 = (_Color.xyz * tmpvar_2.xyz);
					  mediump vec3 tmpvar_4;
					  mediump vec3 tmpvar_5;
					  tmpvar_5 = mix (unity_ColorSpaceDielectricSpec.xyz, tmpvar_3, vec3(_Metallic));
					  tmpvar_4 = (tmpvar_3 * (unity_ColorSpaceDielectricSpec.w - (_Metallic * unity_ColorSpaceDielectricSpec.w)));
					  mediump vec3 tmpvar_6;
					  tmpvar_6 = _LightColor0.xyz;
					  mediump float tmpvar_7;
					  lowp vec4 tmpvar_8;
					  tmpvar_8 = texture2D (_OcclusionMap, xlv_TEXCOORD0.xy);
					  tmpvar_7 = tmpvar_8.y;
					  rl_1 = dot (xlv_TEXCOORD4.yzw, _WorldSpaceLightPos0.xyz);
					  highp vec4 tmpvar_9;
					  tmpvar_9 = unity_SpecCube0_HDR;
					  mediump float tmpvar_10;
					  tmpvar_10 = (1.0 - _Glossiness);
					  mediump vec4 hdr_11;
					  hdr_11 = tmpvar_9;
					  mediump vec4 tmpvar_12;
					  tmpvar_12.xyz = xlv_TEXCOORD4.yzw;
					  tmpvar_12.w = ((tmpvar_10 * (1.7 - 
					    (0.7 * tmpvar_10)
					  )) * 6.0);
					  lowp vec4 tmpvar_13;
					  tmpvar_13 = textureCube (unity_SpecCube0, xlv_TEXCOORD4.yzw, tmpvar_12.w);
					  mediump vec4 tmpvar_14;
					  tmpvar_14 = tmpvar_13;
					  mediump vec2 tmpvar_15;
					  tmpvar_15.x = ((rl_1 * rl_1) * (rl_1 * rl_1));
					  tmpvar_15.y = tmpvar_10;
					  lowp vec4 tmpvar_16;
					  tmpvar_16 = texture2D (unity_NHxRoughness, tmpvar_15);
					  mediump vec4 tmpvar_17;
					  tmpvar_17.w = 1.0;
					  tmpvar_17.xyz = (((
					    (xlv_TEXCOORD2.xyz * tmpvar_7)
					   * tmpvar_4) + (
					    (((tmpvar_14.w * hdr_11.x) * tmpvar_14.xyz) * tmpvar_7)
					   * 
					    mix (tmpvar_5, xlv_TEXCOORD1.www, xlv_TEXCOORD5.www)
					  )) + ((tmpvar_4 + 
					    ((tmpvar_16.w * 16.0) * tmpvar_5)
					  ) * (tmpvar_6 * 
					    clamp (dot (xlv_TEXCOORD5.xyz, _WorldSpaceLightPos0.xyz), 0.0, 1.0)
					  )));
					  mediump vec4 xlat_varoutput_18;
					  xlat_varoutput_18.xyz = tmpvar_17.xyz;
					  xlat_varoutput_18.w = 1.0;
					  gl_FragData[0] = xlat_varoutput_18;
					}
					
					
					#endif


Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" }
					

ENDGLSL

 }
 Pass {
  Name "FORWARD_DELTA"
  Tags { "LIGHTMODE"="ForwardAdd" "SHADOWSUPPORT"="true" "RenderType"="Opaque" "PerformanceChecks"="False" }
  ZWrite Off
  Blend [_SrcBlend] One
GLSLPROGRAM
Keywords { "POINT" "SHADOWS_OFF" }
					
					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					attribute vec3 _glesNormal;
					attribute vec4 _glesMultiTexCoord0;
					attribute vec4 _glesMultiTexCoord1;
					uniform highp vec3 _WorldSpaceCameraPos;
					uniform highp vec4 _WorldSpaceLightPos0;
					uniform highp mat4 glstate_matrix_mvp;
					uniform highp mat4 _Object2World;
					uniform highp mat4 _World2Object;
					uniform highp vec4 _MainTex_ST;
					uniform highp vec4 _DetailAlbedoMap_ST;
					uniform mediump float _UVSec;
					uniform mediump mat4 _LightMatrix0;
					varying highp vec4 xlv_TEXCOORD0;
					varying highp vec3 xlv_TEXCOORD1;
					varying mediump vec4 xlv_TEXCOORD3;
					varying mediump vec3 xlv_TEXCOORD4;
					varying mediump vec3 xlv_TEXCOORD5;
					void main ()
					{
					  mediump vec3 tmpvar_1;
					  tmpvar_1 = _glesNormal;
					  highp vec2 tmpvar_2;
					  tmpvar_2 = _glesMultiTexCoord0.xy;
					  highp vec2 tmpvar_3;
					  tmpvar_3 = _glesMultiTexCoord1.xy;
					  mediump vec3 normalWorld_4;
					  mediump vec3 eyeVec_5;
					  mediump vec3 lightDir_6;
					  highp vec4 tmpvar_7;
					  mediump vec4 tmpvar_8;
					  highp vec4 tmpvar_9;
					  tmpvar_9 = (_Object2World * _glesVertex);
					  tmpvar_7 = (glstate_matrix_mvp * _glesVertex);
					  highp vec4 texcoord_10;
					  texcoord_10.xy = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
					  highp vec2 tmpvar_11;
					  if ((_UVSec == 0.0)) {
					    tmpvar_11 = tmpvar_2;
					  } else {
					    tmpvar_11 = tmpvar_3;
					  };
					  texcoord_10.zw = ((tmpvar_11 * _DetailAlbedoMap_ST.xy) + _DetailAlbedoMap_ST.zw);
					  highp vec3 tmpvar_12;
					  tmpvar_12 = (_WorldSpaceLightPos0.xyz - (tmpvar_9.xyz * _WorldSpaceLightPos0.w));
					  lightDir_6 = tmpvar_12;
					  mediump vec3 tmpvar_13;
					  tmpvar_13 = normalize(lightDir_6);
					  lightDir_6 = tmpvar_13;
					  highp vec3 tmpvar_14;
					  tmpvar_14 = normalize((tmpvar_9.xyz - _WorldSpaceCameraPos));
					  eyeVec_5 = tmpvar_14;
					  highp vec3 norm_15;
					  norm_15 = tmpvar_1;
					  highp vec4 v_16;
					  v_16.x = _World2Object[0].x;
					  v_16.y = _World2Object[1].x;
					  v_16.z = _World2Object[2].x;
					  v_16.w = _World2Object[3].x;
					  highp vec4 v_17;
					  v_17.x = _World2Object[0].y;
					  v_17.y = _World2Object[1].y;
					  v_17.z = _World2Object[2].y;
					  v_17.w = _World2Object[3].y;
					  highp vec4 v_18;
					  v_18.x = _World2Object[0].z;
					  v_18.y = _World2Object[1].z;
					  v_18.z = _World2Object[2].z;
					  v_18.w = _World2Object[3].z;
					  highp vec3 tmpvar_19;
					  tmpvar_19 = normalize(((
					    (v_16.xyz * norm_15.x)
					   + 
					    (v_17.xyz * norm_15.y)
					  ) + (v_18.xyz * norm_15.z)));
					  normalWorld_4 = tmpvar_19;
					  tmpvar_8.yzw = (eyeVec_5 - (2.0 * (
					    dot (normalWorld_4, eyeVec_5)
					   * normalWorld_4)));
					  gl_Position = tmpvar_7;
					  xlv_TEXCOORD0 = texcoord_10;
					  xlv_TEXCOORD1 = (_LightMatrix0 * (_Object2World * _glesVertex)).xyz;
					  xlv_TEXCOORD3 = tmpvar_8;
					  xlv_TEXCOORD4 = tmpvar_13;
					  xlv_TEXCOORD5 = normalWorld_4;
					}
					
					
					#endif
					#ifdef FRAGMENT
					uniform mediump vec4 unity_ColorSpaceDielectricSpec;
					uniform lowp vec4 _LightColor0;
					uniform sampler2D unity_NHxRoughness;
					uniform mediump vec4 _Color;
					uniform sampler2D _MainTex;
					uniform mediump float _Metallic;
					uniform mediump float _Glossiness;
					uniform sampler2D _LightTexture0;
					varying highp vec4 xlv_TEXCOORD0;
					varying highp vec3 xlv_TEXCOORD1;
					varying mediump vec4 xlv_TEXCOORD3;
					varying mediump vec3 xlv_TEXCOORD4;
					varying mediump vec3 xlv_TEXCOORD5;
					void main ()
					{
					  mediump vec3 c_1;
					  lowp vec4 tmpvar_2;
					  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD0.xy);
					  mediump vec3 tmpvar_3;
					  tmpvar_3 = (_Color.xyz * tmpvar_2.xyz);
					  mediump float tmpvar_4;
					  tmpvar_4 = dot (xlv_TEXCOORD3.yzw, xlv_TEXCOORD4);
					  mediump vec2 tmpvar_5;
					  tmpvar_5.x = ((tmpvar_4 * tmpvar_4) * (tmpvar_4 * tmpvar_4));
					  tmpvar_5.y = (1.0 - _Glossiness);
					  lowp vec4 tmpvar_6;
					  tmpvar_6 = texture2D (unity_NHxRoughness, tmpvar_5);
					  c_1 = (((tmpvar_3 * 
					    (unity_ColorSpaceDielectricSpec.w - (_Metallic * unity_ColorSpaceDielectricSpec.w))
					  ) + (
					    (tmpvar_6.w * 16.0)
					   * 
					    mix (unity_ColorSpaceDielectricSpec.xyz, tmpvar_3, vec3(_Metallic))
					  )) * _LightColor0.xyz);
					  highp float tmpvar_7;
					  tmpvar_7 = dot (xlv_TEXCOORD1, xlv_TEXCOORD1);
					  lowp vec4 tmpvar_8;
					  tmpvar_8 = texture2D (_LightTexture0, vec2(tmpvar_7));
					  c_1 = (c_1 * (tmpvar_8.w * clamp (
					    dot (xlv_TEXCOORD5, xlv_TEXCOORD4)
					  , 0.0, 1.0)));
					  mediump vec4 tmpvar_9;
					  tmpvar_9.w = 1.0;
					  tmpvar_9.xyz = c_1;
					  mediump vec4 xlat_varoutput_10;
					  xlat_varoutput_10.xyz = tmpvar_9.xyz;
					  xlat_varoutput_10.w = 1.0;
					  gl_FragData[0] = xlat_varoutput_10;
					}
					
					
					#endif


Keywords { "POINT" "SHADOWS_OFF" }
					

ENDGLSL

 }
 Pass {
  Name "SHADOWCASTER"
  Tags { "LIGHTMODE"="SHADOWCASTER" "SHADOWSUPPORT"="true" "RenderType"="Opaque" "PerformanceChecks"="False" }
GLSLPROGRAM
Keywords { "SHADOWS_DEPTH" }
					
					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					attribute vec3 _glesNormal;
					uniform highp vec4 _WorldSpaceLightPos0;
					uniform highp vec4 unity_LightShadowBias;
					uniform highp mat4 glstate_matrix_mvp;
					uniform highp mat4 _Object2World;
					uniform highp mat4 _World2Object;
					uniform highp mat4 unity_MatrixVP;
					void main ()
					{
					  highp vec3 vertex_1;
					  vertex_1 = _glesVertex.xyz;
					  highp vec4 clipPos_2;
					  if ((unity_LightShadowBias.z != 0.0)) {
					    highp vec4 tmpvar_3;
					    tmpvar_3.w = 1.0;
					    tmpvar_3.xyz = vertex_1;
					    highp vec3 tmpvar_4;
					    tmpvar_4 = (_Object2World * tmpvar_3).xyz;
					    highp vec4 v_5;
					    v_5.x = _World2Object[0].x;
					    v_5.y = _World2Object[1].x;
					    v_5.z = _World2Object[2].x;
					    v_5.w = _World2Object[3].x;
					    highp vec4 v_6;
					    v_6.x = _World2Object[0].y;
					    v_6.y = _World2Object[1].y;
					    v_6.z = _World2Object[2].y;
					    v_6.w = _World2Object[3].y;
					    highp vec4 v_7;
					    v_7.x = _World2Object[0].z;
					    v_7.y = _World2Object[1].z;
					    v_7.z = _World2Object[2].z;
					    v_7.w = _World2Object[3].z;
					    highp vec3 tmpvar_8;
					    tmpvar_8 = normalize(((
					      (v_5.xyz * _glesNormal.x)
					     + 
					      (v_6.xyz * _glesNormal.y)
					    ) + (v_7.xyz * _glesNormal.z)));
					    highp float tmpvar_9;
					    tmpvar_9 = dot (tmpvar_8, normalize((_WorldSpaceLightPos0.xyz - 
					      (tmpvar_4 * _WorldSpaceLightPos0.w)
					    )));
					    highp vec4 tmpvar_10;
					    tmpvar_10.w = 1.0;
					    tmpvar_10.xyz = (tmpvar_4 - (tmpvar_8 * (unity_LightShadowBias.z * 
					      sqrt((1.0 - (tmpvar_9 * tmpvar_9)))
					    )));
					    clipPos_2 = (unity_MatrixVP * tmpvar_10);
					  } else {
					    highp vec4 tmpvar_11;
					    tmpvar_11.w = 1.0;
					    tmpvar_11.xyz = vertex_1;
					    clipPos_2 = (glstate_matrix_mvp * tmpvar_11);
					  };
					  highp vec4 clipPos_12;
					  clipPos_12.xyw = clipPos_2.xyw;
					  clipPos_12.z = (clipPos_2.z + clamp ((unity_LightShadowBias.x / clipPos_2.w), 0.0, 1.0));
					  clipPos_12.z = mix (clipPos_12.z, max (clipPos_12.z, -(clipPos_2.w)), unity_LightShadowBias.y);
					  gl_Position = clipPos_12;
					}
					
					
					#endif
					#ifdef FRAGMENT
					void main ()
					{
					  gl_FragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
					}
					
					
					#endif


Keywords { "SHADOWS_DEPTH" }
					

ENDGLSL

 }
}
Fallback "VertexLit"
}