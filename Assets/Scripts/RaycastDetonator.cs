using UnityEngine;

public class RaycastDetonator : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Detonate();
    }

    private void Detonate()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity) == false)
            return;
        
        if(hit.transform.TryGetComponent(out Explosive explosive) == false)
            return;

        explosive.Explode();
    }
}
