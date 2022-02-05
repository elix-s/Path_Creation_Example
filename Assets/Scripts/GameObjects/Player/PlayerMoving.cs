using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Zenject;

namespace DrawLine3D 
{
    ///<summary>
    ///класс перемещения кубика по траектории
    ///</summary>
    [RequireComponent(typeof(BoxCollider))]
    public class PlayerMoving : MonoBehaviour
    {
        #region Variables

        [SerializeField] private EndOfPathInstruction _endOfPathInstruction;
        [SerializeField] private float _timer;
        [SerializeField] private Vector3 _offset;
   
        private PathCreator _pathCreator;
        private float _speed; 
        private float _acceleration;
        private float _distanceTravelled;
        private int _counter;
        private float _startTimer;
        private float _startInertiaTimer;
        
        [SerializeField] private float _accelerationSpeed;
        [SerializeField] private float _inertiaTimer;
        [SerializeField] private float _basicSpeed;
        [SerializeField] private float _inertiaMotionTime;

        public bool InertialMotion;
        
        #endregion

        #region Methods

        [Inject]
        private void Construct(PathCreator path)
        {
            _pathCreator = path;
        }
    
        ///<summary>
        ///старт движения
        ///</summary>
        private void StartMotion()
        {
            _timer -= Time.deltaTime;

            if(_timer <=0)
            {
                if(_speed != _accelerationSpeed)
                    _speed = _accelerationSpeed;
            }
          
            Motion();
            _counter = 1;
        }

        ///<summary>
        ///метод движения по инерции
        ///</summary>
        private void InertiaMotion()
        {
            if(InertialMotion != true)
                InertialMotion = true;

            _inertiaTimer -= Time.deltaTime;

            if(_inertiaTimer > 0)
                Motion();

            if(_inertiaTimer <= 0)
                InertialMotion = false;
        }

        ///<summary>
        ///остановка движения
        ///</summary>
        private void StopMotion()
        {
            _timer = _startTimer;
            _inertiaTimer = _inertiaMotionTime;
            _speed = _basicSpeed;
            _counter = 0;
        }

        ///<summary>
        ///метод перемещения куба по пути, созданным PathCreator
        ///</summary>
        private void Motion()
        {
            _distanceTravelled += _speed * Time.deltaTime;
            transform.position = _pathCreator.path.GetPointAtDistance(_distanceTravelled, _endOfPathInstruction) + _offset;
            transform.rotation = _pathCreator.path.GetRotationAtDistance(_distanceTravelled, _endOfPathInstruction);
        }

        ///<summary>
        ///метод апдейта PathCreator после изменения
        ///</summary>
        private void OnPathChanged() 
        {
            _distanceTravelled = _pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }

        ///<summary>
        ///считаем пройденную дистанцию
        ///</summary>
        public float CalculatingTravelledDistance()
        {
            float Distance = _distanceTravelled/_pathCreator.path.length; 
            return Mathf.Round(Distance * 100.0f) * 0.01f;
        }

        #endregion

        #region MonoBehaviour
        
        void Start()
        {
            _speed = _basicSpeed;
            _acceleration = _accelerationSpeed;
            _startTimer = _timer;
            _startInertiaTimer = _inertiaMotionTime;

            if (_pathCreator != null)
                _pathCreator.pathUpdated += OnPathChanged;

            //ставим куб в первую точку пути
            transform.position = _pathCreator.path.GetPointAtDistance(_distanceTravelled) + _offset;
            transform.rotation = _pathCreator.path.GetRotationAtDistance(_distanceTravelled);
        }

        void FixedUpdate()
        {
            if(Input.anyKey)
            {
                #if UNITY_ANDROID && !UNITY_EDITOR

                foreach (Touch touch in Input.touches)
                {
                    int id = touch.fingerId;
                    if (!EventSystem.current.IsPointerOverGameObject(id))
                        StartMotion();
                } 

                #endif
                
                #if UNITY_ANDROID && UNITY_EDITOR 

                //Проверка, что клик сделан не над элементом интерфейса
                if(!EventSystem.current.IsPointerOverGameObject())          
                    StartMotion();                
                
                #endif                              
            }

            if(!Input.anyKey && _counter == 1)
                InertiaMotion();
                    
            if(!Input.anyKey && _counter == 1 && InertialMotion == false)
                StopMotion();         
        }

        #endregion
    }
}
