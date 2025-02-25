using UnityEngine;

public class EffectController : MonoBehaviour
{
    // 자동 파괴 타이머
    public float lifetime = 2f;
    
    // 파티클 시스템 참조
    private ParticleSystem[] particleSystems;
    
    void Start()
    {
        // 모든 파티클 시스템 가져오기
        particleSystems = GetComponentsInChildren<ParticleSystem>();
        
        // 라이프타임 후 자동 파괴
        Destroy(gameObject, lifetime);
    }
    
    // 모든 파티클 시스템 재생
    public void PlayAllEffects()
    {
        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Play();
        }
    }
    
    // 모든 파티클 시스템 중지
    public void StopAllEffects()
    {
        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Stop();
        }
    }
}