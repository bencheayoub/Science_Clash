using UnityEngine;

public abstract class EffectData : ScriptableObject
{
    [SerializeField, TextArea] private string description;
    public string Description => description;

    public abstract void Apply(in EffectContext context);
}