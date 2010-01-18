
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
sampler2D DiffuseSampler=sampler_state
{
Texture = <DiffuseMap>;
AddressU  = CLAMP;        
AddressV  = CLAMP;
//AddressW  = WRAP;
MinFilter=Linear;
MagFilter=Linear;
MipFilter=Linear;
};

float SampleSize=0.0f;

PSin MainVS(VSin input)
{
    PSin output;
    //output.Position = mul(float4(input.Position.xyz,1),mul(World,mul(View,Projection)));
	output.Position=input.Position*float4(1,-1,1,1);
	output.UV=input.UV;
    return output;
}

float4 MainPS(PSin input):COLOR
{
	float4 diffuse=tex2D(DiffuseSampler,input.UV);
	float4 ret = diffuse;
	
	float4 UV = float4(input.UV,0,2);
	
	float4 l=tex2Dlod(DiffuseSampler,UV+float4(-SampleSize,0,0,1));
	float4 r=tex2Dlod(DiffuseSampler,UV+float4(SampleSize,0,0,2));
	float4 u=tex2Dlod(DiffuseSampler,UV+float4(0,-SampleSize,0,3));
	float4 d=tex2Dlod(DiffuseSampler,UV+float4(0,SampleSize,0,4));
	l.rgb*=l.a;	
	u.rgb*=u.a;
	r.rgb*=r.a;
	d.rgb*=d.a;
	
	float bleed = max(0,(l.a+r.a+u.a+d.a)-diffuse.a*4);
	
	ret = saturate(diffuse+float4(1,0,0,0.5)*bleed*bleed*8);

	return ret;
}

technique Main
{
    pass Pass1
    {
        VertexShader= compile vs_3_0 MainVS();
        PixelShader = compile ps_3_0 MainPS();
    }
}