using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Spawner))]
public class Explosive : MonoBehaviour
{
    [field: SerializeField] [Min(1)] public float BaseExplosionForce  { get; private set; } = 10;
    [field: SerializeField] [Min(1)] public float ExplosionRadius  { get; private set; } = 15;
    
    private Renderer _renderer;
    private Spawner _spawner;
    private Collider[] _explosionColliders = new Collider[150];

    public float DivisionChance { get; private set; } = 100f;
    private float ExplosionDistanceToForceFactor => -BaseExplosionForce / ExplosionRadius;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _spawner = GetComponent<Spawner>();
    }

    private void Start()
    {
        SetRandomColor();
    }

    public void Detonate()
    {
        float minPercentage = 0f;
        float maxPercentage = 100f;
    
        if (Random.Range(minPercentage, maxPercentage) > DivisionChance)
            Explode();
        else 
            _spawner.SpawnShards(this);

        Destroy(gameObject);
    }
    
    public void Initialize(Vector3 scale, float divisionChance, float baseExplosionForce, float explosionRadius)
    {
        transform.localScale = scale;
        DivisionChance = divisionChance;
        BaseExplosionForce = baseExplosionForce;
        ExplosionRadius = explosionRadius;
    }
    
    private void SetRandomColor()
    {
        float minColorChannelValue = 0;
        float maxColorChannelValue = 1;

        float redChannel = Random.Range(minColorChannelValue, maxColorChannelValue);
        float greenChannel = Random.Range(minColorChannelValue, maxColorChannelValue);
        float blueChannel = Random.Range(minColorChannelValue, maxColorChannelValue);
        
        _renderer.material.color = new Color(redChannel, greenChannel, blueChannel);
    }

    private void Explode()
    {
        int collisionCount = Physics.OverlapSphereNonAlloc(transform.position, ExplosionRadius, _explosionColliders);
        
        if(collisionCount == 0)
            return;

        Vector3 currentPosition = transform.position;

        for (int i = 0; i < collisionCount; i++)
        {
            ExplodeCollider(_explosionColliders[i], currentPosition);
        }
    }

    private void ExplodeCollider(Collider explodingCollider, Vector3 currentPosition)
    {
        if (explodingCollider.TryGetComponent(out Rigidbody colliderRigidbody) == false)
            return;
        
        float distanceToExplosion = Vector3.Distance(colliderRigidbody.position, currentPosition);

        if (distanceToExplosion > ExplosionRadius)
            return;

        float explosionForce = ExplosionDistanceToForceFactor * distanceToExplosion + BaseExplosionForce;
        Vector3 explosionDirection = (colliderRigidbody.position - currentPosition).normalized;
        colliderRigidbody.AddForce(explosionDirection * explosionForce, ForceMode.Impulse);
    }
}
