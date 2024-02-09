using System.Collections.Generic;

//Note:Due to the simplest nature of the game and the fact that no effect needs to exist per tile instance,
//it is not necessary to implement the factory pattern, as this would only make maintenance more difficult,
//would not be the most optimized option and would involve a longer process for implementation of new effects.
//Therefore, the use of a mapping with cached effects is enough to facilitate implementation
public class TileEffectFactory
{
    private readonly Dictionary<TileEffect, ITileEffect> _effectsMapping = new Dictionary<TileEffect, ITileEffect>();

    public TileEffectFactory()
    {
        _effectsMapping[TileEffect.Special_Clear_Column] =  new ClearColumnEffect();
        _effectsMapping[TileEffect.Special_Clear_Row] =  new ClearRowEffect();
    }

    public ITileEffect GetTileEffect(TileEffect effect)
    {
        return _effectsMapping.ContainsKey(effect) ? _effectsMapping[effect] : null;
    }
}
