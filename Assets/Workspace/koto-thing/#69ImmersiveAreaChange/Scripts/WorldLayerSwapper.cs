using UnityEngine;
using UnityEngine.Rendering;

namespace Workspace.koto_thing
{
    public class WorldLayerSwapper : MonoBehaviour
    {
        [SerializeField, Tooltip("入れ替え対象となるレイヤー名A")]
        private string layerNameA = "MainWorld";

        [SerializeField, Tooltip("入れ替え対象となるレイヤー名B")]
        private string layerNameB = "PortalWorld";

        private static readonly int StencilID = Shader.PropertyToID("_StencilComp");

        private int layerA = -1;
        private int layerB = -1;
        private bool valid;

        private void Awake()
        {
            layerA = LayerMask.NameToLayer(layerNameA);
            layerB = LayerMask.NameToLayer(layerNameB);

            valid = layerA >= 0 && layerB >= 0 && layerA != layerB;
            if (!valid)
            {
                Debug.LogError($"[WorldLayerSwapper] レイヤー名が不正です: A='{layerNameA}'({layerA}), B='{layerNameB}'({layerB})", this);
                enabled = false;
            }
        }

        // 直下の子から再帰的に入れ替える（ルート自身は含めない）
        public void SwapAllChildLayers()
        {
            if (!valid) return;

            foreach (Transform child in transform)
                RecursiveSwapLayer(child);
        }

        // ルート自身も含めて入れ替える
        public void SwapSelfAndChildren()
        {
            if (!valid) return;
            RecursiveSwapLayer(transform);
        }

        private void RecursiveSwapLayer(Transform target)
        {
            var go = target.gameObject;
            int currentLayer = go.layer;

            if (currentLayer == layerA)
            {
                go.layer = layerB;
                SetStencilCompare(target, CompareFunction.Equal);
            }
            else if (currentLayer == layerB)
            {
                go.layer = layerA;
                SetStencilCompare(target, CompareFunction.Always);
            }

            foreach (Transform child in target)
                RecursiveSwapLayer(child);
        }

        private static void SetStencilCompare(Transform t, CompareFunction func)
        {
            var renderer = t.GetComponent<Renderer>();
            if (renderer == null) return;

            var mats = renderer.sharedMaterials;
            if (mats == null) return;

            int value = (int)func;
            for (int i = 0; i < mats.Length; i++)
            {
                var mat = mats[i];
                if (mat != null && mat.HasProperty(StencilID))
                {
                    mat.SetInt(StencilID, value);
                }
            }
        }
    }
}
