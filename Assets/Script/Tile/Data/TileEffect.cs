using System;

[Flags] 
public enum TileEffect
{
    Default = 0,
    Special_Clear_Row = 1 << 0,
    Special_Clear_Column = 1 << 1,
    Special_Clear_Area = 1 << 2,
    Special_Clear_Color = 1 << 3
}