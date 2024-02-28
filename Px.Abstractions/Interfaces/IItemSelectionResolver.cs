﻿namespace Px.Abstractions.Interfaces
{
    public interface IItemSelectionResolver
    {
        ItemSelection Resolve(string language, string selection, out bool selectionExists);
    }
}
