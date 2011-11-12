// Matrices
float4x4 xWorld;
float4x4 xView;
float4x4 xProjection;

// Textures
sampler xTiles;

// Fog
bool xFogEnabled;
float3 xFogColor;
float xFogStart;
float xFogEnd;
float3 xCameraPosition;

// Billboard
bool xBillboardEnabled;

// Light
float3 xAmbientLightColor;

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float3 Normal : NORMAL;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float3 Normal	: TEXCOORD1;
	float Fog		: TEXCOORD2;
};

const int MAGIC_NUMBER = 666.0f;

float Fog(float4 worldPosition)
{
	return saturate((length(xCameraPosition - worldPosition) - xFogStart) / (xFogEnd - xFogStart));
}

VertexShaderOutput Billboard(VertexShaderInput input)
{
    VertexShaderOutput output;
	float2 normalizedTexCoord;

	// Normalize tex coord. E.g. (0, 0) or (0, 1)
	normalizedTexCoord.x = (input.TexCoord.x >= MAGIC_NUMBER) ? 1 : 0;
	normalizedTexCoord.y = (input.TexCoord.y >= MAGIC_NUMBER) ? 1 : 0;

	// Re-calculate tex coord for pixel shader
	output.TexCoord = input.TexCoord;
	if (output.TexCoord.x >= MAGIC_NUMBER)
		output.TexCoord.x -= MAGIC_NUMBER;
	if (output.TexCoord.y >= MAGIC_NUMBER)
		output.TexCoord.y -= MAGIC_NUMBER;

	// Go
	float3 center = mul(input.Position, xWorld);
    float3 eyeVector = center - xCameraPosition;

    float3 upVector = float3(0, 1, 0);
    upVector = normalize(upVector);
    float3 sideVector = cross(eyeVector, upVector);
    sideVector = normalize(sideVector);

    float3 finalPosition = center;
    finalPosition += (normalizedTexCoord.x - 0.5f) * sideVector;
    finalPosition += (1.0f - normalizedTexCoord.y * 1.0f) * upVector;

    float4 finalPosition4 = float4(finalPosition, 1);

	output.Fog = Fog(finalPosition4);

    float4x4 preViewProjection = mul (xView, xProjection);
    output.Position = mul(finalPosition4, preViewProjection);


	output.Normal = 0;

	return output;
}

VertexShaderOutput AllInOneVS(VertexShaderInput input)
{
    VertexShaderOutput output;
	
	if (xBillboardEnabled)
		return Billboard(input);

	// WorldViewProjection transform
    float4 worldPosition = mul(input.Position, xWorld);
    float4 viewPosition = mul(worldPosition, xView);
    output.Position = mul(viewPosition, xProjection);
	
	// Other stuff
    output.Normal = mul(input.Normal, xWorld);
	output.TexCoord = input.TexCoord;
	output.Fog = Fog(worldPosition);

    return output;
}

float4 AllInOnePS(VertexShaderOutput input) : COLOR0
{
    // Store the final color of the pixel
	float4 tex = tex2D(xTiles, input.TexCoord);
	clip(tex.a < 1.0f ? -1:1 );

    float3 finalColor = tex;

	finalColor *= xAmbientLightColor;

	// Lerp between the computed final color and the fog color
	finalColor = lerp(finalColor, xFogColor, input.Fog);

    return float4(finalColor, tex.a);
}

technique AllInOne
{
    pass Pass1
    {
		AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
		 CULLMODE = NONE;

        VertexShader = compile vs_2_0 AllInOneVS();
        PixelShader = compile ps_2_0 AllInOnePS();
    }
}
