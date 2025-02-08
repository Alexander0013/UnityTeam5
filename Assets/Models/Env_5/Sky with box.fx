float4x4 WorldViewProjMatrix : WORLDVIEWPROJECTION;
float4x4 WorldMatrix : WORLD;
float4x4 ViewMatrix	 : VIEW;
float4x4 ProjectMatrix  : PROJECTION;
float4x4 WorldViewMatrix : WORLDVIEW;

float4x4 LightWorldViewProjMatrix : WORLDVIEWPROJECTION < string Object = "Light"; >;
float3   LightDirection	: DIRECTION < string Object = "Light"; >;
float3   CameraPosition	: POSITION  < string Object = "Camera"; >;

float Time : TIME;

texture SkyMap < string ResourceName = "tex/sky.png"; >;
sampler SkyMapSampler = sampler_state
{
	texture = <SkyMap>;
	MINFILTER = LINEAR; 
    MAGFILTER = LINEAR; 
    MIPFILTER = LINEAR;
	ADDRESSU = WRAP; ADDRESSV = CLAMP;
};

texture NoiseMap < string ResourceName = "tex/noise.png"; >;
sampler NoiseMapSampler = sampler_state
{
	texture = <NoiseMap>;
	MINFILTER = LINEAR; 
    MAGFILTER = LINEAR; 
    MIPFILTER = LINEAR;
	ADDRESSU = WRAP; ADDRESSV = WRAP;
};

struct Attributes {
    float4 positionOS : POSITION;
};

struct Varyings {
	float4 positionCS		    : SV_POSITION;
    float3 positionWS           : TEXCOORD0;
};

Varyings vert(Attributes input) 
{
    Varyings output = (Varyings)0;

    float4 positionOS = input.positionOS;
    positionOS.xyz *= 300;

    float3 positionWS = mul(positionOS, WorldMatrix).xyz;
    float3 positionVS = mul(float4(positionWS, 1), ViewMatrix).xyz;
	float4 positionCS = mul(float4(positionVS, 1), ProjectMatrix);
    output.positionCS = positionCS;
    output.positionWS = positionWS;

    return output;
}

float4 frag(Varyings input) : COLOR0
{
    float3 positionWS = normalize(input.positionWS);

    const float pi = 3.141592653589793;

    float phi = 0.5 - (asin(positionWS.y) / pi);
    float theta = 0.5 + (atan2(positionWS.z, positionWS.x) / (2.0 * pi));


    float2 uv = float2(theta, phi);
    uv.x *= 3;
    uv.y *= 3.0;
    uv.y -= 0.6;

    float ry = phi;

    float2 noiseUV = float2(theta, phi);
    float noise = tex2D(NoiseMapSampler, (noiseUV + Time * 0.01) * 5).r;
    noise *= 1.0;
    noise = saturate(noise);

    float sdf = tex2D(SkyMapSampler, uv).a;
    sdf *= 2;
    sdf += sin(Time * 1.0) * noise;
    sdf = saturate(sdf);

    float3 cloud = tex2D(SkyMapSampler, uv).rgb;

    float3 backgroundColor = int3(132, 189, 247) / 255.0;
    float3 bottomColor = int3(247, 223, 148) / 255.0;

    float a = 10;
    float3 color = cloud * sdf + backgroundColor * (1 - sdf);
    color = lerp(backgroundColor, color, saturate(ry * 10));
    color = lerp(color, bottomColor, saturate((ry * 3 - 1.5) * 10));

    return float4(color, 1);
}

technique MainTec < string MMDPass = "object"; >
{
    pass DrawObject
	{
        VertexShader = compile vs_3_0 vert();
        PixelShader  = compile ps_3_0 frag();
    }
}
technique MainTec_ss < string MMDPass = "object_ss"; >
{
    pass DrawObject
	{
        VertexShader = compile vs_3_0 vert();
        PixelShader  = compile ps_3_0 frag();
    }
}
technique ShadowTec < string MMDPass = "shadow"; > { }
technique EdgeDepthTec < string MMDPass = "edge"; > { }