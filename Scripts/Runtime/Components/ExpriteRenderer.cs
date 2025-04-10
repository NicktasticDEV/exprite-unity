using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Exprite
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ExpriteRenderer : MonoBehaviour
    {
        // Public fields
        public ExpriteAnimationPack AnimationPack;
        public bool PreloadAnimations = false;

        public AnimationDefinition? CurrentAnimation { get; private set; }
        public bool IsPlaying { get; private set; }
        public int CurrentFrame { get; private set; }

        // Private fields
        private static Dictionary<ExpriteAnimationPack, Dictionary<string, List<Sprite>>> _preloadedAnimations = new Dictionary<ExpriteAnimationPack, Dictionary<string, List<Sprite>>>();
        private SpriteRenderer _spriteRenderer;
        private ExpriteAnimationPack _previousAnimationPack;

        #region Lifecycle

        // Initialize stuff
        void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _previousAnimationPack = AnimationPack;

            if (PreloadAnimations && AnimationPack != null)
            {
                PreloadAnimationPack();
            }

            #if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnExitPlayMode;
            #endif 
        }

        void Update()
        {
            // Check if the animation pack has changed
            if (_previousAnimationPack != AnimationPack && AnimationPack != null)
            {
                OnSparrowAnimationPackChanged();
                _previousAnimationPack = AnimationPack;
            }
        }

        #endregion

        #region Animation Controls

        public void Play(string animationName, int frame=0)
        {
            if (IsPlaying)
            {
                StopAllCoroutines();
            }

            StartCoroutine(PlayAnimation(animationName, frame));
        }

        #endregion

        #region Implementation Details

        IEnumerator PlayAnimation(string animationName, int frame=0)
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
                    timeAccumulator -= timePerFrame;

                    if (!PreloadAnimations)
                    {
                        SubTexture subTextureFrame = subTextures[frameIndex];
                        float adjustedY = AnimationPack.texture.height - subTextureFrame.y - subTextureFrame.height;

                        Vector2 pivot = new Vector2(
                            (subTextureFrame.frameX - animation.offset.x - AnimationPack.globalOffset.x) / subTextureFrame.width,
                            1f - ((subTextureFrame.frameY + animation.offset.y + AnimationPack.globalOffset.y) / subTextureFrame.height)
                        );

                        _spriteRenderer.sprite = Sprite.Create(
                            AnimationPack.texture,
                            new Rect(subTextureFrame.x, adjustedY, subTextureFrame.width, subTextureFrame.height),
                            pivot
                        );
                    }
                    else
                    {
                        _spriteRenderer.sprite = _preloadedAnimations[AnimationPack][animationName][frameIndex];
                    }

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

        void PreloadAnimationPack()
        {
            if (!_preloadedAnimations.ContainsKey(AnimationPack))
            {
                Dictionary<string, List<Sprite>> animations = new Dictionary<string, List<Sprite>>();

                foreach (AnimationDefinition animation in AnimationPack.animations)
                {
                    List<Sprite> sprites = new List<Sprite>();

                    SubTexture[] subTextures = AnimationPack.GetSubTexturesFromAnimationDefinition(animation);

                    foreach (SubTexture subTexture in subTextures)
                    {
                        float adjustedY = AnimationPack.texture.height - subTexture.y - subTexture.height;

                        Vector2 pivot = new Vector2(
                            (subTexture.frameX - animation.offset.x - AnimationPack.globalOffset.x) / subTexture.width,
                            1f - ((subTexture.frameY + animation.offset.y + AnimationPack.globalOffset.y) / subTexture.height)
                        );

                        Sprite sprite = Sprite.Create(
                            AnimationPack.texture,
                            new Rect(subTexture.x, adjustedY, subTexture.width, subTexture.height),
                            pivot
                        );

                        sprites.Add(sprite);
                    }

                    animations.Add(animation.name, sprites);
                }

                _preloadedAnimations.Add(AnimationPack, animations);
            }
            else
            {
                Debug.Log("Animation Pack already preloaded");
            }
        }

        void OnSparrowAnimationPackChanged()
        {
            Debug.Log("Sparrow Animation Pack Changed");
            if (PreloadAnimations)
            {
                PreloadAnimationPack();
            }
        }

        #endregion

        #region Miscellaneous
    
        #if UNITY_EDITOR
        void OnExitPlayMode(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                _preloadedAnimations.Clear();
            }
        }
        #endif

        #endregion
    }
}