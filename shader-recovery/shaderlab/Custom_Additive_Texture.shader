Shader "Custom/Additive Texture" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
}
SubShader {
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  GpuProgramID 37377
Program "vp" {
SubProgram "gles " {
GpuProgramIndex 0
}
}
Program "fp" {
SubProgram "gles " {
GpuProgramIndex 1
}
}
 }
}
}
x
8
,@
#version 100
#ifdef VERTEX
attribute vec4 _glesVertex;
_Color
&MultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp'
smediumpQ
MainTex_ST;
varying low
xlv_COLOR
?
2
TEXCOORD0;
void main ()
{
 l
tmpvar_1;
) =
 |
/
*2;H
2
3H
o3 = ((
X.xy *
i.xy) +
7zw)K
92 =
gl_Posit
>= (Z
( *
K
7
) =
	/
' =
}
#endif8
FRAGMENT
sampler2D
A
(texture2D (
,W
6) *
)E
FragData[0]b
L
.
x
8
P
