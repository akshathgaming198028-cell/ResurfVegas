Shader "Custom/Distorted/Unlit with overlay and Rim and SPmask" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _Mask ("Mask (RGB)", 2D) = "white" { }
 _Color ("Color Tint", Color) = (0,0,0,0)
 _RimColor ("Reflection Color", Color) = (1,1,1,0.5)
}
SubShader {
 Pass {
  GpuProgramID 30570
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
<
P
8
"
,
#version 100
#ifdef VERTEX
attribute vec4 _glesVertex;
3
Normal;8
&MultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp'
odelviewU
invtrans7
Clowp
SkyGradientTopColor(
oBottom+
FogSilhouette'
YfloatS
lOffset
O
gDistor
dmedium
MainTex_ST;
varying
xlv_COLOR
?
2
TEXCOORD2A
3!
0!
1!
d
3
!
4;
void main ()
{
 5
screenPosY_1;
 )
tmpvar_2;2
3
3
4
5Y
2
6Y
7
9
O = (
( *Y
_).xyzR
8R
8.w = 0.0
Zxyz =
95
9
#
% *f
,
/ =Y
2
+vp
1
 .x3
6+ (
z
e2.z) *
:.x)H
yH
yH
yH
6
/9
w*
r
*
 = clamp.
!6,L
p(-1.0,
)
A
6 * 0.5) +
&
1$
/10
I
/ =E
7i
_mix (
#
,F
(
y(
    (z
-
)(
"/ .
?1.0%
E), 0>
*);*
10
20
2
a((1100w
4
@ / 3
t
A
E
2E
,)
5
Y
)
3
3
8
5
7j
	Z
gl_Posit5
=_
V
) =a
	N
X.xy *
0.xy
<zw)P
	x
5
	m
' =
}
#endif
FRAGMENT
sampler2D
&sk
bool _DebugShaders
e
H
1
a2 mask
, =~
w
exture2D (
,R
Iif (
 )
}
c} else$
c/
+
(
,~
K
)L
8c_4H
65.w
2
!
"ww
}
FragData[0]
3
.
<
8
P
Projector Multiply Distorted
