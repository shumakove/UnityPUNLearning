using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace com.mygame.soho
{
    public class PlayerAnimatorManager : Photon.MonoBehaviour
    {
        private Animator animator;

        #region PUPLIC PROPERTIES
        public float DirectionDampTime = .25f;
        #endregion


        #region MONOBEHAVIOUR MESSAGES
        // Use this for initialization
        void Start()
        {
            animator = GetComponent<Animator>();
            if(!animator)
            {
                Debug.LogError("PlayerAnimatorManager is Missing Animator component", this);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(photonView.isMine == false && PhotonNetwork.connected == true)
            {
                return;
            }

            if(!animator)
            {
                return;
            }

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if(stateInfo.IsName("Base Layer.Run"))
            {
                if(Input.GetButtonDown("Fire2"))
                {
                    animator.SetTrigger("Jump");
                }
            }

            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            if( v< 0)
            {
                v = 0;
            }

            animator.SetFloat("Speed", h * h + v * v);
            animator.SetFloat("Direction", h, DirectionDampTime, Time.deltaTime);
        }
        #endregion
    }
}
