Shader "Bend/UV Scroll (Screen)/Simple" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _ScrollSpeed ("Scroll speed (xy)", Vector) = (1,1,0,0)
}
SubShader { 
 Tags { "QUEUE"="Geometry" }
 Pass {
  Tags { "QUEUE"="Geometry" }
GLSLPROGRAM

					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					attribute vec4 _glesMultiTexCoord0;
					uniform highp vec4 _Time;
					uniform highp mat4 glstate_matrix_mvp;
					uniform lowp vec4 _SkyGradientTopColor;
					uniform lowp vec4 _SkyGradientBottomColor;
					uniform lowp vec4 _FogSilhouetteColor;
					uniform lowp float _SkyGradientOffset;
					uniform highp vec4 _MainTex_ST;
					uniform highp vec4 _ScrollSpeed;
					varying highp vec2 xlv_TEXCOORD0;
					varying lowp vec4 xlv_TEXCOORD1;
					void main ()
					{
					  lowp float screenPosY_1;
					  lowp vec4 tmpvar_2;
					  highp vec4 pos_3;
					  highp vec4 tmpvar_4;
					  tmpvar_4 = (glstate_matrix_mvp * _glesVertex);
					  pos_3.zw = tmpvar_4.zw;
					  highp float tmpvar_5;
					  tmpvar_5 = (tmpvar_4.z * tmpvar_4.z);
					  pos_3.x = (tmpvar_4.x + (tmpvar_5 * -0.0005));
					  pos_3.y = (tmpvar_4.y + (tmpvar_5 * -0.0003));
					  highp float tmpvar_6;
					  tmpvar_6 = (pos_3.y / tmpvar_4.w);
					  screenPosY_1 = tmpvar_6;
					  tmpvar_2.xyz = mix (_SkyGradientBottomColor.xyz, _SkyGradientTopColor.xyz, vec3(clamp ((
					    (screenPosY_1 - _SkyGradientOffset)
					   / 
					    (1.0 - _SkyGradientOffset)
					  ), 0.0, 1.0)));
					  lowp float tmpvar_7;
					  highp float tmpvar_8;
					  tmpvar_8 = clamp (((1100.0 - tmpvar_4.w) / 300.0), 0.0, 1.0);
					  tmpvar_7 = tmpvar_8;
					  tmpvar_2.xyz = mix (tmpvar_2.xyz, _FogSilhouetteColor.xyz, vec3(tmpvar_7));
					  highp float tmpvar_9;
					  tmpvar_9 = clamp (((800.0 - tmpvar_4.w) / 500.0), 0.0, 1.0);
					  tmpvar_2.w = tmpvar_9;
					  gl_Position = pos_3;
					  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy + (
					    (_Time.yy * _ScrollSpeed.xy)
					   * _MainTex_ST.xy)) + _MainTex_ST.zw);
					  xlv_TEXCOORD1 = tmpvar_2;
					}
					
					
					#endif
					#ifdef FRAGMENT
					uniform sampler2D _MainTex;
					varying highp vec2 xlv_TEXCOORD0;
					varying lowp vec4 xlv_TEXCOORD1;
					void main ()
					{
					  lowp vec4 c_1;
					  lowp vec4 tmpvar_2;
					  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD0);
					  c_1.w = tmpvar_2.w;
					  c_1.xyz = mix (xlv_TEXCOORD1.xyz, tmpvar_2.xyz, xlv_TEXCOORD1.www);
					  gl_FragData[0] = c_1;
					}
					
					
					#endif




ENDGLSL

 }
}
SubShader { 
 Pass {
GLSLPROGRAM

					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					attribute vec4 _glesMultiTexCoord0;
					uniform highp mat4 glstate_matrix_mvp;
					uniform highp vec4 _MainTex_ST;
					varying highp vec2 xlv_TEXCOORD0;
					void main ()
					{
					  highp vec4 pos_1;
					  highp vec4 tmpvar_2;
					  tmpvar_2 = (glstate_matrix_mvp * _glesVertex);
					  pos_1.zw = tmpvar_2.zw;
					  highp float tmpvar_3;
					  tmpvar_3 = (tmpvar_2.z * tmpvar_2.z);
					  pos_1.x = (tmpvar_2.x + (tmpvar_3 * -0.0005));
					  pos_1.y = (tmpvar_2.y + (tmpvar_3 * -0.0003));
					  gl_Position = pos_1;
					  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
					}
					
					
					#endif
					#ifdef FRAGMENT
					uniform sampler2D _MainTex;
					uniform mediump vec4 _Color;
					varying highp vec2 xlv_TEXCOORD0;
					void main ()
					{
					  lowp vec4 tmpvar_1;
					  tmpvar_1 = texture2D (_MainTex, xlv_TEXCOORD0);
					  gl_FragData[0] = (tmpvar_1 * _Color);
					}
					
					
					#endif




ENDGLSL

 }
}
Fallback "Diffuse"
}