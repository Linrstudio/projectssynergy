
struct VSin
{
    float3 Position :POSITION;
    float2 UV		:TEXCOORD;
	float4 Color	:COLOR;
};

struct PSin
{
    float4 Position	:POSITION;
    float2 UV		:TEXCOORD;
	float4 Color	:TEXCOORD1;
};

texture2D DiffuseMap;
sampler2D DiffuseSampler=sampler_state{Texture = <DiffuseMap>;MinFilter=Linear;MagFilter=Linear;MipFilter=Linear;};

texture2D WobblerMap;
sampler2D WobblerSampler=sampler_state{Texture = <WobblerMap>;MinFilter=Point;MagFilter=Point;MipFilter=Point; BorderColor=float4(0,0,0,0); 
AddressU=CLAMP;
AddressV=CLAMP;
};

sampler2D WobblerResultSampler=sampler_state{Texture = <WobblerMap>;MinFilter=Linear;MagFilter=Linear;MipFilter=Point; BorderColor=float4(0,0,0,0); 
AddressU=CLAMP;
AddressV=CLAMP;
};

float2 RenderSize;

PSin MainVS(VSin input)
{
    PSin output;
	output.Position=float4(0,0,1,1);
	output.Position.xy=input.Position.xy;
	output.UV=input.UV+float2(0.05,0.05)/RenderSize;
	output.Color=input.Color;
    return output;
}

float4 WobblePS(PSin input):COLOR
{
	float4 c=tex2D(WobblerSampler,input.UV);
	float4 l=tex2D(WobblerSampler,input.UV+float2(-8,0)/RenderSize);
	float4 u=tex2D(WobblerSampler,input.UV+float2(0,-8)/RenderSize);
	float4 r=tex2D(WobblerSampler,input.UV+float2(8,0)/RenderSize);
	float4 d=tex2D(WobblerSampler,input.UV+float2(0,8)/RenderSize);
	
	float targetheight = (l.x+r.x+u.x+d.x)/4;
	float height = c.x;
	float speed = c.y;
	
	speed+=(targetheight-height)/4;
	height+=speed;
	height*=0.98f;//friction
	
	float nx=l.x-r.x;
	float ny=d.x-u.x;
	
	return float4(height,speed,nx,ny);
}

float4 MainPS(PSin input):COLOR
{
	float4 wobble=tex2D(WobblerResultSampler,input.UV);
	float3 normal = normalize(float3(wobble.zw,0.5));

	//float4 diffuse=tex2D(DiffuseSampler,input.UV+(wobble.zw*2-1)/(RenderSize/10));
	float4 diffuse=tex2D(DiffuseSampler,input.UV+normal.xy/4);
	diffuse*=normal.z;
	float4 specular = float4(1,1,1,1)*pow(dot(normal,normalize(float3(-0.5,0.5,4))),256);
	diffuse+=specular;
	//return wobble;//debug
	return diffuse;
}

technique Main
{
    pass Pass1
    {
        VertexShader= compile vs_2_0 MainVS();
        PixelShader = compile ps_2_0 MainPS();
    }
}

technique Wobbler
{
    pass Pass1
    {
        VertexShader= compile vs_2_0 MainVS();
        PixelShader = compile ps_2_0 WobblePS();
    }
}