using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace UnityStandardAssets.CrossPlatformInput
{
    public class ButtonHandler : MonoBehaviour
    {

        public string Name;
        public bool pressed = false;
      
        void OnEnable()
        {

        }
      //  IEnumerator wait()
      //  {
      //      yield return new WaitForSeconds(1);
      //  }

        public void SetDownState()
        {
            CrossPlatformInputManager.SetButtonDown(Name);
            pressed = true;
        }


        public void SetUpState()
        {
            CrossPlatformInputManager.SetButtonUp(Name);
            pressed = false;
        }


        public void SetAxisPositiveState()
        {
            CrossPlatformInputManager.SetAxisPositive(Name);
        }


        public void SetAxisNeutralState()
        {
            CrossPlatformInputManager.SetAxisZero(Name);
        }


        public void SetAxisNegativeState()
        {
            CrossPlatformInputManager.SetAxisNegative(Name);
        }

        public void Update()
        {
          //  if (pressed==true)
          //  {
          //      StartCoroutine(wait());
          //  }
        }
        
    }
}
