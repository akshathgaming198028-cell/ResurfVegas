Shader "Custom/Additive Constant Color" {
Properties {
 _MainTex ("Main Texture", 2D) = "white" { }
 _MainColor ("Color (RGBC)", Color) = (1,1,1,0)
}
SubShader {
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  Blend One One
  GpuProgramID 21221
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
d
,
8
,
#version 100
#ifdef VERTEX
attribute vec4 _glesVertex;
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
 J
tmpvar_1;
 C
2
_ = ((
X.xy *
i.xy) +
0zw)K
gl_Positt
>= (
( *j
4
) =
' =
}
#endif
FRAGMENTo
l
VColor
sampler2D
j
:
 * texture2D (y
,d
)G
FragData[0]
3
.
8
P
Bend UV Scroll (Screen)Simple
2
