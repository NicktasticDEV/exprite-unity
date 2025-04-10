using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;
using Exprite;
using System;

[CreateAssetMenu(fileName = "ExpriteAnimationPack", menuName = "Exprite/Exprite Animation Pack", order = 1)]
public class ExpriteAnimationPack : ScriptableObject
{
    // Public fields
    public Texture2D texture;
    public TextAsset atlas;
    public Vector2 globalOffset;
    public AnimationDefinition[] animations;

    // Private fields
    XmlSerializer serializer = new XmlSerializer(typeof(TextureAtlas));

    // Functions
    /// <summary> Retrieves an animation definition by its name. </summary>
    /// <param name="animationName">The name of the animation to retrieve.</param>
    /// <returns>The animation definition with the specified name.</returns>
    public AnimationDefinition GetAnimationDefinitionByName(string animationName)
    {
        foreach (AnimationDefinition animation in animations)
        {
            if (animation.name == animationName)
            {
                return animation;
            }
        }

        throw new Exception("Animation not found: " + animationName);
    }

    /// <summary> Retrieves all SubTexture data from an animation. </summary>
    /// <param name="animation">The name of the animation to retrieve.</param>
    /// <returns>The animation definition with the specified name.</returns>
    public SubTexture[] GetSubTexturesFromAnimationDefinition(AnimationDefinition animation)
    {
        TextureAtlas textureAtlas = (TextureAtlas)serializer.Deserialize(new System.IO.StringReader(atlas.text));

        // Get all Subtextures from the animation that have the prefix defined in the animation
        List<SubTexture> subTextures = new List<SubTexture>();

        foreach (SubTexture subTexture in textureAtlas.SubTexture)
        {
            if (subTexture.name.StartsWith(animation.prefix))
            {
                subTextures.Add(subTexture);
            }
        }

        return subTextures.ToArray();
    }
}