using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] [Range(1, 10)] private int _shardsToSpawnMinAmount = 2;
    [SerializeField] [Range(1, 10)] private int _shardsToSpawnMaxAmount = 6;
    [SerializeField] [Min(1)] private float _spawnRadius = 5;
    
    private float _decreaseFactor = 2f;
    private float _explosionPowerIncreaseFactor = 1.8f;
    
    private void OnValidate()
    {
        if (_shardsToSpawnMaxAmount < _shardsToSpawnMinAmount)
            _shardsToSpawnMaxAmount = _shardsToSpawnMinAmount;
    }
    
    public void SpawnShards(Explosive originalCube)
    {
        int shardsToSpawnAmount = Random.Range(_shardsToSpawnMinAmount, _shardsToSpawnMaxAmount);

        for (int i = 0; i < shardsToSpawnAmount; i++)
            SpawnShard(originalCube);
    }

    private void SpawnShard(Explosive originalCube)
    {
        Vector3 spawnDirection = Random.insideUnitSphere;
        spawnDirection.y = Random.value;
        
        Vector3 spawnPosition = originalCube.transform.position + spawnDirection * _spawnRadius;
        
        Explosive newShard = Instantiate(originalCube, spawnPosition, Quaternion.identity) ;
        Vector3 newScale = originalCube.transform.localScale / _decreaseFactor;
        float newDivisionChance = originalCube.DivisionChance / _decreaseFactor;
        float newBaseExplosionForce = originalCube.BaseExplosionForce * _explosionPowerIncreaseFactor;
        float newExplosionRadius = originalCube.ExplosionRadius * _explosionPowerIncreaseFactor;
        newShard.Initialize(newScale, newDivisionChance, newBaseExplosionForce, newExplosionRadius);

        if (newShard.TryGetComponent(out Rigidbody newShardRigidbody))
            newShardRigidbody.AddForce(spawnDirection * originalCube.BaseExplosionForce, ForceMode.Impulse);
    }
}
