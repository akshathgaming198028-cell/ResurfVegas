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
  Cull Off
GLSLPROGRAM

					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					attribute vec4 _glesMultiTexCoord0;
					attribute vec4 _glesMultiTexCoord1;
					uniform highp vec4 _Time;
					uniform highp mat4 glstate_matrix_mvp;
					uniform lowp vec4 _SkyGradientTopColor;
					uniform lowp vec4 _SkyGradientBottomColor;
					uniform lowp vec4 _FogSilhouetteColor;
					uniform lowp float _SkyGradientOffset;
					uniform mediump vec4 _MainTex_ST;
					uniform highp vec2 _Distort;
					uniform highp float _AmplitudeX;
					uniform highp float _AmplitudeY;
					uniform highp float _FrequenceyX;
					uniform highp float _FrequenceyY;
					varying mediump vec2 xlv_TEXCOORD0;
					varying lowp vec4 xlv_TEXCOORD1;
					void main ()
					{
					  highp vec4 tmpvar_1;
					  tmpvar_1 = _glesVertex;
					  mediump vec4 tmpvar_2;
					  tmpvar_2 = _glesMultiTexCoord1;
					  highp vec4 tmpvar_3;
					  tmpvar_3.w = tmpvar_1.w;
					  lowp float screenPosY_4;
					  lowp vec3 localPos_5;
					  highp vec4 tmpvar_6;
					  lowp vec4 tmpvar_7;
					  mediump vec3 tmpvar_8;
					  tmpvar_8.x = 0.0;
					  tmpvar_8.y = tmpvar_2.x;
					  tmpvar_8.z = tmpvar_2.y;
					  localPos_5 = tmpvar_8;
					  lowp float z_9;
					  z_9 = tmpvar_1.z;
					  lowp float frequencyY_10;
					  lowp float amplitudeY_11;
					  lowp float frequencyX_12;
					  lowp float amplitudeX_13;
					  lowp vec3 off_14;
					  off_14.z = 0.0;
					  lowp float tmpvar_15;
					  tmpvar_15 = (0.01 * z_9);
					  highp float tmpvar_16;
					  tmpvar_16 = (_AmplitudeX * localPos_5.z);
					  amplitudeX_13 = tmpvar_16;
					  lowp float tmpvar_17;
					  tmpvar_17 = (0.0001 * localPos_5.y);
					  highp float tmpvar_18;
					  tmpvar_18 = (_FrequenceyX + tmpvar_17);
					  frequencyX_12 = tmpvar_18;
					  lowp float tmpvar_19;
					  tmpvar_19 = ((localPos_5.z * 0.7) + tmpvar_15);
					  highp float tmpvar_20;
					  tmpvar_20 = sin(((
					    -(_Time.y)
					   * frequencyX_12) + tmpvar_19));
					  off_14.x = (tmpvar_20 * amplitudeX_13);
					  highp float tmpvar_21;
					  tmpvar_21 = (_AmplitudeY * localPos_5.z);
					  amplitudeY_11 = tmpvar_21;
					  frequencyY_10 = _FrequenceyY;
					  lowp float tmpvar_22;
					  tmpvar_22 = ((localPos_5.z * 0.1) + tmpvar_15);
					  highp float tmpvar_23;
					  tmpvar_23 = sin(((_Time.y * frequencyY_10) + tmpvar_22));
					  off_14.y = (tmpvar_23 * amplitudeY_11);
					  tmpvar_3.xyz = (_glesVertex.xyz + off_14);
					  tmpvar_6 = (glstate_matrix_mvp * tmpvar_3);
					  tmpvar_6.x = (tmpvar_6.x + ((tmpvar_6.z * tmpvar_6.z) * _Distort.x));
					  tmpvar_6.y = (tmpvar_6.y + ((tmpvar_6.z * tmpvar_6.z) * _Distort.y));
					  highp float tmpvar_24;
					  tmpvar_24 = (tmpvar_6.y / tmpvar_6.w);
					  screenPosY_4 = tmpvar_24;
					  tmpvar_7.xyz = mix (_SkyGradientBottomColor.xyz, _SkyGradientTopColor.xyz, vec3(clamp ((
					    (screenPosY_4 - _SkyGradientOffset)
					   / 
					    (1.0 - _SkyGradientOffset)
					  ), 0.0, 1.0)));
					  lowp float tmpvar_25;
					  highp float tmpvar_26;
					  tmpvar_26 = clamp (((1100.0 - tmpvar_6.w) / 300.0), 0.0, 1.0);
					  tmpvar_25 = tmpvar_26;
					  tmpvar_7.xyz = mix (tmpvar_7.xyz, _FogSilhouetteColor.xyz, vec3(tmpvar_25));
					  highp float tmpvar_27;
					  tmpvar_27 = clamp (((800.0 - tmpvar_6.w) / 500.0), 0.0, 1.0);
					  tmpvar_7.w = tmpvar_27;
					  gl_Position = tmpvar_6;
					  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
					  xlv_TEXCOORD1 = tmpvar_7;
					}
					
					
					#endif
					#ifdef FRAGMENT
					uniform sampler2D _MainTex;
					uniform lowp vec4 _Color;
					varying mediump vec2 xlv_TEXCOORD0;
					varying lowp vec4 xlv_TEXCOORD1;
					void main ()
					{
					  lowp vec4 c_1;
					  lowp vec4 tmpvar_2;
					  tmpvar_2 = (texture2D (_MainTex, xlv_TEXCOORD0) * _Color);
					  c_1.w = tmpvar_2.w;
					  c_1.xyz = mix (xlv_TEXCOORD1.xyz, tmpvar_2.xyz, xlv_TEXCOORD1.www);
					  gl_FragData[0] = c_1;
					}
					
					
					#endif




ENDGLSL

 }
}
}