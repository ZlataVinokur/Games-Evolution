using UnityEngine;

 
public class ParallaxBackground : MonoBehaviour
    {
        [System.Serializable]
        public struct Layer
        {
            public Transform layerTransform;
            [Range(0f, 1f)] public float verticalParallaxFactor;
        }

        [SerializeField] private Layer[] layers;
        [SerializeField] private Transform cameraTransform;
        private Vector3 previousCamPos;

        private void Start()
        {
            if (cameraTransform == null) cameraTransform = Camera.main.transform;
            previousCamPos = cameraTransform.position;
        }

        private void LateUpdate()
        {
            Vector3 delta = cameraTransform.position - previousCamPos;
            foreach (Layer layer in layers)
            {
                Vector3 newPos = layer.layerTransform.position;
                newPos.y += delta.y * layer.verticalParallaxFactor;
                layer.layerTransform.position = newPos;
            }
            previousCamPos = cameraTransform.position;
        }
    }
 