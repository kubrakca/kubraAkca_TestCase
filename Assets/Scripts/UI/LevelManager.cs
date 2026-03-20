using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace UI
{
    public class LevelManager : MonoBehaviour
    {
        public List<LevelStatus> GetLevels()
        {
            return new List<LevelStatus> 
            { 
                LevelStatus.Completed,
                LevelStatus.Completed, 
                LevelStatus.Active,    
                LevelStatus.Locked,   
                LevelStatus.Locked     
            };
        }
    }
}