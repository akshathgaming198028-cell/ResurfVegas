#version 100
#ifdef VERTEX
attribute vec4 _glesVertex;
_Color
7MultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
varying mediumpQ
xlv_COLOR
G
Avec2
TEXCOORD0;
void main ()
{
  gl_Posit
>= (v
( *
F);
 r
) =
 n
/ =
.xy;
}
#endifd
FRAGMENT
sampler2D _MainTex
B	K
lcol_1;
`mask_2
3low
tmpvar_3
 = texture2D (
,
A
R
) =G
	t
`
4`
4 = clamp (ceil((
- 0.5)), 0.0, 1.0)Q
5Q
5Q
((
    (k
*R
 1)
-b
) / -0.49k
9
F.xyz
5
(
*
<
5w =
s5.w * (0
C.x +E
T.y) +H
&.z
Rw)));
FragData[0] =
p
?
P
