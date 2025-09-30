using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;

namespace Workspace.koto_thing
{
    public class PlayerDocumentModel : MonoBehaviour
    {
        [SerializeField, Tooltip("ドキュメントにインタラクトできる距離")] private float interactionDistance = 50.0f;
        
        public readonly Subject<DocumentData> OnOpenDocument = new();

        private bool loading = false;
        private bool documentOpen = false;

        public bool IsLoading => loading;
        public bool IsDocumentOpen { get => documentOpen; set => documentOpen = value; }
        
        public void TryInteract()
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, interactionDistance))
            {
                if (hit.collider.TryGetComponent<DocumentSource>(out var documentSource))
                {
                    StartCoroutine(LoadAndShow(documentSource));
                }
            }
        }

        private IEnumerator LoadAndShow(DocumentSource documentSource)
        {
            loading = true;

            var task = documentSource.LoadDataAsync();
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Exception == null && task.Result != null)
            {
                Debug.Log("Document loaded: " + task.Result.title);
                OnOpenDocument.OnNext(task.Result);
                IsDocumentOpen = true;
            }
            
            loading = false;
        }
    }
}