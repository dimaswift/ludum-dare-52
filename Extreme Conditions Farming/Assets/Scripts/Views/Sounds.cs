using System;
using UnityEngine;

namespace ECF.Views
{
    [CreateAssetMenu(menuName = "ECF/Sounds")]
    public class Sounds : ScriptableObject
    {
        public AudioClip[] error;
        public AudioClip[] coins;
    }
}