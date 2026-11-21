Shader "Custom/Distorted/FlyingCarpet" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _Color ("Color (RGBA)", Color) = (1,1,1,1)
 _Frequency ("Frequency", Range(0.01,5)) = 4
}
SubShader {
 Pass {
  Cull Off
  GpuProgramID 34167
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
a
L
t
8
8
,9
#version 100
#ifdef VERTEX
attribute vec4 _glesVertex;
_Color
MultiTexCoord0;
uniform highp(
LTime
mat4 glstate_matrix_mvpA
MainTex_ST
float _Frequency@
2 _Distort;
varying
xlv_TEXCOORD0;
void main ()
{
tmpvar_1;
* =?
Z  low0
20
92 =S
`
30
U3.w =m
/.w2
4
intensity_5
N
92.xN
{3 off_6
-
7
7.z = 0.0
(xy
/xzH
84
8 = abs(((
    fract(
A  ((9
A.x *C
`(50.0,
w0.0)) *
)8
#- @
Tdot (
,C
1B
)2
) / 9.42477) + 0.5
* 2.0) -+
&.x
(((d
"*
5(((
*
(3.0 - (2.0
#))w
?
z
.y * 0.3
@3.xy
(
+3
),
4(
{
3.
(
6+ (
z7
14.z
n
:.x)H
yH
yH
yH
gl_Posit
=
*K
0.xy
zw);
}
