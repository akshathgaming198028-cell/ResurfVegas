Shader "Particles/Additive" {
Properties {
 _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
 _MainTex ("Particle Texture", 2D) = "white" { }
 _InvFade ("Soft Particles Factor", Range(0.01,3)) = 1
}
SubShader {
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha One
  ColorMask RGB
  GpuProgramID 62330
Program "vp" {
SubProgram "gles " {
GpuProgramIndex 0
}
SubProgram "gles " {
GpuProgramIndex 1
}
SubProgram "gles " {
GpuProgramIndex 2
}
SubProgram "gles " {
GpuProgramIndex 3
}
}
Program "fp" {
SubProgram "gles " {
GpuProgramIndex 4
}
SubProgram "gles " {
GpuProgramIndex 5
}
SubProgram "gles " {
GpuProgramIndex 6
}
SubProgram "gles " {
GpuProgramIndex 7
}
}
 }
}
}
C
4
0
0
D
`
P
L
`
@
\
P
FOG_LINEAR
SOFTPARTICLES_ON
#version 100
#ifdef VERTEX
attribute vec4 _glesVertex;
_Color
MultiTexCoord0;
uniform highp(
ProjectionParams&
mat4 glstate_matrix_mvp'
odelview{
unity_Fogy
MainTex_ST;
varying low
xlv_COLOR
=
2
TEXCOORD0"
Yfloat#
1E
4"
2;
void main ()
{
 +
tmpvar_1;
2
#
N = (U
( *
)H
?o_3Z
4Z
4Z
f
Z* 0.5H
2*
56
U5.x =C
).x
yS
^4.y *5
.
Xo_3.x/
65 +V
w$
&zwn
G1.zwo
@2.xy
K
z = -((f
?
!.zq
pgl_Posi
) =
>
) =K
 :
(P
*
i.xy) +
<zw)P
(
, *
 .zG
J
72 =]
}
#endifQ
FRAGMENTY
|ZBuffer|
sampler2D
x
S_Tint.
@
CameraDepthTexture-
_InvFade
 xy
;6
2col
?
3K
3 = clampn
 * (
    (1.0/(((
=
t\
 2D
/ (w
,
1).x
K
@w)))k
--
.z)), 0.0, 1.0)
(
5w *
.);:
F(2.06
 1)
\
$ (
10))l
f
(.w
wu
u
	*
^
*
3
)s
FragData[0] =
3
8
.
`
(
P
.FF(
P
L
!
\
K
-
X
	/
	t
4	O
N
h
Y
(
A
=
O
g
(
t
<
/
7
U
K
(
H
2
2
@
3
/a
D
!
?
19
(
9
:
v
O
?2.w
O
1O
2O
3O
1
P
Mobile-Particle-Add
1
