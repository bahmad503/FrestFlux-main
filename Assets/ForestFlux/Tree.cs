using System;

public class Tree
{
    // Attributes
    public float Width { get; set; }
    public float Height { get; set; }
    public string Type { get; set; }

    // Constants for the allometric equations
    private const float a = 0.1f;  // Example coefficient, replace with species-specific values
    private const float b = 2.5f;  // Example exponent for width, replace with species-specific values
    private const float c = 1.0f;  // Example exponent for height, replace with species-specific values
    private const float CarbonFraction = 0.5f;  // Typically, 50% of the biomass is carbon
    private const float RootToShootRatio = 0.2f;  // Belowground biomass is 20% of aboveground

    // Constructor to initialize the properties
    public Tree(float width, float height, string type)
    {
        Width = width;
        Height = height;
        Type = type;
    }

    // Method to calculate the aboveground biomass
    public float CalculateAboveGroundBiomass()
    {
        // Updated formula incorporating both width and height
        return a * MathF.Pow(Width, b) * MathF.Pow(Height, c);
    }

    // Method to calculate the belowground biomass
    public float CalculateBelowGroundBiomass()
    {
        return CalculateAboveGroundBiomass() * RootToShootRatio;
    }

    // Method to calculate the total biomass (aboveground + belowground)
    public float CalculateBiomass()
    {
        return CalculateAboveGroundBiomass() + CalculateBelowGroundBiomass();
    }

    // Method to calculate the carbon storage capacity
    public float CalculateCarbonStorage()
    {
        return CalculateBiomass() * CarbonFraction;
    }
}
