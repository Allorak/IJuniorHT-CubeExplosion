using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Rigidbody))]
public class Explosive : MonoBehaviour
{
    [SerializeField] [Range(1, 10)] private int _shardsToSpawnMinAmount = 2;
    [SerializeField] [Range(1, 10)] private int _shardsToSpawnMaxAmount = 6;
    [SerializeField] [Min(1)] private float _spawnRadius = 5;
    [SerializeField] [Min(1)] private float _baseExplosionForce = 10;
    [SerializeField] [Min(1)] private float _explosionRadius = 15;

    private Renderer _renderer;
    private float _divisionChance = 100f;
    private float _decreaseFactor = 2f;
    private Collider[] _explosionColliders = new Collider[150];
    private float _explosionPowerIncreaseFactor = 1.8f;
    private float ExplosionDistanceToForceFactor => -_baseExplosionForce / _explosionRadius;

    private void OnValidate()
    {
        if (_shardsToSpawnMaxAmount < _shardsToSpawnMinAmount)
            _shardsToSpawnMaxAmount = _shardsToSpawnMinAmount;
    }

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        SetRandomColor();
    }

    public void Detonate()
    {
        if (Random.Range(0, 100f) > _divisionChance)
            Explode();
        else 
            Divide();

        Destroy(gameObject);
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
    
    private void Divide()
    {
        int shardsToSpawnAmount = Random.Range(_shardsToSpawnMinAmount, _shardsToSpawnMaxAmount);

        for (int i = 0; i < shardsToSpawnAmount; i++)
            SpawnShard();
    }

    private void SpawnShard()
    {
        Vector3 spawnDirection = Random.insideUnitSphere;
        spawnDirection.y = Random.value;
        
        Vector3 spawnPosition = transform.position + spawnDirection * _spawnRadius;
        
        Explosive newShard = Instantiate(this, spawnPosition, Quaternion.identity) ;
        Vector3 newScale = transform.localScale / _decreaseFactor;
        float newDivisionChance = _divisionChance / _decreaseFactor;
        float newBaseExplosionForce = _baseExplosionForce * _explosionPowerIncreaseFactor;
        float newExplosionRadius = _explosionRadius * _explosionPowerIncreaseFactor;
        newShard.Initialize(newScale, newDivisionChance, newBaseExplosionForce, newExplosionRadius);
        
        newShard.GetComponent<Rigidbody>().AddForce(spawnDirection * _baseExplosionForce, ForceMode.Impulse);
    }
    
    private void Initialize(Vector3 scale, float divisionChance, float baseExplosionForce, float explosionRadius)
    {
        transform.localScale = scale;
        _divisionChance = divisionChance;
        _baseExplosionForce = baseExplosionForce;
        _explosionRadius = explosionRadius;
    }

    private void Explode()
    {
        int collisionCount = Physics.OverlapSphereNonAlloc(transform.position, _explosionRadius, _explosionColliders);
        
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

        if (distanceToExplosion > _explosionRadius)
            return;

        float explosionForce = ExplosionDistanceToForceFactor * distanceToExplosion + _baseExplosionForce;
        Vector3 explosionDirection = (colliderRigidbody.position - currentPosition).normalized;
        colliderRigidbody.AddForce(explosionDirection * explosionForce, ForceMode.Impulse);
    }
}
