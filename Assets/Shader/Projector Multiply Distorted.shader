Shader "Custom/Distorted/Projector Multiply" {
Properties {
 _ShadowTex ("Cookie", 2D) = "gray" { }
 _Falloff ("Falloff distance", Float) = 0.01
}
SubShader { 
 Tags { "RenderType"="Transparent-1" }
 Pass {
  Tags { "RenderType"="Transparent-1" }
  ZWrite Off
  Blend DstColor Zero
  ColorMask RGB
  Offset -1, -1
GLSLPROGRAM

					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					uniform highp mat4 glstate_matrix_mvp;
					uniform highp vec4 _Distort;
					uniform lowp float _Falloff;
					uniform mediump mat4 _Projector;
					varying mediump vec4 xlv_TEXCOORD0;
					varying lowp vec4 xlv_COLOR;
					void main ()
					{
					  highp float neg_1;
					  highp vec4 tmpvar_2;
					  mediump vec4 tmpvar_3;
					  lowp vec4 tmpvar_4;
					  tmpvar_2 = (glstate_matrix_mvp * _glesVertex);
					  tmpvar_2.x = (tmpvar_2.x + ((tmpvar_2.z * tmpvar_2.z) * _Distort.x));
					  tmpvar_2.y = (tmpvar_2.y + ((tmpvar_2.z * tmpvar_2.z) * _Distort.y));
					  tmpvar_3 = (_Projector * _glesVertex);
					  mediump float tmpvar_5;
					  tmpvar_5 = float((tmpvar_3.z < 0.0));
					  neg_1 = tmpvar_5;
					  mediump float tmpvar_6;
					  tmpvar_6 = clamp ((tmpvar_3.z * _Falloff), 0.0, 1.0);
					  highp vec4 tmpvar_7;
					  tmpvar_7 = vec4((neg_1 + tmpvar_6));
					  tmpvar_4 = tmpvar_7;
					  gl_Position = tmpvar_2;
					  xlv_TEXCOORD0 = tmpvar_3;
					  xlv_COLOR = tmpvar_4;
					}
					
					
					#endif
					#ifdef FRAGMENT
					uniform sampler2D _ShadowTex;
					varying mediump vec4 xlv_TEXCOORD0;
					varying lowp vec4 xlv_COLOR;
					void main ()
					{
					  lowp vec4 tmpvar_1;
					  tmpvar_1 = mix (texture2DProj (_ShadowTex, xlv_TEXCOORD0), vec4(1.0, 1.0, 1.0, 1.0), xlv_COLOR.xxxx);
					  gl_FragData[0] = tmpvar_1;
					}
					
					
					#endif




ENDGLSL

 }
}
}