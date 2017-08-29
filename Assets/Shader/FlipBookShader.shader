//
// created by jiadong chen
// http://www.chenjd.me
//

Shader "chenjd/FlipBookShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_BackTex ("BackPage", 2D) = "white" {}
		_CurPageAngle ("CurPageAngle", Range(0,1)) = 0 
	}

	SubShader
	{
			
		Tags { "RenderType"="Opaque" }
		LOD 100

		CGINCLUDE

		#include "UnityCG.cginc"
		#define pi 3.1415926

		sampler2D _MainTex;
		float4 _MainTex_ST;

		sampler2D _BackTex;
		float4 _BackTex_ST;

		float _CurPageAngle;

		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2f
		{
			float2 uv : TEXCOORD0;
			UNITY_FOG_COORDS(1)
			float4 vertex : SV_POSITION;
		};


		float4 flip_book(float4 vertex)
		{
			float4 temp = vertex;

			float theta = _CurPageAngle * pi;

			float flipCurve = exp(-0.1 * pow(vertex.x - 0.5, 2)) * _CurPageAngle;

			theta += flipCurve;

			temp.x = vertex.x * cos(clamp(theta, 0, pi));
			temp.y = vertex.x * sin(clamp(theta, 0, pi));

			vertex = temp;

			return vertex;
		}

		v2f vert_flip (appdata v)
		{
			v2f o;
			o.uv = TRANSFORM_TEX(v.uv, _MainTex);
			o.uv.xy = 1 - o.uv.xy;

			float4 vertex = o.uv.x <= 0.5 ? v.vertex : flip_book(v.vertex);
			o.vertex = UnityObjectToClipPos(vertex);

			return o;
		}

		v2f vert_next_page(appdata v)
		{
		    v2f o;
		    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		    o.uv.y = 1 - o.uv.y;

			o.vertex = UnityObjectToClipPos(v.vertex);

		    return o;
		}
		
		fixed4 frag_flip (v2f i) : SV_Target
		{
			fixed4 col = tex2D(_MainTex, i.uv);
			UNITY_APPLY_FOG(i.fogCoord, col);
			return col;
		}

		fixed4 frag_flip_back (v2f i) : SV_Target
		{
			i.uv.x = 1 - i.uv.x;
			fixed4 col = tex2D(_BackTex, i.uv);
			return col;
		}

		ENDCG


		//用3个pass来实现翻书的效果
		//第一页
		Pass
		{
			Cull Back

			CGPROGRAM
			#pragma vertex vert_flip
			#pragma fragment frag_flip
			
			ENDCG
		}

		//翻起来的背面
		Pass
		{
			Cull Front
			Offset -1, -1

			CGPROGRAM
			#pragma vertex vert_flip
			#pragma fragment frag_flip_back
			ENDCG
		}

		//第二页
		Pass
		{
			Cull Back
			Offset 1, 1

			CGPROGRAM

			#pragma vertex vert_next_page
			#pragma fragment frag_flip_back

			ENDCG
		}

	}
}
