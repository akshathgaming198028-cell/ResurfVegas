Shader "Hidden/Internal-GUITextureBlit" {
Properties {
 _MainTex ("Texture", any) = "white" { }
}
SubShader {
 Tags { "ForceSupported"="true" }
 Pass {
  Tags { "ForceSupported"="true" }
  ZTest Always
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  GpuProgramID 45730
Program "vp" {
SubProgram "gles " {
GpuProgramIndex 0
}
SubProgram "gles3 " {
GpuProgramIndex 1
}
}
Program "fp" {
SubProgram "gles " {
GpuProgramIndex 2
}
SubProgram "gles3 " {
GpuProgramIndex 3
}
}
 }
}
}
^
8
L
$
8
n
'
#ifdef VERTEX
#version 300 es
uniform 	vec4 _Time;
?Sin
?Cos
unity_Delta
3 _WorldSpaceCameraPos[
ProjectionParam!
_Scree
ZBuffer
_Ortho!
ClipPlanes[6].
Zmat4x0
(
?Inv+
"
LightPos0$
iB
ORang
4)
/X0!
Y!
Z!
mediump )
_Atten*
)
Color[8]v
&
Q
z
Q
SpotDirecQ
OSHAr"
g"
b"
Bf
Bf
Bf
C!
3
0)
1)
2)
3
hadowSplitSpheres[4]+
oqRadii[
-
OBias[
G
_sNear
	?Far
2g
/[4o
OData
FadeCenterAndType|
glstate_matrix_mvp$
odelview0+
invtrans4
0_Ob
2
%
 OD
?
T
Opose
p
3low
.
l
W
_ambient
MX
V
Pn
#
Aa
?Sky%
Equator)
oGround(
1Fog
?FogI
_mapST)
oynamic'
SpecCube0_BoxMax&
/in&
TProbef
F
5
?HDRX
1
1
1
!
1
a
OGrey)
oDouble
.
ielectric
6
Luminance
;map
*
\
_MainTex_ST
0GUI
rTexture
;
in high=
in_POSITION0
X
in_COLOR04
2
TEXCOORD0;
out5
%vs5
6
,vs6
11;
J
!t0
02 t
oid main()
{
    t0 =
.yyyy *`
F[1];4
J0] *L
axxxx +
9
29
Gzzzz9
4gl_
B
3B
GwwwwB
Q
( =
Q
'
#
@
@
t1.xy = t0.
d[1].xyq
1
)
0)
! *&
19
29
9
C
3C
/wwC
*in
*
+
"zwI
return;
}
#endif
s
FRAGMENTu
Qpreci
mfloat;
iJ
sampler2D
!
~
.vs~
H
1;
layout(loca
:0)
SV_Target0v
d3 t10_
+
@.xyz
(
,
))
`
5
A
*
(
y
\
>
,p
5).wa
w_
1[
P
1
8
S
E
Z100
"
attribute
glesVertl
MultiTexCoordp
T
I
'
w
N
wvarying
2xlv
-
"
(
( *
)?
) =
P
(
)
)P
(8
O * (
)}
?}
*
a
j
0col
w
(Q
72D
	w
)
)I
_
(
w
U
").
pgl_Frag`
~
=
/
L
P
Soft
UU
UU
  !!!!!!
 "$&()*+,------,+*)(&$"
 #&),.035689:;<<<<;:986530.,)&#
!%),/369<?BDFHIJKLLLLKJIHFDB?<963/,)%!
 $)-159=AEILORUWY[\^^^^^^\[YWUROLIEA=951-)$
"'+05:?DIMRVZ^aegjlmoppqqppomljgea^ZVRMID?:50+'"
#(-28>DJOTY^chlptwz|
|zwtplhc^YTOJD>82-(#
