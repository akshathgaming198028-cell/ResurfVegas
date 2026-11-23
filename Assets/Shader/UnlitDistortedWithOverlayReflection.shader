Shader "Custom/Distorted/Unlit with overlay and Reflection" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _Color ("Color Tint", Color) = (0,0,0,0)
 _ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
 _Cube ("Reflection Cubemap", CUBE) = "_Skybox" { }
}
SubShader { 
 Pass {
GLSLPROGRAM

					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					attribute vec3 _glesNormal;
					attribute vec4 _glesMultiTexCoord0;
					uniform highp vec3 _WorldSpaceCameraPos;
					uniform highp mat4 glstate_matrix_mvp;
					uniform highp mat4 _Object2World;
					uniform highp mat4 _World2Object;
					uniform lowp vec4 _SkyGradientTopColor;
					uniform lowp vec4 _SkyGradientBottomColor;
					uniform lowp vec4 _FogSilhouetteColor;
					uniform lowp float _SkyGradientOffset;
					uniform highp vec4 _Distort;
					uniform mediump vec4 _MainTex_ST;
					varying lowp vec4 xlv_COLOR;
					varying mediump vec2 xlv_TEXCOORD2;
					varying lowp vec3 xlv_TEXCOORD0;
					varying lowp vec3 xlv_TEXCOORD1;
					varying lowp vec4 xlv_TEXCOORD3;
					void main ()
					{
					  lowp float screenPosY_1;
					  lowp mat4 modelMatrixInverse_2;
					  lowp mat4 modelMatrix_3;
					  highp vec4 tmpvar_4;
					  lowp vec4 tmpvar_5;
					  lowp vec3 tmpvar_6;
					  lowp vec4 tmpvar_7;
					  modelMatrix_3 = _Object2World;
					  modelMatrixInverse_2 = _World2Object;
					  tmpvar_6 = ((modelMatrix_3 * _glesVertex).xyz - _WorldSpaceCameraPos);
					  lowp vec4 tmpvar_8;
					  tmpvar_8.w = 0.0;
					  tmpvar_8.xyz = _glesNormal;
					  tmpvar_4 = (glstate_matrix_mvp * _glesVertex);
					  tmpvar_4.x = (tmpvar_4.x + ((tmpvar_4.z * tmpvar_4.z) * _Distort.x));
					  tmpvar_4.y = (tmpvar_4.y + ((tmpvar_4.z * tmpvar_4.z) * _Distort.y));
					  highp float tmpvar_9;
					  tmpvar_9 = (tmpvar_4.y / tmpvar_4.w);
					  screenPosY_1 = tmpvar_9;
					  tmpvar_7.xyz = mix (_SkyGradientBottomColor.xyz, _SkyGradientTopColor.xyz, vec3(clamp ((
					    (screenPosY_1 - _SkyGradientOffset)
					   / 
					    (1.0 - _SkyGradientOffset)
					  ), 0.0, 1.0)));
					  lowp float tmpvar_10;
					  highp float tmpvar_11;
					  tmpvar_11 = clamp (((1100.0 - tmpvar_4.w) / 300.0), 0.0, 1.0);
					  tmpvar_10 = tmpvar_11;
					  tmpvar_7.xyz = mix (tmpvar_7.xyz, _FogSilhouetteColor.xyz, vec3(tmpvar_10));
					  highp float tmpvar_12;
					  tmpvar_12 = clamp (((800.0 - tmpvar_4.w) / 500.0), 0.0, 1.0);
					  tmpvar_7.w = tmpvar_12;
					  gl_Position = tmpvar_4;
					  xlv_COLOR = tmpvar_5;
					  xlv_TEXCOORD2 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
					  xlv_TEXCOORD0 = (tmpvar_8 * modelMatrixInverse_2).xyz;
					  xlv_TEXCOORD1 = tmpvar_6;
					  xlv_TEXCOORD3 = tmpvar_7;
					}
					
					
					#endif
					#ifdef FRAGMENT
					uniform lowp vec4 _Color;
					uniform sampler2D _MainTex;
					uniform lowp samplerCube _Cube;
					uniform lowp vec4 _ReflectColor;
					varying mediump vec2 xlv_TEXCOORD2;
					varying lowp vec3 xlv_TEXCOORD0;
					varying lowp vec3 xlv_TEXCOORD1;
					varying lowp vec4 xlv_TEXCOORD3;
					void main ()
					{
					  lowp vec4 c_1;
					  lowp vec3 tmpvar_2;
					  tmpvar_2 = normalize(xlv_TEXCOORD1);
					  lowp vec3 tmpvar_3;
					  tmpvar_3 = normalize(xlv_TEXCOORD0);
					  lowp vec4 tmpvar_4;
					  tmpvar_4 = texture2D (_MainTex, xlv_TEXCOORD2);
					  lowp vec4 tmpvar_5;
					  tmpvar_5 = ((tmpvar_4 * _Color) + ((
					    ((textureCube (_Cube, (tmpvar_2 - (2.0 * 
					      (dot (tmpvar_3, tmpvar_2) * tmpvar_3)
					    ))) * _ReflectColor) * (1.0 - clamp (-(
					      dot (tmpvar_2, tmpvar_3)
					    ), 0.0, 1.0)))
					   * 0.495) * (
					    (tmpvar_4.x + tmpvar_4.z)
					   + tmpvar_4.y)));
					  c_1.w = tmpvar_5.w;
					  c_1.xyz = mix (xlv_TEXCOORD3.xyz, tmpvar_5.xyz, xlv_TEXCOORD3.www);
					  gl_FragData[0] = c_1;
					}
					
					
					#endif




ENDGLSL

 }
}
}