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

// Billboard
bool xBillboardEnabled;
float3 xCameraPosition;
float3 xAllowedRotDir;

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

VertexShaderOutput Billboard(VertexShaderInput input)
{
	VertexShaderOutput output;

	// Work out what direction we are viewing the billboard from.
    float3 viewDirection = xView._m02_m12_m22;

    float3 rightVector = normalize(cross(viewDirection, input.Normal));

    // Calculate the position of this billboard vertex.
    float3 position = input.Position; 

    // Offset to the left or right.
    position += rightVector * (input.TexCoord.x - 0.5);
    
    // Offset upward if we are one of the top two vertices.
    position += input.Normal * (1 - input.TexCoord.y);

    output.Position = float4(position, 1);
    output.TexCoord = input.TexCoord;
	output.Fog = 0;
	output.Normal = input.Normal;
    
    return output;
}

VertexShaderOutput NewBillboard(VertexShaderInput input)
{
    VertexShaderOutput output;

    float width = 5;
    float height = 5;


    // Work out what direction we are viewing the billboard from.
    float3 viewDirection = xView._m02_m12_m22;

    float3 rightVector = normalize(cross(viewDirection, input.Normal));

    // Calculate the position of this billboard vertex.
    float3 position = input.Position;

	float x = (input.TexCoord.)

    // Offset to the left or right.
    position += rightVector * (input.TexCoord.x - 0.5) * width;
    
    // Offset upward if we are one of the top two vertices.
    position += input.Normal * (1 - input.TexCoord.y) * height;

    // Apply the camera transform.
    float4 viewPosition = mul(float4(position, 1), xView);

    output.Position = mul(viewPosition, xProjection);

    output.TexCoord = input.TexCoord;
    output.Fog = 0;
	output.Normal = 0;
    
    return output;
}

VertexShaderOutput AllInOneVS(VertexShaderInput input)
{
    VertexShaderOutput output;
	
	/*if (xBillboardEnabled)
	{
		output = Billboard(input);
		input.Position = output.Position;
	}

    float4 worldPosition = mul(input.Position, xWorld);
    float4 viewPosition = mul(worldPosition, xView);
    output.Position = mul(viewPosition, xProjection);
    output.Normal = mul(input.Normal, xWorld);
	output.TexCoord = input.TexCoord;

    // Calculate fog value
    output.Fog = saturate((length(xCameraPosition - worldPosition) - xFogStart) / (xFogEnd - xFogStart)) * xFogEnabled;
	*/


    return NewBillboard(input);
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

        VertexShader = compile vs_2_0 AllInOneVS();
        PixelShader = compile ps_2_0 AllInOnePS();
    }
}
