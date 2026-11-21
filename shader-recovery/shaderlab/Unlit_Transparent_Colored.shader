Shader "Unlit/Transparent Colored" {
Properties {
 _MainTex ("Base (RGB), Alpha (A)", 2D) = "black" { }
}
SubShader {
 LOD 200
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  Offset -1, -1
  GpuProgramID 43944
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
SubShader {
 LOD 100
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  ColorMask RGB
  Offset -1, -1
  GpuProgramID 71517
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
q
8
T
,
#version 100
#ifdef VERTEX
attribute vec4 _glesVertex;
_Color
/MultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
mediumpI
2 tmpvar_1;
varying
xlv_TEXCOORD0$
Blowp
!
COLOR;
void main ()
{
g
E2;
/ =
7.xy%
91 =<
gl_Posit]
>= (
( *S
)4
' =
) =o
}
#endif
FRAGMENTf
sampler2D _MainTexD
A
&
(texture2D (
,t
6) *d
)G
FragData[0]
#
.
8
8
/j
8
'
O
?_ST
\
4X
vclamp (
, 0.0, 1.0)^
Y
3.
P3.w =C
Xxyz =
	[
t
/((
( *`
i.xy) +
?zw)
)
$
V
(
0
P
LilitaOne-LoadScene
?
M
A
=
m
DSIG
m
FFTM_
|
k
GDEF
k
 GPOS
Y
k@
RGSUBl
t
m
 OS/2fO
`cmap5
gasp
j
glyf
?H
	d
Y
head
W
6hhea
T
$hmtx
!
locag_}
maxp
-
P
x
 name
b
xpost
Z@c
h
preph
)
_
<
-
-
$
$
M
@
2
/@
J
PYRS
@
%
M
-
A
A
Z
"
#
,
-
F
_
_
-
Q
Z
%
1
5
#
=
#
+
V
+
?
#
:
5
(
:
4
D
C
3
H
B
c
'
R
=
1
h
2
2
-
m
2
l
2
(
2
2
:
.
N
2
t
g
3
%
P
K
,
|
/
^
G
2
5
w
-
-
-
-
'
+
p
(
-
j
t
j
P
-
#
W
i
(
&
T
P
Y
e
C
/
T
(
S
C
`
7
1
e
G
<
(
5
:
d
)
n
(
Y
G
Q
P
S
d
E
H
A
"
"
-
-
-
-
l
+
b
e
t
t
t
t
g
S
.
R
>
(
'
c
&
(
(
(
(
t
\
&
}
m
g
3
%
H
H
p
/
C
U
Y
:
F
p
5
4
Y
F
5
%
%
e
S
:
#
Y
Y
9
>
6
#
5
[
C
4
~
1
B
S
a
x
~
 " & 0 : D
!""
1
A
R
`
x
}
   & 0 9 D
!""
q
e
O
K
8
3
 !"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`a
pcdh
v
oj
ti
q
fu
kz
bm
l{
w
x
P
`
J
$
T
@
x
<
`
J
x
@
v
&
H
h
	*	N	p
$
T
z
Z
8
h
<
j
<
^
@
z
T
:
n
P
&
l
T
d
@
:
t
B
|
4
n
&
n
T
D
~
0
v
T
H
J
 8 z
!F!
!
!
!
"""f"
"
#
#D
