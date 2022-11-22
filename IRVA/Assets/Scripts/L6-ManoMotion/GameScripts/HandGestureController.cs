using UnityEngine;

namespace AR_ManoMotion
{
    public class HandGestureController : MonoBehaviour
    {
        private FruitSpawner fruitSpawner;

        void Start()
        {
            fruitSpawner = GetComponentInChildren<FruitSpawner>();
        }

        void Update()
        {
            /** L6_TODO: Here you'll implement the gameplay changes based on hand and gesture information
             * __________________________________________________________________________________________
             *  First, get all the info you need
             *      -> Hand info, gesture info, continuous and trigger gestures, as well as hand side information
             *      
             *  Using this data, implement the following:
             *  
             *      -> When the handside is 'HandSide.Palmside', disable fruit spawning altogether. If it's 'HandSide.Backside' spawning should be active (default behavior)
             *      
             *      -> For each frame in which the continuous gesture 'CLOSED_HAND_GESTURE' is active, increase the spawn rate by 20 (or some number to see a difference)
             *         For any other continuous gesture the spawn rate should be kept as default
             *         Hint: Use the spawn rate variable found in the fruit spawner script
             *         You need to figure out where to use it, changing it's value won't do anything as it's not used in code
             *         
             *      -> When the trigger gesture 'PICK' is detected, destroy all fruit instances which are on-screen.
             *         Hint: Use 'DestroyFruitInstance()' function found on fruit controller script, as it plays particle and sound effect as well */
        }
    }
}