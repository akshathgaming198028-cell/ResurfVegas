Shader "Custom/Particles/AlphaBlendedOverlay" {
Properties {
 _MainTex ("Particle Texture", 2D) = "white" { }
}
SubShader {
 Tags { "QUEUE"="Overlay" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Overlay" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  GpuProgramID 48638
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
v
P
H
D
l
8
$
H
FOG_LINEAR
H
/
8
,j
#version 100
#ifdef VERTEX
attribute vec4 _glesVertex;
_Color
&MultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp'
O
MainTex_ST;
varying low
xlv_COLOR0
>
2
TEXCOORD"
oid main ()
{
 I
tmpvar_1;
  medium
2
 = clamp (
, 0.0, 1.0)+
91 =B
Y
3.
P3.w =C
Xxyz =
"
) =
_ = ((
X.xy *`
i.xy) +
!zw
gl_Posit9
>= (
% *
);
}
#endifb
FRAGMENT
sampler2Do
V
(texture2D (
,W
7) *
FragData[0]{
?
$
/
'
odelview0
unity_FogParams
P
@
Yfloat"
1J
M
3K
E
2
{
	2
p
$2)
4
5h
5
94 =B
g
6
7F
7q
((
    sqrt(dot (
,u
)#
*L
 .z
/w)
6
7
8.
8
8
4
A
l
6
8
G
\
b
k
2col
&
>
Zmix (
,%
,
(
)G
D
P
