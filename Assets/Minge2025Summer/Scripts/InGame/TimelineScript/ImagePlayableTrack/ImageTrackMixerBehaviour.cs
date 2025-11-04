using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Minge2025Summer.Scripts.InGame.TimelineScript.ImagePlayableTrack
{
    public class ImageTrackMixerBehaviour : PlayableBehaviour
    {
        private Sprite m_DefaultSprite;
        private Color m_DefaultColor;
        private Material m_DefaultMaterial;

        private Image m_TrackBinding;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            SetDefaults(playerData as Image);
            if (m_TrackBinding == null)
                return;

            int inputCount = playable.GetInputCount();

            Sprite blendedSprite = null;
            Color blendedColor = Color.clear;
            Material blendedMaterial = null;
            float totalWeight = 0f;
            float greatestWeight = 0f;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<ImagePlayableBehaviour> inputPlayable = (ScriptPlayable<ImagePlayableBehaviour>)playable.GetInput(i);
                ImagePlayableBehaviour input = inputPlayable.GetBehaviour();

                blendedColor += input.color * inputWeight;
                totalWeight += inputWeight;

                // use the sprite and material with the highest weight
                if (inputWeight > greatestWeight)
                {
                    blendedSprite = input.sprite;
                    blendedMaterial = input.material;
                    greatestWeight = inputWeight;
                }
            }
            
            if (totalWeight <= 0f)
            {
                m_TrackBinding.sprite = null;
                m_TrackBinding.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                m_TrackBinding.material = null;
                return;
            }

            // blend to the default values
            m_TrackBinding.sprite = blendedSprite != null ? blendedSprite : m_DefaultSprite;
            m_TrackBinding.color = Color.Lerp(m_DefaultColor, blendedColor, totalWeight);
            m_TrackBinding.material = blendedMaterial != null ? blendedMaterial : m_DefaultMaterial;
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            RestoreDefaults();
        }

        private void SetDefaults(Image image)
        {
            if (image == m_TrackBinding)
                return;

            RestoreDefaults();

            m_TrackBinding = image;
            if (m_TrackBinding != null)
            {
                m_DefaultSprite = m_TrackBinding.sprite;
                m_DefaultColor = m_TrackBinding.color;
                m_DefaultMaterial = m_TrackBinding.material;
            }
        }

        private void RestoreDefaults()
        {
            if (m_TrackBinding == null)
                return;
            
            m_TrackBinding.sprite = m_DefaultSprite;
            m_TrackBinding.color = m_DefaultColor;
            m_TrackBinding.material = m_DefaultMaterial;
        }
    }
}