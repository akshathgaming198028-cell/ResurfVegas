Shader "Custom/Sky" {
Properties {
 _GradientFade ("Fade", Float) = 1
 _GradientOffset ("Gradient offset", Float) = 0
}
SubShader { 
 Tags { "QUEUE"="Background+2000" "RenderType"="Opaque" }
 Pass {
  Tags { "QUEUE"="Background+2000" "RenderType"="Opaque" }
  ZWrite Off
GLSLPROGRAM

					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					uniform highp mat4 glstate_matrix_mvp;
					uniform lowp vec4 _SkyGradientTopColor;
					uniform lowp vec4 _SkyGradientBottomColor;
					uniform lowp float _SkyGradientOffset;
					varying lowp vec4 xlv_COLOR;
					void main ()
					{
					  lowp float gradPos_1;
					  highp vec4 tmpvar_2;
					  lowp vec4 tmpvar_3;
					  tmpvar_2 = (glstate_matrix_mvp * _glesVertex);
					  highp float tmpvar_4;
					  tmpvar_4 = (tmpvar_2.y / tmpvar_2.w);
					  gradPos_1 = tmpvar_4;
					  tmpvar_3.xyz = mix (_SkyGradientBottomColor.xyz, _SkyGradientTopColor.xyz, vec3(clamp ((
					    (gradPos_1 - _SkyGradientOffset)
					   / 
					    (1.0 - _SkyGradientOffset)
					  ), 0.0, 1.0)));
					  tmpvar_3.w = 1.0;
					  gl_Position = tmpvar_2;
					  xlv_COLOR = tmpvar_3;
					}
					
					
					#endif
					#ifdef FRAGMENT
					varying lowp vec4 xlv_COLOR;
					void main ()
					{
					  gl_FragData[0] = xlv_COLOR;
					}
					
					
					#endif




ENDGLSL

 }
}
SubShader { 
 Tags { "QUEUE"="Background" "RenderType"="Opaque" }
 Pass {
  Tags { "QUEUE"="Background" "RenderType"="Opaque" }
  ZWrite Off
  Cull Front
GLSLPROGRAM

					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					attribute vec4 _glesMultiTexCoord0;
					uniform highp mat4 glstate_matrix_mvp;
					uniform highp vec4 _MainTex_ST;
					varying lowp vec4 xlv_COLOR0;
					varying highp vec2 xlv_TEXCOORD0;
					void main ()
					{
					  lowp vec4 tmpvar_1;
					  mediump vec4 tmpvar_2;
					  tmpvar_2 = clamp (vec4(0.0, 0.0, 0.0, 1.1), 0.0, 1.0);
					  tmpvar_1 = tmpvar_2;
					  highp vec4 tmpvar_3;
					  tmpvar_3.w = 1.0;
					  tmpvar_3.xyz = _glesVertex.xyz;
					  xlv_COLOR0 = tmpvar_1;
					  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
					  gl_Position = (glstate_matrix_mvp * tmpvar_3);
					}
					
					
					#endif
					#ifdef FRAGMENT
					uniform sampler2D _MainTex;
					varying highp vec2 xlv_TEXCOORD0;
					void main ()
					{
					  lowp vec4 tmpvar_1;
					  tmpvar_1 = texture2D (_MainTex, xlv_TEXCOORD0);
					  gl_FragData[0] = tmpvar_1;
					}
					
					
					#endif




ENDGLSL

 }
}
Fallback "Diffuse"
}