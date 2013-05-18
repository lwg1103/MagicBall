float4x4 gWorld;
float4x4 gView;
float4x4 gProj;
float4x4 gTrans;
float gOpacity;
Texture2D <float4> xTexture;
sampler TextureSampler;

struct VS_IN
{
	float4 pos : POSITION;
	float2 cords :TEXCOORD0;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
	float2 cords :TEXCOORD0;
};

PS_IN vs_tex(VS_IN input)
{
	PS_IN output = (PS_IN)0;
	output.pos = input.pos;
        output.cords =input.cords;
	return output;
}

float4 ps_tex(PS_IN input) : SV_Target
{
   float2 temp;
	temp  = float2(input.cords[0],input.cords[1]);

	return   xTexture.Sample(TextureSampler,temp );
}

technique11 SolidTexture
{
	pass P0
	{
		SetGeometryShader(0);
		SetVertexShader(CompileShader(vs_4_0, vs_tex()));
		SetPixelShader(CompileShader(ps_4_0, ps_tex()));
	}
}

// POINT LIGHT TECHNIQUES

float4 ps_blue_pl(PS_IN input) : SV_Target
{
	// TODO: Implement Point Light
	return float4(0, 0, 1, gOpacity);
}

technique11 PointLightBlue
{
	pass P0
	{
		SetGeometryShader(0);
		SetVertexShader(CompileShader(vs_4_0, vs_tex()));
		SetPixelShader(CompileShader(ps_4_0, ps_blue_pl()));
	}
}