using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Rigidbody))]
public class Explosive : MonoBehaviour
{
    [SerializeField] [Range(1, 10)] private int _shardsToSpawnMinAmount = 2;
    [SerializeField] [Range(1, 10)] private int _shardsToSpawnMaxAmount = 6;
    [SerializeField] [Min(1)] private float _spawnRadius = 5;
    [SerializeField] [Min(1)] private float _explosionForce = 10;

    private Renderer _renderer;
    private float _divisionChance = 100f;
    
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

    public void Explode()
    {
        if (Random.Range(0, 100f) > _divisionChance)
        {
            Destroy(gameObject);
            return;
        }

        int shardsToSpawnAmount = Random.Range(_shardsToSpawnMinAmount, _shardsToSpawnMaxAmount);
        
        for (int i = 0; i < shardsToSpawnAmount; i++)
            SpawnShard();
        
        Destroy(gameObject);
    }

    private void SetRandomColor()
    {
        float minColorChannelValue = 0;
        float maxColorChannelValue = 1;

        float redChannel = Random.Range(minColorChannelValue, maxColorChannelValue);
        float greenChannel = Random.Range(minColorChannelValue, maxColorChannelValue);
        float blueChannel = Random.Range(minColorChannelValue, maxColorChannelValue);
        
        _renderer.material.color = new Color(redChannel, greenChannel, blueChannel, 1f);
    }

    private void SpawnShard()
    {
        Vector3 spawnDirection = Random.insideUnitSphere;
        spawnDirection.y = Random.value;
        
        Vector3 spawnPosition = transform.position + spawnDirection * _spawnRadius;
        
        var newShard = Instantiate(this, spawnPosition, Quaternion.identity) ;
        newShard.Initialize(transform.localScale/2, _divisionChance/2);
        
        newShard.GetComponent<Rigidbody>().AddForce(spawnDirection * _explosionForce, ForceMode.Impulse);
    }
    
    private void Initialize(Vector3 scale, float divisionChance)
    {
        transform.localScale = scale;
        _divisionChance = divisionChance;
    }
}
