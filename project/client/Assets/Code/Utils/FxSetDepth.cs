using UnityEngine;
using System.Collections.Generic;

namespace DreamTown
{
    public class FxSetDepth : MonoBehaviour
    {
        public int Depth = 3001;
        public bool CloneMaterial = true;
        private int m_tmpDepth = 0;
        private List<Material> mCloneList = new List<Material>();

        public void Start()
        {
            SetFxDepth(Depth);
            m_tmpDepth = Depth;
            mCloneList.Clear();
        }


        public void Update()
        {
            if (Depth != m_tmpDepth)
            {
                SetFxDepth(Depth);
                m_tmpDepth = Depth;
            }
        }

        public void SetFxDepth(int depth)
        {
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                if (!CloneMaterial)
                {
                    Material[] shareMaterials = renderers[i].sharedMaterials;
                    for (int j = 0; j < shareMaterials.Length; j++)
                    {
                        if (shareMaterials[j])
                            shareMaterials[j].renderQueue = depth;
                    }
                    renderers[i].sharedMaterials = shareMaterials;
                }
                else
                {
                    Material[] shareMaterials = renderers[i].materials;
                    mCloneList.AddRange(shareMaterials);
                    for (int j = 0; j < shareMaterials.Length; j++)
                    {
                        if (shareMaterials[j])
                            shareMaterials[j].renderQueue = depth;
                    }
                    renderers[i].materials = shareMaterials;
                }
            }
        }


        void OnDestroy()
        {
            for (int i =0; i<mCloneList.Count; ++i)
            {
                Object.Destroy(mCloneList[i]);
            }
        }
    }
}