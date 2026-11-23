Shader "Custom/Distorted/Unlit with overlay and Rim" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _Color ("Color Tint", Color) = (0,0,0,0)
}
SubShader { 
 Pass {
GLSLPROGRAM

					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					attribute vec3 _glesNormal;
					attribute vec4 _glesMultiTexCoord0;
					uniform highp mat4 glstate_matrix_mvp;
					uniform highp mat4 glstate_matrix_modelview0;
					uniform highp mat4 glstate_matrix_invtrans_modelview0;
					uniform highp vec4 _Distort;
					uniform mediump vec4 _MainTex_ST;
					varying lowp vec4 xlv_COLOR;
					varying mediump vec2 xlv_TEXCOORD2;
					varying lowp vec3 xlv_TEXCOORD0;
					varying lowp vec3 xlv_TEXCOORD1;
					void main ()
					{
					  highp vec4 tmpvar_1;
					  lowp vec4 tmpvar_2;
					  lowp vec3 tmpvar_3;
					  lowp vec3 tmpvar_4;
					  tmpvar_4 = (glstate_matrix_modelview0 * _glesVertex).xyz;
					  lowp vec4 tmpvar_5;
					  tmpvar_5.w = 0.0;
					  tmpvar_5.xyz = _glesNormal;
					  highp vec3 tmpvar_6;
					  tmpvar_6 = (glstate_matrix_invtrans_modelview0 * tmpvar_5).xyz;
					  tmpvar_3 = tmpvar_6;
					  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
					  tmpvar_1.x = (tmpvar_1.x + ((tmpvar_1.z * tmpvar_1.z) * _Distort.x));
					  tmpvar_1.y = (tmpvar_1.y + ((tmpvar_1.z * tmpvar_1.z) * _Distort.y));
					  gl_Position = tmpvar_1;
					  xlv_COLOR = tmpvar_2;
					  xlv_TEXCOORD2 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
					  xlv_TEXCOORD0 = tmpvar_3;
					  xlv_TEXCOORD1 = tmpvar_4;
					}
					
					
					#endif
					#ifdef FRAGMENT
					uniform lowp vec4 _Color;
					uniform sampler2D _MainTex;
					varying mediump vec2 xlv_TEXCOORD2;
					varying lowp vec3 xlv_TEXCOORD0;
					varying lowp vec3 xlv_TEXCOORD1;
					void main ()
					{
					  lowp vec4 tmpvar_1;
					  tmpvar_1 = texture2D (_MainTex, xlv_TEXCOORD2);
					  lowp vec4 tmpvar_2;
					  tmpvar_2 = ((tmpvar_1 * _Color) + (vec4((1.5 * 
					    pow ((1.0 - clamp (-(
					      dot (normalize(xlv_TEXCOORD1), normalize(xlv_TEXCOORD0))
					    ), 0.0, 1.0)), 2.75)
					  )) * clamp (
					    (0.33 * ((tmpvar_1.x + tmpvar_1.z) + tmpvar_1.y))
					  , 0.0, 0.33)));
					  gl_FragData[0] = tmpvar_2;
					}
					
					
					#endif




ENDGLSL

 }
}
}