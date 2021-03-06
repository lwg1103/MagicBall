﻿float4x4 gWorld;
float4x4 gView;
float4x4 gProj;
float4x4 gTrans;
float gOpacity;
Texture2D <float4> xTexture;
sampler TextureSampler;

//light
float3 gLightPos;
//float4 gLightAtt;
//float gLightRange;
//float4 gLightAmbient;
float4 gLightDiffuse;

float lightStrength;

struct VS_IN_TEX
{
	float4 pos : POSITION;
	float3 normal : NORMAL0;
	float2 cords :TEXCOORD0;
};

struct PS_IN_TEX
{
	float4 pos : SV_POSITION;
	float3 normal : NORMAL0;
	float2 cords :TEXCOORD0;
	float3 lightPos : TEXCOORD1;
};

PS_IN_TEX vs_tex(VS_IN_TEX input)
{
	PS_IN_TEX output = (PS_IN_TEX)0;
	input.pos.w = 1.0f;

	output.pos = mul(mul(mul(input.pos, mul(gWorld, gTrans)), gView), gProj);
        output.cords =input.cords;

	output.normal = mul(input.normal, mul(gTrans, gWorld));	
    output.normal = normalize(output.normal);

	float4 worldPosition = mul(input.pos, mul(gWorld, gTrans));

	output.lightPos = gLightPos - worldPosition.xyz;
	output.lightPos = normalize(output.lightPos);

	return output;
}

float4 ps_tex(PS_IN_TEX input) : SV_Target
{
	float lightIntensity = saturate(dot(input.normal, input.lightPos));
	float4 color = gLightDiffuse + lightIntensity * lightStrength;

	float2 temp;
	temp  = float2(input.cords[0],input.cords[1]);
	
	float4 texel = xTexture.Sample(TextureSampler,temp );

	color = saturate(color) * texel;

	return float4(color.xyz, 1.0f);
}

float4 ps_glow_tex(PS_IN_TEX input) : SV_Target
{
	float2 temp;
	temp  = float2(input.cords[0],input.cords[1]);
	
	float4 texel = xTexture.Sample(TextureSampler,temp );

	float4 color = saturate(texel * 0.3 + lightStrength*0.7);

	return float4(color.xyz, lightStrength * 0.33f);
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

technique11 GlowTexture
{
	pass P0
	{
		SetGeometryShader(0);
		SetVertexShader(CompileShader(vs_4_0, vs_tex()));
		SetPixelShader(CompileShader(ps_4_0, ps_glow_tex()));
	}
}