Shader "Hidden/CrossfadeCamera"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_CrossfadeTexture ("Texture", 2D) = "white" {}
		_Alpha ("Alpha", FLOAT) = 1
	}

	SubShader
	{
		ZTest Always 
		Cull Off 
		ZWrite Off 
		Fog { Mode Off }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#pragma target 3.0

			struct v2f
			{
				float4 pos   : SV_POSITION;
				float2 uv[2] : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;

			sampler2D _CrossfadeTexture;
			half _Alpha;

			v2f vert(appdata_img IN)
			{
				v2f OUT;
				OUT.pos = mul(UNITY_MATRIX_MVP, IN.vertex);

				OUT.uv[0] = IN.texcoord.xy;
				OUT.uv[1] = IN.texcoord.xy;
				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					OUT.uv[0].y = 1-OUT.uv[0].y;
				#endif		
				return OUT;
			}

			half4 frag(v2f IN) : SV_Target
			{
				half3 tex = tex2D(_MainTex, IN.uv[1]).rgb;
				half3 overlay = tex2D(_CrossfadeTexture, IN.uv[0]).rgb;

				half3 blend = (tex * (1.0 - _Alpha)) + (overlay * _Alpha);

				return half4(blend, 1);
			}
			ENDCG
		}
	}
}
