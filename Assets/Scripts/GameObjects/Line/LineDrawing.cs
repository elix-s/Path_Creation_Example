using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace DrawLine3D 
{
    [RequireComponent(typeof(LineRenderer))]
    public class LineDrawing : MonoBehaviour
    {
        #region Variables

        private GameObject _playerCube;
        private LineRenderer _line;  

        private int _positionToUpdate = 1;
        private Transform _cubePosition;
        private int _counter;
        private Vector3 _cubeStartPosition;
        private PlayerMoving _playerMoving;

        [SerializeField] private Vector3 _offset;
            
        #endregion

        #region Methods

        [Inject]
        private void Construct(Player player)
        {
            _playerCube = player.gameObject;
        }

        ///<summary>
        ///рисование линии за кубом через LineRenderer
        ///</summary>
        private void Drawing()
        {
            _line.SetPosition(_positionToUpdate, _cubePosition.position + _offset);
            _positionToUpdate ++;
            _line.positionCount = _positionToUpdate + 1;
            _line.SetPosition(_positionToUpdate, _cubePosition.position + _offset);
        }

        private void StartPosition()
        {           
            _cubeStartPosition = _cubePosition.position;
            _line.SetPosition(0, _cubeStartPosition + _offset);  
        } 
                
        private void SetVisible()
        {
            GetComponent<LineRenderer>().enabled = true;
        }

        #endregion

        #region MonoBehaviour
            
        void Start()
        { 
            _line = GetComponent<LineRenderer>();
            _cubePosition = _playerCube.GetComponent<Transform>();;
            _cubeStartPosition = transform.position;
            _playerMoving = _playerCube.GetComponent<PlayerMoving>();

            GetComponent<LineRenderer>().enabled = false;  
            Invoke("StartPosition",0.1f);     
        }

        void FixedUpdate()
        {
            if(Input.anyKey || _playerMoving.InertialMotion == true)
            {
                Drawing();               
            }

            if(Input.anyKey && _counter == 0)
            {
                SetVisible();
                _counter = 1;               
            }    
        }

        #endregion
    }
}
