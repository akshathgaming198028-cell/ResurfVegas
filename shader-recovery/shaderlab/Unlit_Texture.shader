Shader "Unlit/Texture" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
}
SubShader {
 LOD 100
 Tags { "RenderType"="Opaque" }
 Pass {
  Tags { "RenderType"="Opaque" }
  GpuProgramID 35353
Program "vp" {
SubProgram "gles " {
GpuProgramIndex 0
}
SubProgram "gles " {
GpuProgramIndex 1
}
}
Program "fp" {
SubProgram "gles " {
GpuProgramIndex 2
}
SubProgram "gles " {
GpuProgramIndex 3
}
}
 }
}
}
$
0
H
8
H
,t
#version 100
#ifdef VERTEX
attribute vec4 _glesVertex;
&MultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp'
O
MainTex_ST;
varying medium"
2 xlv_TEXCOORD0;
void main ()
{
 -
tmpvar_1;
_ = ((
X.xy *
i.xy) +
0zw)K
gl_Posit?
>= (
( *5
4
' =
}
#endif
FRAGMENT"
sampler2D
$3lowj
2col
@.xyz
exture2D (i
,T
)*
7
sw = 1.0
FragData[0] =b
.
FOG_LINEAR
unity_FogParams
4
$
D
Yfloat#
1
p
2
=
K
>
1
2
:
L.z *
 .z}
/w){
2
oColor;
G
?
yclamp (]
`, 0.0,
)
Zmix (
,
,
3
?2))0
I
0
H
/
8
P
Unlit - Transparent Colored
