using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EliCDavis.Latice
{


    public class Driver : MonoBehaviour
    {

        /// <summary>
        /// The max time a sensor can take for reading a distance before timing
        /// out
        /// </summary>
        private readonly float sensorReadingTimeout = .06f;

        /// <summary>
        /// How often we update the LED PWM values
        /// </summary>
        private readonly float LEDChangeRate = 0.01f;

        [SerializeField]
        private Transform leftSensor;

        [SerializeField]
        private Transform middleSensor;

        [SerializeField]
        private Transform rightSensor;

        [SerializeField]
        private Transform[] LEDs;

        private LightController currentController;

        private float GetSensorDistance(Transform sensor)
        {
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(sensor.position, sensor.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {
                return hit.distance;
            }
            return -666;
        }

        private void Start()
        {
            currentController = new ProximityBlinkingController(LEDs.Length);
            StartCoroutine(SimulateReadingSensors());
            StartCoroutine(SimulateAdjustingPWM());
        }

        IEnumerator SimulateReadingSensors()
        {
            while (true)
            {
                currentController.SetSensorValues(new float[] {
                    GetSensorDistance(leftSensor),
                    GetSensorDistance(middleSensor),
                    GetSensorDistance(rightSensor)
                });

                // 3 for the number of sensors we're simulating
                yield return new WaitForSeconds(sensorReadingTimeout * 3);
            }
        }

        IEnumerator SimulateAdjustingPWM()
        {
            while (true)
            {
                yield return new WaitForSeconds(LEDChangeRate);

                var pwms = currentController.GetPWMValues(Time.time);

                for (int i = 0; i < pwms.Length; i++)
                {
                    LEDs[i].GetComponent<Light>().intensity = pwms[i];
                }
            }
        }

    }

}