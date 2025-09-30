using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Exprite
{
    [AddComponentMenu("Exprite/Exprite Renderer")]

    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class ExpriteRenderer : MonoBehaviour
    {
        // Public fields
        public ExpriteAnimationPack AnimationPack;

        public AnimationDefinition? CurrentAnimation { get; private set; }
        public bool IsPlaying { get; private set; }
        public int CurrentFrame { get; private set; }

        // Private fields
        private static Dictionary<ExpriteAnimationPack, Dictionary<string, List<Sprite>>> _preloadedAnimations = new Dictionary<ExpriteAnimationPack, Dictionary<string, List<Sprite>>>();
        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;
        private ExpriteAnimationPack _previousAnimationPack;

        #region Lifecycle

        // Initialize stuff
        void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshFilter = GetComponent<MeshFilter>();
            _previousAnimationPack = AnimationPack;
        }

        void Update()
        {
            // Check if the animation pack has changed
            if (_previousAnimationPack != AnimationPack && AnimationPack != null)
            {
                _previousAnimationPack = AnimationPack;
            }
        }

        #endregion

        #region Animation Controls

        public void Play(string animationName, int frame = 0)
        {
            if (IsPlaying)
            {
                StopAllCoroutines();
            }

            StartCoroutine(PlayAnimation(animationName, frame));
        }

        #endregion

        #region Implementation Details

        IEnumerator PlayAnimation(string animationName, int frame = 0)
        {
            AnimationDefinition animation = AnimationPack.GetAnimationDefinitionByName(animationName);

            IsPlaying = true;
            CurrentAnimation = animation;

            int frameIndex = frame;
            float timePerFrame = 1f / animation.fps;
            float timeAccumulator = 0f;

            SubTexture[] subTextures = AnimationPack.GetSubTexturesFromAnimationDefinition(animation);

            while (true)
            {
                timeAccumulator += Time.deltaTime;

                // Check if we need to move to the next frame
                if (timeAccumulator >= timePerFrame)
                {
                    animation = AnimationPack.GetAnimationDefinitionByName(animationName);
                    timeAccumulator -= timePerFrame;

                    // Update the sprite to the current frame
                    SubTexture subTextureFrame = subTextures[frameIndex];
                    
                    Mesh mesh = new Mesh();
                    float width = subTextureFrame.width / AnimationPack.texture.pixelsPerUnit;
                    float height = subTextureFrame.height / AnimationPack.texture.pixelsPerUnit;

                    float frameWidth = subTextureFrame.frameWidth == 0 ? subTextureFrame.width : subTextureFrame.frameWidth;
                    float frameHeight = subTextureFrame.frameHeight == 0 ? subTextureFrame.height : subTextureFrame.frameHeight;

                    float xOffset = ((subTextureFrame.frameX + frameWidth / 2) - animation.offset.x - AnimationPack.globalOffset.x) / AnimationPack.texture.pixelsPerUnit;
                    float yOffset = ((subTextureFrame.frameY - subTextureFrame.height + frameHeight / 2) - animation.offset.y - AnimationPack.globalOffset.y) / AnimationPack.texture.pixelsPerUnit;

                    // Vertices
                    Vector3[] vertices = new Vector3[]
                    {
                        new Vector3(0 - xOffset, 0 + yOffset, 0),
                        new Vector3(width - xOffset, 0 + yOffset, 0),
                        new Vector3(0 - xOffset, height + yOffset, 0),
                        new Vector3(width - xOffset, height + yOffset, 0)
                    };

                    // Triangles
                    int[] triangles = new int[]
                    {
                        0, 2, 1,
                        2, 3, 1
                    };

                    // UVs
                    float texWidth = AnimationPack.texture.texture.width;
                    float texHeight = AnimationPack.texture.texture.height;
                    // Sparrow is top-left origin based while Unity's UV origin is bottom-left based
                    // Convert by flipping the V coordinates
                    float uMin = subTextureFrame.x / texWidth;
                    float uMax = (subTextureFrame.x + subTextureFrame.width) / texWidth;
                    float vTopSource = subTextureFrame.y;
                    float vBottomSource = subTextureFrame.y + subTextureFrame.height;
                    float vMin = 1f - vBottomSource / texHeight;
                    float vMax = 1f - vTopSource / texHeight;
                    Vector2[] uvs = new Vector2[]
                    {
                        new Vector2(uMin, vMin),
                        new Vector2(uMax, vMin),
                        new Vector2(uMin, vMax),
                        new Vector2(uMax, vMax)
                    };

                    // Set
                    mesh.vertices = vertices;
                    mesh.triangles = triangles;
                    mesh.uv = uvs;
                    mesh.RecalculateNormals();
                    _meshFilter.mesh = mesh;
                    _meshRenderer.material.mainTexture = AnimationPack.texture.texture;
                    
                    // Frame done
                    frameIndex++;

                    // Check if we reached the end of the animation
                    if (frameIndex >= subTextures.Length)
                    {
                        //Check if animation is supposed to loop
                        if (animation.loop)
                        {
                            frameIndex = 0;
                        }
                        else
                        {
                            IsPlaying = false;
                            CurrentAnimation = null;
                            yield break;
                        }
                    }
                }

                yield return null;
            }
        }
        #endregion
    }
}