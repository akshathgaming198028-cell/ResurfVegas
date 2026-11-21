Shader "Custom/Distorted/Unlit TeleboardFX" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _Scale ("Scale", Range(0,1)) = 1
 _Offset ("Offset", Range(0,1000)) = 0
 _MainColor ("Color (RGBC)", Color) = (1,1,1,0)
}
SubShader {
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  ColorMask 0
  GpuProgramID 6379
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
 Pass {
  Tags { "QUEUE"="Transparent" }
  Blend One One
  GpuProgramID 118225
Program "vp" {
SubProgram "gles " {
GpuProgramIndex 2
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
`
8
D
6#version 100
#ifdef VERTEX
attribute vec4 _glesVertex;
uniform highp
ProjectionParams&
mat4 glstate_matrix_mvpM
|Distort
float _Scale
Offset;
varying lowU
xlv_COLOR
s
2
TEXCOORD0;
void main ()
{
tmpvar_1;
 _
(2;-
2
3
9
N = (
( *e
)1
 .x3
6+ (
6z **
Ez) *@
:.x)H
yH
yH
yH
Oo_4;
5q
5o
 * 0.5)
66
6
C
).x
^5.y *z
.=
Xo_4.x/
66 +V
w$
Gzw =
wo
3
I
V
(
C
t
1
[
pgl_Posi
) =4
) =6
' =;
}
#endif
FRAGMENT>
MainColor$
sampler2D
?Tex
"dmedium
_excol
exture2D (
,o
)P
:
3
72 *
	<
FragData[0] =
Y
.
D
8
/
|
5
V
#
Q
?
'
i(1.0,
)W
P
P
ScoreBooster
g
^B
g
Bz<>Bg
^B
g
Bz<>B
?U,
BU,
BU,
B
9
B
|B
cB
9
B
|B
cB
pow1
='
'
'
R8B
dB
?B
R8B
dB
?B
L?91
B91
B91
B
}B
BW
B
}B
BW
B
pow2
=
w
B
BO
Bw
B
BO
B
*?
B
B
BGe
BGe
Bp~
BGe
BGe
Bp~
B
pow3
?
Q?
m_Enabled
pow1
?
Q?
m_Enabled
pow2
?
Q?
m_Enabled
pow3
pB
?
?
?
?
?
?
?
?
?
?
?
?
?
?
?
?
?
?
?
?
?
?
?
?
?
?
?
?
?
?
?
?
?
?
?
4
?
?
?
?
?
?
?
?
?
?
?
?
?
?
?
?
ScoreBoo
