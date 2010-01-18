
struct VSin
{
    float4 Position :POSITION;
    float2 UV		:TEXCOORD;
};

struct PSin
{
    float4 Position	:POSITION;
    float2 UV		:TEXCOORD;
};

texture2D DiffuseMap;
sampler2D DiffuseSampler=sampler_state{Texture = <DiffuseMap>;MinFilter=Linear;MagFilter=Linear;MipFilter=Linear;};

float2 Scale;

PSin MainVS(VSin input)
{
    PSin output;
    //output.Position = mul(float4(input.Position.xyz,1),mul(World,mul(View,Projection)));
	output.Position=input.Position*2-1;
	output.Position.xy*=Scale;
	output.UV=input.UV;
    return output;
}

float4 MainPS(PSin input):COLOR
{
	float4 diffuse=tex2D(DiffuseSampler,input.UV);
	return diffuse;
}

technique Main
{
    pass Pass1
    {
        VertexShader= compile vs_3_0 MainVS();
        PixelShader = compile ps_3_0 MainPS();
    }
}