using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use

        public float speed = 0;
        public float steering = 0;

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }


        private void FixedUpdate()
        {
            // pass the input to the car!
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
#if UNITY_EDITOR
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
#endif

            speed = Mathf.Clamp(speed + (v * 10 * Time.deltaTime), 0, 30);
            steering = Mathf.Clamp(steering + (h * Time.deltaTime), -1, 1);

            m_Car.Move(steering, speed, v, 0f);
        }
    }
}
