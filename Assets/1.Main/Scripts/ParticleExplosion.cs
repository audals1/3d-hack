using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleExplosion : MonoBehaviour
{
    SkillData skillData;
    
    void OnParticleCollision(GameObject other)
    {
        if(other.CompareTag("Monster"))
        {
            var monster = other.GetComponent<MonsterController>();
            monster.SetDamage(AttackType.Normal,20f, skillData);
            
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
}
